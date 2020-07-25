using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingParts : MonoBehaviour
{
    public enum BuildingType
    {
        Housing,
        Shop,
        Workplace,
        Pub
    }

    public enum BuildingPartType
    {
        None,
        Wall,
        Door,
        Furniture,
        Floor,
        Road
    }

    public enum FurnitureType
    {
        None,
        Chair,
        Table,
        Bed,
        FoodStorage,
        BeerStorage,
        ShopCounter
    }

    public enum FacingDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    public enum FoodStores
    {
        None,
        Bread
    }

    public enum DrinkStores
    {
        None,
        Beer
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
