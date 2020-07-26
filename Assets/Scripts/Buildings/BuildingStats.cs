using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingStats : MonoBehaviour
{
    public BuildingParts.BuildingType buildingType;
    public GameObject ownedBy;
    public GameObject[] workers;
    public BuildingParts.BuildingPartType[] buildingArea;
    public BuildingParts.FurnitureType[] furnitureType;
    public Vector2[] buildingAreaLocation;
    public BuildingParts.FoodStores[] foodStores;
    public BuildingParts.DrinkStores[] drinkStores;
    public int salePrice;
    public bool isOpen;


    // Start is called before the first frame update
    void Start()
    {
        InitialBuildingCheck();
        if (buildingType != BuildingParts.BuildingType.Housing) workers = new GameObject[10];
    }

    // Finds all objects in the building and sets correct stats where needed
    void InitialBuildingCheck()
    {
        buildingArea = new BuildingParts.BuildingPartType[transform.childCount];
        buildingAreaLocation = new Vector2[transform.childCount];
        furnitureType = new BuildingParts.FurnitureType[transform.childCount];
        foodStores = new BuildingParts.FoodStores[6];
        // If the building is a pub, generate random initial stockpile of drinks
        if (buildingType == BuildingParts.BuildingType.Pub)
        {
            drinkStores = new BuildingParts.DrinkStores[12];
            int drinkAmount = Random.Range(5, drinkStores.Length);
            for (int i = 0; i < drinkAmount; i++)
            {
                drinkStores[i] = BuildingParts.DrinkStores.Beer;
            }
        }

        // Find all objects that aren't flooring and get their part type, location, and furniture type (if any)
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<BuildingPartsData>().partType != BuildingParts.BuildingPartType.Floor)
            {
                buildingArea[i] = transform.GetChild(i).GetComponent<BuildingPartsData>().partType;
                buildingAreaLocation[i] = new Vector2(transform.GetChild(i).transform.position.x, transform.GetChild(i).transform.position.z);
                furnitureType[i] = transform.GetChild(i).GetComponent<BuildingPartsData>().furnitureType;
            }
        }

        // Finds all floor objects, and gets their location if no object is placed on top of it
        for (int i = 0; i < transform.childCount; i++)
        {
            bool foundDuplicate = false;
            if (transform.GetChild(i).GetComponent<BuildingPartsData>().partType == BuildingParts.BuildingPartType.Floor)
            {
                for (int j = 0; j < buildingAreaLocation.Length; j++)
                {
                    if (buildingAreaLocation[j] == new Vector2(transform.GetChild(i).transform.position.x, transform.GetChild(i).transform.position.z))
                    {
                        foundDuplicate = true;
                        break;
                    }
                }

                if (foundDuplicate) continue;

                buildingArea[i] = transform.GetChild(i).GetComponent<BuildingPartsData>().partType;
                buildingAreaLocation[i] = new Vector2(transform.GetChild(i).transform.position.x, transform.GetChild(i).transform.position.z);
            }
        }

        // Generates random starting food for all buildings
        int foodAmount = Random.Range(1, foodStores.Length);
        for (int i = 0; i < foodAmount; i++)
        {
            foodStores[i] = BuildingParts.FoodStores.Bread;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        if (foodStores.Length > 0) UpdateFoodStores();
        if (drinkStores.Length > 0) UpdateDrinkStores();
    }

    // Updates food to make sure food goes to the top of the storage array
    void UpdateFoodStores()
    {
        for (int i = 1; i < foodStores.Length; i++)
        {
            if (foodStores[i] != BuildingParts.FoodStores.None && foodStores[i - 1] == BuildingParts.FoodStores.None)
            {
                foodStores[i - 1] = foodStores[i];
                foodStores[i] = BuildingParts.FoodStores.None;
            }
        }
    }

    // Updates drink to make sure drinks go to the top of the storage array
    void UpdateDrinkStores()
    {
        for (int i = 1; i < drinkStores.Length; i++)
        {
            if (drinkStores[i] != BuildingParts.DrinkStores.None && drinkStores[i - 1] == BuildingParts.DrinkStores.None)
            {
                drinkStores[i - 1] = drinkStores[i];
                drinkStores[i] = BuildingParts.DrinkStores.None;
            }
        }
    }
}
