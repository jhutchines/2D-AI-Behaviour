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

    void InitialBuildingCheck()
    {
        buildingArea = new BuildingParts.BuildingPartType[transform.childCount];
        buildingAreaLocation = new Vector2[transform.childCount];
        furnitureType = new BuildingParts.FurnitureType[transform.childCount];
        foodStores = new BuildingParts.FoodStores[6];
        if (buildingType == BuildingParts.BuildingType.Pub)
        {
            drinkStores = new BuildingParts.DrinkStores[12];
            int drinkAmount = Random.Range(5, drinkStores.Length);
            for (int i = 0; i < drinkAmount; i++)
            {
                drinkStores[i] = BuildingParts.DrinkStores.Beer;
            }
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<BuildingPartsData>().partType != BuildingParts.BuildingPartType.Floor)
            {
                buildingArea[i] = transform.GetChild(i).GetComponent<BuildingPartsData>().partType;
                buildingAreaLocation[i] = new Vector2(transform.GetChild(i).transform.position.x, transform.GetChild(i).transform.position.z);
                furnitureType[i] = transform.GetChild(i).GetComponent<BuildingPartsData>().furnitureType;
            }
        }

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

        int foodAmount = Random.Range(1, foodStores.Length);
        for (int i = 0; i < foodAmount; i++)
        {
            foodStores[i] = BuildingParts.FoodStores.Bread;
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateFoodStores();
        if (foodStores.Length > 0) UpdateDrinkStores();
    }

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
