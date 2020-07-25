using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCharacters : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] heads;
    public Sprite[] bodies;

    [Header("Names")]
    public string[] firstNames;
    public string[] lastNames;

    [Header("Spawn Options")]
    public GameObject newCharacter;
    public Vector3 spawnLocation;
    public int spawnAmount;

    public CharacterTasks.CurrentTask[] hobbies;

    // Start is called before the first frame update
    void Start()
    {
        SpawnCharacters();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R)) SpawnCharacters();
    }

    void SpawnCharacters()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            GameObject newSpawn = Instantiate(newCharacter, spawnLocation, transform.rotation);
            newSpawn.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = bodies[Random.Range(0, bodies.Length)];
            newSpawn.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite = heads[Random.Range(0, heads.Length)];
            newSpawn.GetComponent<CharacterStats>().currentMoney = Random.Range(250, 700);
            newSpawn.GetComponent<CharacterStats>().currentHunger = Random.Range(60f, 100f);
            newSpawn.GetComponent<CharacterStats>().characterName = firstNames[Random.Range(0, firstNames.Length)] + " " + lastNames[Random.Range(0, lastNames.Length)];
        }
    }
}
