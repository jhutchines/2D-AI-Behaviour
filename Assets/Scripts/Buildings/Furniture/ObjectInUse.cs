using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInUse : MonoBehaviour
{
    BuildingPartsData partsData;
    // Start is called before the first frame update
    void Start()
    {
        partsData = GetComponent<BuildingPartsData>();
    }

    // Sets the object to "In Use" if any character is on there
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CharacterStats>() != null)
        {
            partsData.inUse = true;
        }
    }

    // Sets the object to no longer "In Use" if the character leaves
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterStats>() != null)
        {
            partsData.inUse = false;
        }
    }
}
