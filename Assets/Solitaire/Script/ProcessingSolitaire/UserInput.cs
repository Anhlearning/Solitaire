using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;
using System.Xml.Linq;
using DG.Tweening;
using Unity.Burst.CompilerServices;
using Solitaire_Manager.Manager;
using Solitaire_Manager.UndoManager;
using Solitaire_Manager.PointManger;
using Solitaire_Manager.AudioManager;
using Solitaire_Card;

namespace Solitaire_Input.UserInput{
    public class Solitaire_UserInput : MonoBehaviour
    {
        [SerializeField] private ParticleSystem winEffect;
        public GameObject slot1;
        private Solitaire solitaire;
        private float timer;
        private Solitaire_UndoManager undoManager;
        private bool isDragging = false;
        private GameObject draggedCard = null;
        [SerializeField] private float durationMoveTopWin;
        [SerializeField] private float timeCheckNoDrag;
        private float timeCheckMax;
        string oldParentPos;

        public static Solitaire_UserInput Instance { private set; get; }
        Vector3 oldPos;
        private void Awake()
        {
            Instance = this;
            timeCheckMax = timeCheckNoDrag;
            undoManager = GetComponent<Solitaire_UndoManager>();
        }
        void Start()
        {
            solitaire = FindObjectOfType<Solitaire>();
            slot1 = this.gameObject;
        }

        // Update is called once per frame
        void Update()
        {
            if (Solitaire_ManagerPoint.Instance.HasWon())
            {
                return;
            }
            if (Input.GetMouseButtonUp(0) && timeCheckNoDrag > 0)
            {
                if (isDragging)
                {
                    timeCheckNoDrag = timeCheckMax;
                    Card(draggedCard.gameObject);
                    StartCoroutine(CheckAutoWin());

                }
                draggedCard = null;
                isDragging = false;
            }
            if (isDragging)
            {
                timeCheckNoDrag -= Time.deltaTime;
                HandleDragging();
                return;
            }
           
             GetMouseClick();

            
        }

        void HandleDragging()
        {

            if (timeCheckNoDrag > 0) return;
            if (!draggedCard.GetComponent<Solitaire_Selectable>().cardFace)
            {
                isDragging = false;
                draggedCard = null;
                timeCheckNoDrag = timeCheckMax;
                return;
            }
            if (isDragging && draggedCard != null)
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                draggedCard.transform.parent = null;
                draggedCard.transform.position = new Vector3(mousePosition.x, mousePosition.y, -5);
            }
            if (Input.GetMouseButtonUp(0))
            {

                draggedCard.GetComponent<Collider2D>().enabled = false;
                foreach (Transform child in draggedCard.transform)
                {
                    if (child.CompareTag("Card"))
                    {
                        child.GetComponent<Collider2D>().enabled = false;
                    }
                }
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                if (hit)
                {
                    Debug.Log(hit.collider.name + " " + draggedCard.name);
                    if (hit.collider.CompareTag("Top"))
                    {
                        bool tmp = Top(hit.collider.gameObject);
                        if (!tmp)
                        {
                            ResetPosObject();
                        }
                    }
                    else if (hit.collider.CompareTag("Card"))
                    {

                        if (Stackable(hit.collider.gameObject))
                        {
                            slot1 = draggedCard.gameObject;
                            Debug.Log(slot1.gameObject.name + " " + hit.collider.name);
                            Stack(hit.collider.gameObject);
                        }
                        else
                        {
                            ResetPosObject();
                        }
                    }
                    else if (hit.collider.CompareTag("Bottom"))
                    {
                        bool tmp = Bottom(hit.collider.gameObject);
                        if (!tmp)
                        {
                            ResetPosObject();
                        }
                    }
                }
                else
                {
                    ResetPosObject();
                }
                draggedCard.GetComponent<Collider2D>().enabled = true;
                foreach (Transform child in draggedCard.transform)
                {
                    if (child.CompareTag("Card"))
                    {
                        child.GetComponent<Collider2D>().enabled = true;
                    }
                }
                isDragging = false;
                draggedCard = null;
                timeCheckNoDrag = timeCheckMax;
            }
        }
        public void ResetPosObject()
        {
            Solitaire_Selectable seTmp = draggedCard.GetComponent<Solitaire_Selectable>();
            draggedCard.transform.position = oldPos;
            draggedCard.transform.parent = GameObject.Find(oldParentPos).transform;
        }
        void GetMouseClick()
        {
            if (!Solitaire_GameManager.Instance.IsPlaying())
            {
                return;
            }

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
                        if (hit.collider.GetComponent<Solitaire_Selectable>().inDeckPile) // if the card clicked on is in the deck pile with the trips
                        {
                            // if it is not blocked
                            if (Blocked(hit.collider.gameObject))
                            {
                                return;
                            }
                        }
                        slot1 = hit.collider.gameObject;
                        draggedCard = hit.collider.gameObject;
                        isDragging = true;
                        oldPos = hit.collider.transform.position;
                        oldParentPos = hit.transform.parent.name;
                    }
                    else if (hit.collider.CompareTag("Top"))
                    {
                        Top(hit.collider.gameObject);
                    }
                    else if (hit.collider.CompareTag("Bottom"))
                    {
                        // clicked bottom
                        Bottom(hit.collider.gameObject);
                    }
                }
                else
                {
                    slot1 = this.gameObject;
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
            if (!selected.GetComponent<Solitaire_Selectable>().cardFace) // if the card clicked on is facedown
            {
                if (!Blocked(selected)) // if the card clicked on is not blocked
                {
                    // flip it over
                    undoManager.ExecuteCommand(new Solitaire_ClickCardCommand(selected.GetComponent<Solitaire_Selectable>(), solitaire));
                    slot1 = this.gameObject;
                }
            }
            else if (selected.GetComponent<Solitaire_Selectable>().inDeckPile) // if the card clicked on is in the deck pile with the trips
            {
                // if it is not blocked
                if (!Blocked(selected))
                {
                    slot1 = selected;
                    AutoStack(selected);

                }
                else
                {
                    slot1 = this.gameObject;
                }

            }
            else
            {
                slot1 = selected;
                AutoStack(selected);
            }
        }
        bool Top(GameObject selected)
        {

            if (slot1.CompareTag("Card"))
            {
                if (slot1.GetComponent<Solitaire_Selectable>().value == 1 && selected.GetComponent<Solitaire_Selectable>().suit == slot1.GetComponent<Solitaire_Selectable>().suit)
                {
                    Stack(selected);
                    return true;
                }
            }
            return false;

        }
        bool Bottom(GameObject selected)
        {
            if (slot1.CompareTag("Card"))
            {
                if (slot1.GetComponent<Solitaire_Selectable>().value == 13)
                {
                    Debug.LogWarning("BOTTOMS");
                    Stack(selected);
                    return true;
                }
            }
            return false;
        }

        bool Stackable(GameObject selected)
        {
            Solitaire_Selectable s1 = slot1.GetComponent<Solitaire_Selectable>();
            Solitaire_Selectable s2 = selected.GetComponent<Solitaire_Selectable>();

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

        void Stack(GameObject selected, float durationMove = 0.2f)
        {
            Solitaire_Selectable s1 = slot1.GetComponent<Solitaire_Selectable>();
            Solitaire_Selectable s2 = selected.GetComponent<Solitaire_Selectable>();
            float yOffset = 0.25f;

            if (s2.isTop || (!s2.isTop && s1.value == 13))
            {
                yOffset = 0;
            }
            Vector3 tmpPos = oldPos;
            Vector3 newPos = new Vector3(selected.transform.position.x, selected.transform.position.y - yOffset, selected.transform.position.z - 0.01f);
            string nameParent = oldParentPos;
            Transform oldParent = GameObject.Find(nameParent).transform;
            undoManager.ExecuteCommand(new Solitaire_MoveCardCommand(tmpPos, newPos, s1, s2, oldParent, solitaire, durationMove));
            slot1 = this.gameObject;
        }
        public IEnumerator CheckAutoWin()
        {
            if (solitaire.CountCardFace == 21 && solitaire.tripsOnDisplay.Count == 0 && solitaire.deck.Count == 0)
            {
                Debug.LogError(solitaire.CountCardFace);
                Solitaire_ManagerPoint.Instance.Win();
                int count = 0;
                while (!Solitaire_ManagerPoint.Instance.HasWon())
                {
                    count++;
                    Debug.LogWarning("Check count is " + count);
                    for (int i = 0; i < solitaire.bottomPos.Length; i++)
                    {
                        if (solitaire.bottomPos[i].transform.childCount >= 2)
                        {
                            Transform latsChild = FindDeepestLastChild(solitaire.bottomPos[i].transform, "Card");
                            if (latsChild != null)
                            {

                                Solitaire_Selectable stack = latsChild.GetComponent<Solitaire_Selectable>();
                                if (stack != null && stack.cardFace == true)
                                {
                                    slot1 = stack.gameObject;
                                    AutoStackTop(stack.gameObject, durationMoveTopWin);
                                }
                            }
                        }
                        yield return new WaitForSeconds(durationMoveTopWin);
                    }
                }
                winEffect.gameObject.SetActive(true);
                Solitaire_AudioManager.Instance.PlayVictoria();
                DOVirtual.DelayedCall(1f, () =>
                {
                    Solitaire_GameManager.Instance.ChangedState(StateSolitaire.GameWin);
                });

            }
        }
        bool Blocked(GameObject selected)
        {
            Solitaire_Selectable s2 = selected.GetComponent<Solitaire_Selectable>();
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
        void AutoStackTop(GameObject selected, float durationMove = 0.2f)
        {
            for (int i = 0; i < solitaire.topPos.Length; i++)
            {
                Solitaire_Selectable stack = solitaire.topPos[i].GetComponent<Solitaire_Selectable>();
                if (selected.GetComponent<Solitaire_Selectable>().value == 1) // if it is an Ace
                {
                    if (stack.value == 0 && stack.suit == selected.GetComponent<Solitaire_Selectable>().suit) // and the top position is empty
                    {
                        slot1 = selected;
                        Debug.LogError(selected.name + " " + "Child tim duoc la :" + stack.gameObject.name);
                        Stack(stack.gameObject, durationMove);
                        return;
                    }
                }
                else
                {
                    if ((solitaire.topPos[i].GetComponent<Solitaire_Selectable>().suit == slot1.GetComponent<Solitaire_Selectable>().suit) && (solitaire.topPos[i].GetComponent<Solitaire_Selectable>().value == slot1.GetComponent<Solitaire_Selectable>().value - 1))
                    {
                        if (HasNoChildren(slot1))
                        {
                            slot1 = selected;

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
                            Debug.LogError(selected.name + " " + "Child tim duoc la :" + stack.gameObject.name + " " + lastCardname);
                            GameObject lastCard = GameObject.Find(lastCardname);
                            if (lastCard != null)
                            {
                                Stack(lastCard, durationMove);
                                return;
                            }
                        }
                    }
                }
            }
        }
        void AutoStack(GameObject selected)
        {
            Debug.LogError("Auto Stack Of" + selected.name);
            for (int i = 0; i < solitaire.topPos.Length; i++)
            {
                Solitaire_Selectable stack = solitaire.topPos[i].GetComponent<Solitaire_Selectable>();
                if (selected.GetComponent<Solitaire_Selectable>().value == 1) // if it is an Ace
                {
                    if (stack.value == 0 && stack.suit == selected.GetComponent<Solitaire_Selectable>().suit) // and the top position is empty
                    {
                        slot1 = selected;
                        Stack(stack.gameObject); // stack the ace up top
                        return;                 // in the first empty position found
                    }
                }
                else
                {
                    if ((solitaire.topPos[i].GetComponent<Solitaire_Selectable>().suit == slot1.GetComponent<Solitaire_Selectable>().suit) && (solitaire.topPos[i].GetComponent<Solitaire_Selectable>().value == slot1.GetComponent<Solitaire_Selectable>().value - 1))
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
            for (int i = 0; i < solitaire.bottomPos.Length; i++)
            {
                if (solitaire.bottomPos[i].transform.childCount == 1 && selected.GetComponent<Solitaire_Selectable>().value == 13)
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
                        Solitaire_Selectable stack = latsChild.GetComponent<Solitaire_Selectable>();
                        if (stack != null)
                        {
                            if (Stackable(stack.gameObject) && stack.cardFace == true)
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
        {
            if (parent.childCount < 2) return null;

            if (parent.childCount == 2 && parent.CompareTag("Card"))
            {
                return parent;
            }

            Transform lastChild = parent.GetChild(parent.childCount - 1);

            return FindDeepestLastChild(lastChild, tagName);
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
}

