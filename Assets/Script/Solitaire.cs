using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solitaire : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefabs;
    [SerializeField] private GameObject deckPos;
    [SerializeField] public Sprite[] cardFaces;
    public GameObject[] BottomPos;
    public GameObject[] TopPos;
    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] value = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    public List<string>[] bottoms;
    public List<string>[] tops;
    public List<string> tripOnDisplay=new List<string>();
    private List<List<string>> deckTrips=new List<List<string>>();
    private int trips;
    private int tripsRemain;
    private int deckLocation;
    private List<string> discardPile= new List<string>();   
    private List<string> bottom0=new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();
    
    List<string>Deck=new List<string>();
    private void Awake()
    {
        bottoms = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };
        Deck = Generate();
    }
    void Start()
    {
        
        Shuffer(Deck);  
        foreach(string s in Deck)
        {
            Debug.Log(s);
        }
        SolitaireSort();
        StartCoroutine(SolitaireDeal());
        SortDeckIntoTrips();
    }

    public static List<string> Generate()
    {
        List<string>tmp=new List<string>();
        foreach(string s in value)
        {
            foreach(string v in suits)
            {
                tmp.Add(v + s);
            }
        }
        return tmp; 
    }
    public void Shuffer<T>(List<T> list)
    {
        int n=list.Count;
        System.Random rand = new System.Random();
        while (n > 1)
        {
            int k=rand.Next(n); 
            n--;
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }
    public List<string> GetDeck()
    {
        return this.Deck;
    }
    IEnumerator SolitaireDeal()
    {
        for(int i = 0; i < 7; i++)
        {
            float zOffset = .1f;
            float yOffset = .0f;
            foreach (string card in bottoms[i])
            {   
                GameObject cardObject = Instantiate(cardPrefabs, new Vector3(BottomPos[i].transform.position.x, BottomPos[i].transform.position.y - yOffset, BottomPos[i].transform.position.z - zOffset), Quaternion.identity, BottomPos[i].transform);
                cardObject.name = card;
                cardObject.GetComponent<Selectable>().row = i;
                yield return new WaitForSeconds(0.01f);
                if (card == bottoms[i][bottoms[i].Count - 1])
                {
                    cardObject.GetComponent<Selectable>().cardFace = true;
                }
                yOffset += 0.1f;
                zOffset += 0.03f;
                discardPile.Add(card);
            }
        }
        foreach(string tmp in discardPile)
        {
            if (Deck.Contains(tmp))
            {
                Deck.Remove(tmp);
            }
        }   
        discardPile.Clear();  
    }
    //Them vao bottom cac la bai 
    void SolitaireSort()
    {
        for(int i = 0; i < 7; i++)
        {
            for(int j = i; j < 7; j++)
            {
                    bottoms[j].Add(Deck.Last<string>());
                    Deck.RemoveAt(Deck.Count-1);
            }
        }
    }
    public void SortDeckIntoTrips()
    {
        trips = Deck.Count / 3;
        tripsRemain = Deck.Count % 3;

        deckTrips.Clear();

        int modifier = 0;
        for(int i = 0; i < trips; i++)
        {
            List<string> tmpList=new List<string>();
            for(int j = 0; j < 3; j++)
            {
                tmpList.Add(Deck[j + modifier]);
            }
            modifier += 3;
            deckTrips.Add(tmpList); 
        }
        if(tripsRemain >0)
        {
            modifier = 0;
            List<string> tmpList=new List<string>();    
            for(int i = 0; i < tripsRemain; i++)
            {
                tmpList.Add(Deck[Deck.Count - tripsRemain + modifier]);
                modifier++;
            }
            deckTrips.Add(tmpList);
            trips++;  
        }
        deckLocation = 0;
    }
    public void DeadlFromDeck()
    {
        foreach(Transform child in deckPos.transform)
        {
            if (child.gameObject.CompareTag("Card"))
            {
                Deck.Remove(child.name);
                discardPile.Add(child.name);
                Destroy(child.gameObject);  
            }
        }

        if(deckLocation < trips)
        {
            tripOnDisplay.Clear();
            float xOffset = 0.7f;
            float zOffset = -0.2f;
            foreach(string card in deckTrips[deckLocation])
            {
                GameObject newCard = Instantiate(cardPrefabs, new Vector3(deckPos.transform.position.x - xOffset, deckPos.transform.position.y, deckPos.transform.position.z + zOffset),Quaternion.identity,deckPos.transform);
                xOffset += 0.1f;
                zOffset += -0.03f;
                newCard.name = card;
                tripOnDisplay.Add(card);
                newCard.GetComponent<Selectable>().inDeckPile = true;
                newCard.GetComponent<Selectable>().cardFace = true;
            }
            deckLocation++;
        }
        else
        {
            RestackTopDeck();
        }
    }
    private void RestackTopDeck()
    {
        foreach(string card in discardPile)
        {
            Deck.Add(card);
        }
        discardPile.Clear();    
        SortDeckIntoTrips();    
    }
    
}
