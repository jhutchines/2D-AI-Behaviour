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

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CharacterStats>() != null)
        {
            partsData.inUse = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterStats>() != null)
        {
            partsData.inUse = false;
        }
    }
}
