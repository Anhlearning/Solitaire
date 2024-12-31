using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class valueCard
{
    public Sprite number;
    public Sprite suitsmall;
    public Sprite suitCenter;
}
public enum Option
{
    easy=1,
    normal=2,
    hard=3
}
public class Solitaire : MonoBehaviour
{   
    [Header("Sprite")]
    public List<valueCard> cardSpriteList;
    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] values = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    [Header("Transform")]
    public GameObject pivotDeck;
    public GameObject cardPrefab;
    public GameObject deckButton;
    public GameObject[] bottomPos;
    public GameObject[] topPos;
    public Dictionary<string, GameObject> mapDeck=new Dictionary<string, GameObject>();
    public Transform deckPlaceHolder;
    public Transform deckRestart;
    [Header("Parameter Caculate")]
    public int CountCardFace=0;
    public float countDownClickDeal;
    public int deckLocation;
    public int trips;
    public int tripsRemainder;
    public float zOffset = -0.2f;
    public float distanceCard;
    [Header("Option")]
    public Option option;
    public List<string>[] bottoms;
    public List<string>[] tops;
    public List<string> tripsOnDisplay = new List<string>();
    public List<List<string>> deckTrips = new List<List<string>>();
    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();
    private UndoManager undoManager;
    private bool isClickedDeal = false;
    public List<string> deck;
    public List<string> discardPile = new List<string>();
   
    private void Awake()
    {
        undoManager = GetComponent<UndoManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        PlayCards();
    }

    public void PlayCards()
    {
        foreach (List<string> list in bottoms)
        {
            list.Clear();
        }

        deck = GenerateDeck();
        Shuffle(deck);

        //test the cards in the deck:
        foreach (string card in deck)
        {
            print(card);
        }
        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        SortDeckIntoTrips(option);

    }

    public void AutoMoveToTop()
    {
        if (CountCardFace == 28) //&& 1 cai gi do)
        {

        }
    }

    public static List<string> GenerateDeck()
    {
        List<string> newDeck = new List<string>();
        foreach (string s in values)
        {
            foreach (string v in suits)
            {
                newDeck.Add(v + s);
            }
        }
        return newDeck;
    }

    void Shuffle<T>(List<T> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            int k = random.Next(n);
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    IEnumerator SolitaireDeal()
    {
        deckButton.GetComponent<Collider2D>().enabled = false;  
        for (int i = 0; i < 7; i++)
        {

            float yOffset = 0;
            float zOffset = 0.03f;
            foreach (string card in bottoms[i])
            {
                yield return new WaitForSeconds(0.05f);
                GameObject newCard = Instantiate(cardPrefab, new Vector3(bottomPos[i].transform.position.x, bottomPos[i].transform.position.y - yOffset, bottomPos[i].transform.position.z - zOffset), Quaternion.identity, bottomPos[i].transform);
                newCard.name = card;
                newCard.GetComponent<Selectable>().row = i;
                if (card == bottoms[i][bottoms[i].Count - 1])
                {
                    newCard.GetComponent<Selectable>().cardFace = true;

                }
                yOffset = yOffset + 0.15f;
                zOffset = zOffset + 0.03f;
                discardPile.Add(card);
            }
        }

        foreach (string card in discardPile)
        {
            if (deck.Contains(card))
            {
                deck.Remove(card);
            }
        }
        deckButton.GetComponent<Collider2D>().enabled = true;
        AddToMap();

    }
    private void AddToMap()
    {
        zOffset = -0.2f;
        foreach(List<string> tmpStringList in deckTrips){
            foreach(string card in tmpStringList){
                GameObject newCard = Instantiate(cardPrefab, new Vector3(deckButton.transform.position.x, deckButton.transform.position.y, deckButton.transform.position.z+zOffset), Quaternion.identity, pivotDeck.transform);
                newCard.name = card;
                mapDeck.Add(card, newCard);
                newCard.gameObject.SetActive(false);
                zOffset+=-0.2f;
            }
        }
        discardPile.Clear();
    }
    void SolitaireSort()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = i; j < 7; j++)
            {
                bottoms[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }

        }

    }

    public void SortDeckIntoTrips(Option option)
    {
        int tmp = (int)option;
        trips = deck.Count / tmp;
        tripsRemainder = deck.Count % tmp;
        deckTrips.Clear();

        int modifier = 0;
        for (int i = 0; i < trips; i++)
        {
            List<string> myTrips = new List<string>();
            for (int j = 0; j < tmp; j++)
            {
                myTrips.Add(deck[j + modifier]);
            }
            deckTrips.Add(myTrips);
            modifier = modifier + tmp;
        }
        if (tripsRemainder != 0)
        {
            List<string> myRemainders = new List<string>();
            modifier = 0;
            for (int k = 0; k < tripsRemainder; k++)
            {
                myRemainders.Add(deck[deck.Count - tripsRemainder + modifier]);
                modifier++;
            }
            deckTrips.Add(myRemainders);
            trips++;
        }
        deckLocation = 0;
    }
    public void DealFromDeck()
    {
        if (!isClickedDeal && (tripsOnDisplay.Count > 0|| deck.Count >0))
        {
            isClickedDeal = true;
            undoManager.ExecuteCommand(new DealCommand(this));
            DOVirtual.DelayedCall(countDownClickDeal, () =>
            {
                isClickedDeal = false;
            });
        }
    }

    public void RestackTopDeck()
    {
        
        discardPile.Clear();
        deck.Clear();
        foreach (string card in tripsOnDisplay)
        {
            Debug.LogWarning(card);
            deck.Add(card);
        }
        tripsOnDisplay.Clear();
        Shuffle(deck);
        SortDeckIntoTrips(option);
        DOVirtual.DelayedCall(0.25f, () =>
        {
            ResetPosZFromDeck();
        });
    }
    private void ResetPosZFromDeck()
    {
        float offset = -0.2f;
        foreach (List<string> tmpStringList in deckTrips)
        {
            foreach (string tmpString in tmpStringList)
            {
                mapDeck[tmpString].transform.position = new Vector3(deckButton.transform.position.x, deckButton.transform.position.y, 0 + offset);
                Debug.LogError("This is : " + tmpString + " Position is: " + mapDeck[tmpString].transform.position);
                offset -= 0.2f;
            }
        }
    }
}
