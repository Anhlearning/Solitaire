using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    private Solitaire solitaire;
    public GameObject slot1;
    private void Awake()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                if (hit.collider.CompareTag("Deck"))
                {
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject); 
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    Bottom();   
                }
                else if(hit.collider.CompareTag("Top"))
                {
                    Top();  
                }
            }
        }
    }
    private void Deck()
    {
        solitaire.DeadlFromDeck();
        Debug.LogWarning("Deck");
    }
    private void Top()
    {
        Debug.LogWarning("Top");
    }
    private void Bottom() {
        Debug.LogWarning("Bottom");
    }
    private void Card(GameObject gameObjectSelected)
    {
        Debug.LogWarning("Card");

        // neu card dang nam up va khong bi khoa thi lat no len 
        if (!gameObjectSelected.GetComponent<Selectable>().cardFace)
        {
            if (!Blocked(gameObjectSelected))
            {
                gameObjectSelected.GetComponent<Selectable>().cardFace = true;
                slot1 = this.gameObject;
                return;
            }
        }
        else if (gameObjectSelected.GetComponent<Selectable>().inDeckPile)
        {
            if (!Blocked(gameObjectSelected))
            {
                slot1 = gameObjectSelected;
            }
        }
        if(slot1 == this.gameObject)
        {
            slot1= gameObjectSelected;  
        }
        else if(slot1 != gameObjectSelected)
        {
            if (Stackable(gameObjectSelected))
            {
                Stack(gameObjectSelected);
            }
            else
            {
                slot1= gameObjectSelected;  
            }
        }
        // neu card nam trong deck pile with the trip thi chon no 
        
        // neu card face up va truoc do khong chon thi chon card

        //neu ma double click va phu hop voi card tren stack thi se duoc stack it 
    }
    private bool Stackable(GameObject Selected)
    {
        Selectable s1=slot1.GetComponent<Selectable>(); 
        Selectable s2=Selected.GetComponent<Selectable>();
        if (s2.inDeckPile) return false;
        if (s2.isTop)
        {
            if (s1.suit == s2.suit || s1.value == 1 && s2.suit == null)
            {
                if(s1.value == s2.value + 1)
                {
                    return true;
                }
            }
            else {
                return false;
            }
        }
        else
        {
            if(s1.value == s2.value - 1)
            {
                bool redcardS1 = true;
                bool redcardS2 = true;  

                if(s1.suit=="C" || s1.suit == "S")
                {
                    redcardS1 = false;
                }
                if(s2.suit == "C"|| s2.suit == "S")
                {
                    redcardS2 = false;
                }

                if(redcardS1 == redcardS2)
                {
                    Debug.Log("NOT Stackable");
                    return false;
                }
                else
                {
                    Debug.Log("Stackable");
                    return true;
                }
            }
        }
        return false;
    }
    private bool Blocked(GameObject selected)
    {
        Selectable s2=selected.GetComponent<Selectable>();
        if(s2.inDeckPile== true)
        {
            if (s2.name == solitaire.tripOnDisplay.Last<string>())
            {
                Debug.Log("TRIPS NO BLOCK");
                return false;
            }
            else
            {
                Debug.Log("BLOCKED");
                return true;
            }
        }
        else
        {
            if (s2.name == solitaire.bottoms[s2.row].Last())
            {
                Debug.Log("BOTTOM NO BLOCK");
                return false;
            }
            else {
                Debug.Log("BLOCKED SOLITAIRE BOTTOMs");
                return true;
            
            }
        }
    }
    private void Stack(GameObject selected)
    {
        Selectable s1=slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();

        float yOffset = 0.1f;

        if(s2.isTop || (!s2.isTop && s1.value == 13))
        {
            yOffset = 0f;
        }
        slot1.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.03f);
        slot1.transform.parent=selected.transform;

        if (s1.inDeckPile)
        {
            solitaire.tripOnDisplay.Remove(slot1.name);
        }
        else if(s1.isTop && s2.isTop && s1.value == 1)
        {
            solitaire.TopPos[s1.row].GetComponent<Selectable>().value = 0;
            solitaire.TopPos[s1.row].GetComponent<Selectable>().suit = null;
        }
        else if (s1.isTop)
        {
            solitaire.TopPos[s1.row].GetComponent <Selectable>().value = s1.value-1;
        }
        else
        {
            solitaire.bottoms[s1.row].Remove(slot1.name);
        }
        s1.inDeckPile = false;
        s1.row = s2.row;

        if (s2.isTop)
        {
            solitaire.TopPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaire.TopPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.isTop = true;
        }
        else
        {
            s1.isTop = false;
        }
        slot1 = this.gameObject;
    }
}
