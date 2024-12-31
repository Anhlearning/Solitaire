using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using System.Xml.Linq;

public class UserInput : MonoBehaviour
{
    [SerializeField] private ParticleSystem winEffect;
    public GameObject slot1;
    private Solitaire solitaire;
    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount = 0;
    private UndoManager undoManager;

    public static UserInput Instance {private set; get; } 

    private void Awake()
    {
        Instance = this;
        undoManager = GetComponent<UndoManager>();  
    }
    void Start()
    {
        solitaire = FindObjectOfType<Solitaire>();
        slot1 = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (clickCount == 1)
        {
            timer += Time.deltaTime;
        }
        if (clickCount == 3)
        {
            timer = 0;
            clickCount = 1;
        }
        if (timer > doubleClickTime)
        {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                // what has been hit? Deck/Card/EmptySlot...
                if (hit.collider.CompareTag("Deck"))
                {
                    //clicked deck
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    Card(hit.collider.gameObject);
                    CheckAutoWin();
                }
                else if (hit.collider.CompareTag("Top"))
                {
                    // clicked top
                    Top(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    // clicked bottom
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    void Deck()
    {
        solitaire.DealFromDeck();
        slot1 = this.gameObject;

    }
    void Card(GameObject selected)
    {
        if (!selected.GetComponent<Selectable>().cardFace) // if the card clicked on is facedown
        {
            if (!Blocked(selected)) // if the card clicked on is not blocked
            {
                // flip it over
                undoManager.ExecuteCommand(new ClickCardCommand(selected.GetComponent<Selectable>(),solitaire));
                Debug.LogWarning(solitaire.CountCardFace);
                slot1 = this.gameObject;    
            }
        }
        else if (selected.GetComponent<Selectable>().inDeckPile) // if the card clicked on is in the deck pile with the trips
        {
            // if it is not blocked
            if (!Blocked(selected))
            {
                if (slot1 == selected) // if the same card is clicked twice
                {
                    if (DoubleClick())
                    {
                        // attempt auto stack
                        AutoStack(selected);
                    }
                }
                else
                {
                    slot1 = selected;
                }
            }
            else
            {
                slot1=this.gameObject;
            }

        }
        else
        {
            if (slot1 == this.gameObject) // not null because we pass in this gameObject instead
            {
                slot1 = selected;
            }

            // if there is already a card selected (and it is not the same card)
            else if (slot1 != selected)
            {
                // if the new card is eligable to stack on the old card
                if (Stackable(selected) && HasNoChildren(selected))
                {
                    Stack(selected);
                }
                else
                {
                    // select the new card
                    slot1 = this.gameObject;
                }
            }

            else if (slot1 == selected) // if the same card is clicked twice
            {
                if (DoubleClick())
                {
                    AutoStack(selected);
                }
            }
        }
    }
    void Top(GameObject selected)
    {
      
        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 1 && selected.GetComponent<Selectable>().suit==slot1.GetComponent<Selectable>().suit)
            {
                Stack(selected);
            }

        }


    }
    void Bottom(GameObject selected)
    {
        if (slot1.CompareTag("Card"))
        {
            if (slot1.GetComponent<Selectable>().value == 13)
            {
                Stack(selected);
            }
        }
    }

    bool Stackable(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
       
        if (!s2.inDeckPile)
        {
            if (s2.isTop) // if in the top pile must stack suited Ace to King
            {
                if (s1.suit == s2.suit || (s1.value == 1 && s2.suit == null))
                {
                    if (s1.value == s2.value + 1)
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else  // if in the bottom pile must stack alternate colours King to Ace
            {
                if (s1.value == s2.value - 1)
                {
                    bool card1Red = true;
                    bool card2Red = true;

                    if (s1.suit == "C" || s1.suit == "S")
                    {
                        card1Red = false;
                    }

                    if (s2.suit == "C" || s2.suit == "S")
                    {
                        card2Red = false;
                    }

                    if (card1Red == card2Red)
                    {
                        print("Not stackable");
                        return false;
                    }
                    else
                    {
                        print("Stackable");
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void Stack(GameObject selected)
    {
        Selectable s1 = slot1.GetComponent<Selectable>();
        Selectable s2 = selected.GetComponent<Selectable>();
        float yOffset = 0.25f;

        if (s2.isTop || (!s2.isTop  && s1.value == 13))
        {
            yOffset = 0;
        }
        Vector3 oldPos = s1.transform.position;
        Vector3 newPos = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
        string nameParent = s1.transform.parent.name;
        Transform oldParent = GameObject.Find(nameParent).transform;
        undoManager.ExecuteCommand(new MoveCardCommand(oldPos, newPos, s1, s2, oldParent,solitaire));
        slot1 = this.gameObject;
    }
    public void CheckAutoWin()
    {
        if (solitaire.CountCardFace == 21 && solitaire.tripsOnDisplay.Count == 0 && solitaire.deck.Count == 0)
        {
            Debug.LogError(solitaire.CountCardFace);
            bool x = !ManagerPoint.Instance.HasWon();
            int count = 0;
            while (count <=10)
            { 
                count++;    
                Debug.Log("Check");
                for (int i = 0; i < solitaire.bottomPos.Length; i++)
                {
                    Debug.Log(solitaire.bottomPos[i].name);
                    if (solitaire.bottomPos[i].transform.childCount >= 2)
                    {
                        Transform latsChild = FindDeepestLastChild(solitaire.bottomPos[i].transform, "Card");
                        Debug.Log(latsChild.name);  
                        if (latsChild != null)
                        {
                            Selectable stack = latsChild.GetComponent<Selectable>();
                            if (stack != null && stack.cardFace == true)
                            {
                                slot1 = stack.gameObject;
                                StartCoroutine(DelayCouroutineAutoStack(stack.gameObject));
                            }
                        }
                    }
                }
            }
            winEffect.gameObject.SetActive(true);
            ManagerPoint.Instance.Win();
        }
    }
    IEnumerator DelayCouroutineAutoStack(GameObject stack)
    {
        AutoStackTop(stack.gameObject);
        yield return new WaitForSeconds(1f);
    }
    bool Blocked(GameObject selected)
    {
        Selectable s2 = selected.GetComponent<Selectable>();
        if (s2.inDeckPile == true)
        {
            if (s2.name == solitaire.tripsOnDisplay.Last()) // if it is the last trip it is not blocked
            {
                return false;
            }
            else
            {
                print(s2.name + " is blocked by " + solitaire.tripsOnDisplay.Last());
                return true;
            }
        }
        else
        {
            if (s2.name == solitaire.bottoms[s2.row].Last()) // check if it is the bottom card
            {
                return false;
            }
            else
            {
                print(s2.name + " is blocked by " + solitaire.bottoms[s2.row].Last());
                return true;
            }
        }
    }

    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2)
        {
            print("Double Click");
            return true;
        }
        else
        {
            return false;
        }
    }
    void AutoStackTop(GameObject selected)
    {
        for (int i = 0; i < solitaire.topPos.Length; i++)
        {
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            if (selected.GetComponent<Selectable>().value == 1) // if it is an Ace
            {
                if (stack.value == 0 && stack.suit == selected.GetComponent<Selectable>().suit) // and the top position is empty
                {
                    slot1 = selected;
                    Stack(stack.gameObject); // stack the ace up top
                    return;                 // in the first empty position found
                }
            }
            else
            {
                if ((solitaire.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitaire.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1))
                {
                    // if it is the last card (if it has no children)
                    Debug.Log("Move to but selected diff 1 ");
                    if (HasNoChildren(slot1))
                    {
                        slot1 = selected;
                        // find a top spot that matches the conditions for auto stacking if it exists
                        string lastCardname = stack.suit + stack.value.ToString();
                        if (stack.value == 1)
                        {
                            lastCardname = stack.suit + "A";
                        }
                        if (stack.value == 11)
                        {
                            lastCardname = stack.suit + "J";
                        }
                        if (stack.value == 12)
                        {
                            lastCardname = stack.suit + "Q";
                        }
                        if (stack.value == 13)
                        {
                            lastCardname = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(lastCardname);
                        Stack(lastCard);
                        return;
                    }
                }
            }
        }
    }
    void AutoStack(GameObject selected)
    {
        for (int i = 0; i < solitaire.topPos.Length; i++)
        {
            Selectable stack = solitaire.topPos[i].GetComponent<Selectable>();
            if (selected.GetComponent<Selectable>().value == 1) // if it is an Ace
            {
                if (stack.value == 0 && stack.suit==selected.GetComponent<Selectable>().suit) // and the top position is empty
                {
                    slot1 = selected;
                    Stack(stack.gameObject); // stack the ace up top
                    return;                 // in the first empty position found
                }
            }
            else
            {
                if ((solitaire.topPos[i].GetComponent<Selectable>().suit == slot1.GetComponent<Selectable>().suit) && (solitaire.topPos[i].GetComponent<Selectable>().value == slot1.GetComponent<Selectable>().value - 1))
                {
                    // if it is the last card (if it has no children)
                    Debug.Log("Move to but selected diff 1 ");
                    if (HasNoChildren(slot1))
                    {
                        slot1 = selected;
                        // find a top spot that matches the conditions for auto stacking if it exists
                        string lastCardname = stack.suit + stack.value.ToString();
                        if (stack.value == 1)
                        {
                            lastCardname = stack.suit + "A";
                        }
                        if (stack.value == 11)
                        {
                            lastCardname = stack.suit + "J";
                        }
                        if (stack.value == 12)
                        {
                            lastCardname = stack.suit + "Q";
                        }
                        if (stack.value == 13)
                        {
                            lastCardname = stack.suit + "K";
                        }
                        GameObject lastCard = GameObject.Find(lastCardname);
                        Stack(lastCard);
                        return;
                    }
                }
            }
        }
        for(int i = 0; i < solitaire.bottomPos.Length; i++)
        {
            if (solitaire.bottomPos[i].transform.childCount == 1 && selected.GetComponent<Selectable>().value == 13)
            {
                slot1 = selected;
                Stack(solitaire.bottomPos[i]);
                return;
            }
            else
            {
                Transform latsChild = FindDeepestLastChild(solitaire.bottomPos[i].transform, "Card");
                if (latsChild != null)
                {
                    Selectable stack = latsChild.GetComponent<Selectable>();
                    if (stack != null)
                    {
                        if (Stackable(stack.gameObject) && stack.cardFace ==true)
                        {
                            slot1 = selected;
                            Debug.Log(stack.name);
                            Stack(stack.gameObject);
                            return;
                        }
                    }
                }
            }
            
            
        }   
    }
    Transform FindDeepestLastChild(Transform parent, string tagName)
    {   if (parent.childCount < 2) return null;

        if (parent.childCount == 2 && parent.CompareTag("Card")){
            return parent;
        }

        Transform lastChild = parent.GetChild(parent.childCount - 1);

        return FindDeepestLastChild(lastChild,tagName);
    }
    bool HasNoChildren(GameObject card)
    {
        int i = 0;
        foreach (Transform child in card.transform)
        {
            i++;
        }
        if (i == 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
