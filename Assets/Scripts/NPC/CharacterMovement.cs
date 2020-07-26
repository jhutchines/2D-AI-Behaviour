using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public GameObject buildingOwned;
    public GameObject buildingInside;
    public bool hasHome;
    public GameObject bedOwned;
    public float characterSpeed;

    CharacterStats characterStats;

    DayNightCycle time;
    float sunsetTime;
    float sunriseTime;
    public float sleepHour;
    public float sleepMinute;
    public float wakeUpHour = 6;

    bool isStarving;
    bool foundFood;
    GameObject foodStore;
    BuildingParts.FoodStores foodFound;

    bool isBusy;
    bool isDoingHobby;
    bool findNewTask;
    float changeTask;
    float currentTimePassed;

    int findChair;
    Vector2 shopCounter;

    // Start is called before the first frame update
    void Start()
    {
        characterStats = GetComponent<CharacterStats>();
        time = GameObject.Find("Time").GetComponent<DayNightCycle>();
        characterStats.nav.destination = new Vector3(transform.position.x + Random.Range(-3, 4), 0, transform.position.z + Random.Range(-3, 4));
        characterSpeed = characterStats.nav.speed;
    }

    // Update is called once per frame
    void Update()
    {
        // Sets character speed based on time speed multiplier
        characterStats.nav.speed = characterSpeed * time.currentTimeSpeed;

        // Checks to make sure they don't already own a building and buy one if one for sale and character can afford it
        if (buildingOwned == null)
        {
            hasHome = false;
            LookForBuilding();
        }
        else hasHome = true;

        CheckBuilding();

        CheckSleep();
        CheckFood();
        FindNewTask();

        FindWork();
        WorkingHours();
        JobTasks();
    }

    // Checks to see if the character is inside a building
    void CheckBuilding()
    {
        WorldGrid grid = GameObject.Find("Plane").GetComponent<WorldGrid>();
        Vector2 currentLocation = new Vector2(transform.position.x, transform.position.z);
        if (grid.gridLocations[Mathf.RoundToInt(currentLocation.x), Mathf.RoundToInt(currentLocation.y)] != null)
        {
            buildingInside = grid.gridLocations[Mathf.RoundToInt(currentLocation.x), Mathf.RoundToInt(currentLocation.y)].transform.parent.gameObject;
        }
        else buildingInside = null;
    }

    // Looks for a building if the character doesn't already have one
    void LookForBuilding()
    {
        GameObject plane = GameObject.Find("Plane");

        // Checks to see if they already own a building in the world
        for (int i = 0; i < plane.transform.childCount; i++)
        {
            GameObject obj = plane.transform.GetChild(i).gameObject;
            if (obj.GetComponent<BuildingStats>() != null)
            {
                if (obj.GetComponent<BuildingStats>().ownedBy == gameObject)
                {
                    buildingOwned = obj;
                    break;
                }
            }
        }

        // Looks for a building to purchase, and buys if can afford it
        for (int i = 0; i < plane.transform.childCount; i++)
        {
            GameObject obj = plane.transform.GetChild(i).gameObject;
            if (obj.GetComponent<BuildingStats>() != null)
            {
                if (obj.GetComponent<BuildingStats>().salePrice != 0 && characterStats.currentMoney >= obj.GetComponent<BuildingStats>().salePrice)
                {
                    buildingOwned = obj;
                    obj.GetComponent<BuildingStats>().ownedBy = gameObject;
                    characterStats.currentMoney -= obj.GetComponent<BuildingStats>().salePrice;
                    obj.GetComponent<BuildingStats>().salePrice = 0;
                    break;
                }
            }
        }
        
    }

    // Character walks around their house if their task is Idling
    IEnumerator IdleTasks()
    {
        isDoingHobby = true;
        int rand = Random.Range(0, buildingOwned.GetComponent<BuildingStats>().buildingArea.Length);
        if (buildingOwned.GetComponent<BuildingStats>().buildingArea[rand] == BuildingParts.BuildingPartType.Floor ||
            (buildingOwned.GetComponent<BuildingStats>().buildingArea[rand] == BuildingParts.BuildingPartType.Furniture &&
            buildingOwned.GetComponent<BuildingStats>().furnitureType[rand] == BuildingParts.FurnitureType.Chair))
        {
            characterStats.nav.destination = new Vector3(buildingOwned.GetComponent<BuildingStats>().buildingAreaLocation[rand].x, 0,
                                                            buildingOwned.GetComponent<BuildingStats>().buildingAreaLocation[rand].y);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
        isDoingHobby = false;
    }

    // Character will find a new task if they are currently not busy with anything
    void FindNewTask()
    {
        // Picks a new task to do from the character's hobbies
        if (findNewTask && !isBusy)
        {
            // Characters are more likely to stick with the hobby they are already doing
            int randomChoice = characterStats.hobbies.hobbies.Length + 3;

            int pickRandom = Random.Range(0, randomChoice);
            if (pickRandom >= characterStats.hobbies.hobbies.Length) characterStats.currentTask = characterStats.idleTask;
            else characterStats.currentTask = characterStats.hobbies.hobbies[pickRandom];

            characterStats.idleTask = characterStats.currentTask;
            // Picks a random time to look for a new task to do
            changeTask = currentTimePassed + Random.Range(10, 40);
            isDoingHobby = false;
            findNewTask = false;
        }

        // Search for a new task if the random time has passed
        currentTimePassed += Time.deltaTime * time.currentTimeSpeed;
        if (currentTimePassed > changeTask) findNewTask = true;

        if (characterStats.currentTask == CharacterTasks.CurrentTask.Idling && hasHome && !isDoingHobby) StartCoroutine(IdleTasks());
        if (characterStats.currentTask == CharacterTasks.CurrentTask.GoingForAWalk) GoWalking();
        if (characterStats.currentTask == CharacterTasks.CurrentTask.Drinking) GoDrinking();
    }

    // Moves a character if their task is to go for a walk
    void GoWalking()
    {
        if (!isDoingHobby)
        {
            WorldGrid grid = GameObject.Find("Plane").GetComponent<WorldGrid>();
            Vector2 walkTo = new Vector2(Random.Range(Mathf.RoundToInt(transform.position.x) - 3, Mathf.RoundToInt(transform.position.x) + 4),
                                            Random.Range(Mathf.RoundToInt(transform.position.z) - 3, Mathf.RoundToInt(transform.position.z) + 4));
            // Finds a spot to walk to that is outside and near the character
            if ((walkTo.x >= 0 && walkTo.x < grid.gridLocations.Length - 1) && (walkTo.y >= 0 && walkTo.y < grid.gridLocations.Length - 1))
            {
                if (grid.gridLocations[Mathf.RoundToInt(walkTo.x), Mathf.RoundToInt(walkTo.y)] == null)
                {
                    characterStats.nav.destination = new Vector3(walkTo.x, 0, walkTo.y);
                    isDoingHobby = true;
                }
            }
        }
        // Finds a new spot to walk to once reached
        else if (new Vector2(transform.position.x, transform.position.z) == new Vector2(characterStats.nav.destination.x, characterStats.nav.destination.z))
        {
            isDoingHobby = false;
        }
    }

    // Moves a character to the pub and picks actions if their task is to go drinking
    void GoDrinking()
    {
        GameObject findCounter = null;
        GameObject findPub = null;
        GameObject getAll = GameObject.Find("Plane").gameObject;

        // Searches all buildings for a pub
        for (int i = 0; i < getAll.transform.childCount; i++)
        {
            if (getAll.transform.GetChild(i).GetComponent<BuildingStats>() != null)
            {
                if (getAll.transform.GetChild(i).GetComponent<BuildingStats>().buildingType == BuildingParts.BuildingType.Pub)
                {
                    findPub = getAll.transform.GetChild(i).gameObject;
                    break;
                }
            }
        }

        // Find the counter inside the pub
        for (int i = 0; i < findPub.transform.childCount; i++)
        {
            if (findPub.transform.GetChild(i).GetComponent<BuildingPartsData>() != null)
            {
                BuildingPartsData obj = findPub.transform.GetChild(i).GetComponent<BuildingPartsData>();
                if (obj.furnitureType == BuildingParts.FurnitureType.ShopCounter)
                {
                    findCounter = obj.gameObject;
                    
                    break;
                }
            }
        }

        // Choses between finding a seat and buying a beer
        if (!isDoingHobby)
        {
            ItemPrices prices = GameObject.Find("Plane").GetComponent<ItemPrices>();
            int rand = Random.Range(0, 10);
            // Small chance character will buy a beer if they can afford it, the pub has beer left, and the counter is manned by the owner
            if (rand > 8 && characterStats.currentMoney >= prices.beerPrice && findPub.GetComponent<BuildingStats>().drinkStores[0] != BuildingParts.DrinkStores.None &&
                findCounter.GetComponent<BuildingPartsData>().activated)
            {
                shopCounter = new Vector2(findCounter.transform.position.x + findCounter.GetComponent<ObjectNavOffset>().customerNavOffset.x,
                                          findCounter.transform.position.z + findCounter.GetComponent<ObjectNavOffset>().customerNavOffset.y);
                characterStats.nav.destination = new Vector3(shopCounter.x, 0, shopCounter.y);
            }
            // Large chance character will find a seat in the pub that isn't already occupied
            else
            {
                findChair = 0;
                while (findPub.transform.GetChild(findChair).GetComponent<BuildingPartsData>().furnitureType != BuildingParts.FurnitureType.Chair &&
                       !findPub.transform.GetChild(findChair).GetComponent<BuildingPartsData>().inUse)
                {
                    findChair = Random.Range(0, findPub.transform.childCount);
                }
                characterStats.nav.destination = findPub.transform.GetChild(findChair).transform.position;
            }
            isDoingHobby = true;
        }

        // Look for a new seat if the chair they are heading for was taken
        if (characterStats.nav.destination.x == findPub.transform.GetChild(findChair).transform.position.x &&
            characterStats.nav.destination.z == findPub.transform.GetChild(findChair).transform.position.z &&
            findPub.transform.GetChild(findChair).GetComponent<BuildingPartsData>().inUse &&
            (characterStats.nav.destination.x > transform.position.x + .8f || characterStats.nav.destination.x < transform.position.x - .8f || 
            characterStats.nav.destination.z > transform.position.z + .8f || characterStats.nav.destination.z < transform.position.z - .8f))
        {
            isDoingHobby = false;
        }

        // Buys a beer once the character makes it to the counter
        if (shopCounter != new Vector2(0, 0) && isDoingHobby)
        {
            if (new Vector2(transform.position.x, transform.position.z) == new Vector2(shopCounter.x, shopCounter.y))
            {
                BuyDrink();
                shopCounter = new Vector2(0, 0);
                Debug.Log(gameObject.name + " bought beer");
            }
        }

    }

    // Buying a beer from the pub
    void BuyDrink()
    {
        // Character pays the price of beer, owner gets the money, drink stores go down and character hunger increases slightly
        ItemPrices prices = GameObject.Find("Plane").GetComponent<ItemPrices>();
        characterStats.currentMoney -= prices.beerPrice;
        buildingInside.GetComponent<BuildingStats>().ownedBy.GetComponent<CharacterStats>().currentMoney += prices.beerPrice;
        buildingInside.GetComponent<BuildingStats>().drinkStores[0] = BuildingParts.DrinkStores.None;
        characterStats.currentHunger += 10;
        isDoingHobby = false;
    }

    // Check if the character should be sleeping
    void CheckSleep()
    {
        sunsetTime = time.sunsetHour;
        sunriseTime = time.sunriseHour;

        // Finds a bed in their house that isn't owned by another character
        if (hasHome && bedOwned == null)
        {
            for (int i = 0; i < buildingOwned.transform.childCount; i++)
            {
                if (buildingOwned.transform.GetChild(i).GetComponent<BuildingPartsData>().furnitureType == BuildingParts.FurnitureType.Bed &&
                    buildingOwned.transform.GetChild(i).GetComponent<BuildingPartsData>().ownedBy == null)
                {
                    bedOwned = buildingOwned.transform.GetChild(i).gameObject;
                    buildingOwned.transform.GetChild(i).GetComponent<BuildingPartsData>().ownedBy = gameObject;
                    break;
                }
            }
        }

        // Chooses a random time to go to bed after sunset
        if (sleepHour == 0)
        {
            sleepHour = sunsetTime + Random.Range(0, 3);
            sleepMinute = Random.Range(0, 60);
        }

        // Goes to bed if after their sleep time and before their wake time, and character is not starving
        if (((time.currentHour == sleepHour && time.currentMinute >= sleepMinute) || time.currentHour > sleepHour || time.currentHour < wakeUpHour) && bedOwned != null
            && !isStarving)
            characterStats.currentTask = CharacterTasks.CurrentTask.Sleeping;

        // Rotates the character to the correct orientation of their bed when sleeping
        if (characterStats.currentTask == CharacterTasks.CurrentTask.Sleeping)
        {
            isBusy = true;
            characterStats.nav.destination = bedOwned.transform.position;

            if (new Vector3(transform.position.x, 0, transform.position.z) == new Vector3(bedOwned.transform.position.x, 0, bedOwned.transform.position.z))
            {
                if (bedOwned.GetComponent<BuildingPartsData>().facingDirection == BuildingParts.FacingDirection.Left)
                    transform.GetChild(0).rotation = Quaternion.Euler(90, 90, 0);
                if (bedOwned.GetComponent<BuildingPartsData>().facingDirection == BuildingParts.FacingDirection.Right)
                    transform.GetChild(0).rotation = Quaternion.Euler(90, -90, 0);
            }

            // Wakes up the character at their wake time
            if (time.currentHour >= wakeUpHour && time.currentHour < sleepHour)
            {
                isBusy = false;
                sleepHour = 0;
            }
        }

        // Resets the character's orientation if rotated for sleeping
        if (characterStats.currentTask != CharacterTasks.CurrentTask.Sleeping)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(90, 0, 0);
        }

    }

    // Check if the character needs to eat
    void CheckFood()
    {
        if (hasHome && buildingOwned.GetComponent<BuildingStats>().foodStores[0] != BuildingParts.FoodStores.None)
        {
            // Character will eat if hungry and doing nothing, and forces the character to go to eat if they are starving
            if ((characterStats.currentHunger <= 50 && isDoingHobby) ||
                 (characterStats.currentHunger <= 30 && characterStats.currentTask != CharacterTasks.CurrentTask.Eating))
            {
                if (characterStats.currentHunger <= 30) isStarving = true;

                // Looks for food storage in the building that is not already in use
                for (int i = 0; i < buildingOwned.GetComponent<BuildingStats>().buildingArea.Length; i++)
                {
                    if (buildingOwned.GetComponent<BuildingStats>().furnitureType[i] == BuildingParts.FurnitureType.FoodStorage)
                    {
                        if (!buildingOwned.transform.GetChild(i).GetComponent<BuildingPartsData>().inUse)
                        {
                            characterStats.nav.destination = new Vector3(buildingOwned.transform.GetChild(i).transform.position.x +
                                                                         buildingOwned.transform.GetChild(i).GetComponent<ObjectNavOffset>().customerNavOffset.x,
                                                                         0,
                                                                         buildingOwned.transform.GetChild(i).transform.position.z +
                                                                         buildingOwned.transform.GetChild(i).GetComponent<ObjectNavOffset>().customerNavOffset.y);
                            foodFound = buildingOwned.GetComponent<BuildingStats>().foodStores[0];
                            isBusy = true;
                            characterStats.currentTask = CharacterTasks.CurrentTask.Eating;
                            buildingOwned.transform.GetChild(i).GetComponent<BuildingPartsData>().inUse = true;
                            foodStore = buildingOwned.transform.GetChild(i).gameObject;
                            foundFood = true;
                            break;
                        }
                    }
                }
            }

            // Starts eating food once the character reaches the food storage
            if (foundFood)
            {
                if (new Vector3(transform.position.x, 0, transform.position.z) == 
                    new Vector3(characterStats.nav.destination.x, 0, characterStats.nav.destination.z))
                {
                    StartCoroutine(EatFood(foodFound, foodStore));
                    foundFood = false;
                }
            }
        }
    }

    // Eats the found food and adds to character hunger
    IEnumerator EatFood(BuildingParts.FoodStores food, GameObject foodStore)
    {
        yield return new WaitForSeconds(2);
        switch (food)
        {
            case BuildingParts.FoodStores.Bread:
                {
                    characterStats.currentHunger += 30;
                }
                break;

            case BuildingParts.FoodStores.None:
                {
                    characterStats.currentHunger = 100;
                    Debug.LogError("Food eaten by " + gameObject + " was " + food);
                }
                break;
        }
        characterStats.currentTask = characterStats.idleTask;
        foodStore.GetComponent<BuildingPartsData>().inUse = false;
        foodFound = BuildingParts.FoodStores.None;
        buildingOwned.GetComponent<BuildingStats>().foodStores[0] = BuildingParts.FoodStores.None;
        isBusy = false;
        isStarving = false;
    }

    // Makes the character look for a job
    void FindWork()
    {
        if (buildingOwned.GetComponent<BuildingStats>().buildingType != BuildingParts.BuildingType.Housing)
        {
            characterStats.workPlace = buildingOwned;
            if (buildingOwned.GetComponent<BuildingStats>().buildingType != BuildingParts.BuildingType.Workplace)
                    characterStats.currentJob = CharacterTasks.CurrentJob.ShopKeeper;
        }
    }

    // Sets up the working hours for different jobs
    void WorkingHours()
    {
        if (characterStats.currentJob == CharacterTasks.CurrentJob.ShopKeeper)
        {
            characterStats.workingStartTime = 6;
            characterStats.workingFinishTime = 23;
        }

        if (characterStats.currentJob != CharacterTasks.CurrentJob.None)
        {
            sleepHour = characterStats.workingFinishTime + 1;
            wakeUpHour = characterStats.workingStartTime - 1;
        }

        else
        {
            wakeUpHour = time.sunriseHour;
        }

    }

    // Gives the character tasks to do based on their job type
    void JobTasks()
    {
        if (time.currentHour >= characterStats.workingStartTime && time.currentHour < characterStats.workingFinishTime && !isStarving)
        {
            characterStats.currentTask = CharacterTasks.CurrentTask.Working;
            switch (characterStats.currentJob)
            {
                case CharacterTasks.CurrentJob.ShopKeeper:
                    {
                        WorkCounter();
                    }
                    break;
            }
            isBusy = true;
        }
        if (time.currentHour >= characterStats.workingFinishTime && characterStats.currentTask == CharacterTasks.CurrentTask.Working)
        {
            isBusy = false;
        }
    }

    // For characters that have the shop keeper job
    void WorkCounter()
    {
        GameObject workplace = characterStats.workPlace;
        GameObject counter = null;
        for (int i = 0; i < workplace.transform.childCount; i++)
        {
            if (workplace.transform.GetChild(i).GetComponent<BuildingPartsData>().furnitureType == BuildingParts.FurnitureType.ShopCounter)
            {
                counter = workplace.transform.GetChild(i).gameObject;
                Vector3 goTo = counter.transform.position;
                Vector2 offSet = counter.GetComponent<ObjectNavOffset>().ownerNavOffset;
                characterStats.nav.destination = new Vector3(goTo.x + offSet.x, 0, goTo.z + offSet.y);
                break;
            }
        }
    }
}
