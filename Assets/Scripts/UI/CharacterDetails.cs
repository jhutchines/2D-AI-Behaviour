using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetails : MonoBehaviour
{
    public Text characterName;
    public Text characterTask;
    public Text currentGold;
    public RectTransform hungerBar;
    public CharacterStats selectedCharacter;
    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        int layerMask = 1 << 8;
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 30, layerMask))
        {
            if (hit.transform.GetComponent<CharacterStats>() != null)
            {
                selectedCharacter = hit.transform.GetComponent<CharacterStats>();
            }
        }
        if (Input.GetMouseButtonDown(1)) selectedCharacter = null;

        UpdateDetails();
    }

    void UpdateDetails()
    {
        if (selectedCharacter == null) transform.GetChild(0).gameObject.SetActive(false);
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            characterName.text = selectedCharacter.characterName;
            characterTask.text = selectedCharacter.displayCurrentTask;
            currentGold.text = selectedCharacter.currentMoney + " gold";
            hungerBar.offsetMax = new Vector2((selectedCharacter.currentHunger * 2) + 25, -27.5f);
        }
    }
}
