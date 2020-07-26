using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterInUse : MonoBehaviour
{
    BuildingPartsData partsData;
    // Start is called before the first frame update
    void Start()
    {
        partsData = GetComponent<BuildingPartsData>();
        Vector2 offset = GetComponent<ObjectNavOffset>().ownerNavOffset;
        GetComponent<BoxCollider>().center = new Vector3(offset.x, 0, offset.y);
    }

    // Sets the counter to "In Use" by the shopkeeper
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<CharacterStats>() != null)
        {
            if (other.GetComponent<CharacterStats>().currentJob == CharacterTasks.CurrentJob.ShopKeeper)
            {
                partsData.activated = true;
            }
        }
    }

    // Counter is no longer in use if the shopkeeper leaves
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<CharacterStats>() != null)
        {
            partsData.activated = false;
        }
    }
}
