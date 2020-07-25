using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterStats : MonoBehaviour
{
    public CharacterTasks.CurrentTask currentTask;
    public CharacterTasks.CurrentJob currentJob;
    [HideInInspector] public CharacterTasks.CurrentTask idleTask = CharacterTasks.CurrentTask.Idling;
    public NavMeshAgent nav;
    public string characterName;
    public string displayCurrentTask;
    public int currentMoney;
    public float currentHunger = 100;
    public NewCharacters hobbies;
    public CharacterTasks.CurrentTask[] characterHobbies;
    public GameObject workPlace;
    public float workingStartTime;
    public float workingFinishTime;

    DayNightCycle time;

    // Start is called before the first frame update
    void Start()
    {
        hobbies = GameObject.Find("Plane").GetComponent<NewCharacters>();
        nav = GetComponent<NavMeshAgent>();
        time = GameObject.Find("Time").GetComponent<DayNightCycle>();
        characterHobbies = new CharacterTasks.CurrentTask[Random.Range(1, hobbies.hobbies.Length)];

        CharacterTasks.CurrentTask[] selectedHobbies;
        selectedHobbies = new CharacterTasks.CurrentTask[characterHobbies.Length];
        for (int i = 0; i < characterHobbies.Length; i++)
        {
            int pickRandom = Random.Range(1, hobbies.hobbies.Length);
            bool alreadyPicked = false;
            {
                for (int j = 0; j < selectedHobbies.Length; j++)
                {
                    if (selectedHobbies[j] == hobbies.hobbies[pickRandom])
                    {
                        alreadyPicked = true;
                        break;
                    }
                }
                if (alreadyPicked) i--;
                else
                {
                    characterHobbies[i] = hobbies.hobbies[pickRandom];
                    selectedHobbies[i] = characterHobbies[i];
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckTask();
        HungerUpdate();
    }

    void CheckTask()
    {
        //if (nav.destination != transform.position)
        //{
        //    currentTask = CharacterTasks.CurrentTask.MovingToLocation;
        //}
        //else
        //{
        //    currentTask = CharacterTasks.CurrentTask.Nothing;
        //}

        string checkTask = currentTask.ToString();
        for (int i = 0; i < checkTask.Length; i++)
        {
            if (char.IsUpper(checkTask[i]))
            {
                if (i == 0) continue;
                checkTask = checkTask.Insert(i, " ");
                i++;
            }
        }
        displayCurrentTask = checkTask;
    }

    void HungerUpdate()
    {
        currentHunger -= 0.2f * Time.deltaTime * time.currentTimeSpeed;
        currentHunger = Mathf.Clamp(currentHunger, 0, 100);
    }
}
