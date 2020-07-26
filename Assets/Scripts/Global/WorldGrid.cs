using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGrid : MonoBehaviour
{
    public GameObject[,] gridLocations;

    // Start is called before the first frame update
    void Start()
    {
        gridLocations = new GameObject[Mathf.RoundToInt(transform.localScale.x * 10), Mathf.RoundToInt(transform.localScale.z * 10)];
        RefreshGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RefreshGrid()
    {
        // Finds all objects and saves their x and z locations for characters to reference
        var buildingObjects = FindObjectsOfType<BuildingPartsData>();
        for (int i = 0; i < buildingObjects.Length; i++)
        {
            gridLocations[Mathf.RoundToInt(buildingObjects[i].transform.position.x),
                          Mathf.RoundToInt(buildingObjects[i].transform.position.z)] = buildingObjects[i].gameObject;
        }
    }
}
