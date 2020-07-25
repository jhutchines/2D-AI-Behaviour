using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTasks : MonoBehaviour
{
    public enum CurrentTask
    {
        Nothing,
        Idling,
        MovingToLocation,
        Sleeping,
        Eating,
        Drinking,
        GoingForAWalk,
        Working
    }

    public enum CurrentJob
    {
        None,
        ShopKeeper
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
