using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPartsData : MonoBehaviour
{
    public BuildingParts.BuildingPartType partType;
    public BuildingParts.FurnitureType furnitureType;
    public BuildingParts.FacingDirection facingDirection;
    public GameObject ownedBy;
    public bool inUse;
    public bool activated;
    public bool isDoor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (isDoor)
        {
            transform.GetChild(0).rotation = Quaternion.Euler(0, 90, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isDoor) transform.GetChild(0).rotation = Quaternion.Euler(0, 0, 0);
    }
}
