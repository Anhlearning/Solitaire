using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Solitaire : MonoBehaviour
{
    [SerializeField] public Sprite[] cardFaces;
    [SerializeField] private GameObject cardPrefabs;

    [SerializeField] private GameObject[] BottomPos;
    [SerializeField] private GameObject[] TopPos;
    public static string[] suits = new string[] { "C", "D", "H", "S" };
    public static string[] value = new string[] { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
    private List<string>[] bottoms;
    private List<string>[] tops;

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
    }
    void Update()
    {
        
    }
    public static List<string> Generate()
    {
        List<string>tmp=new List<string>();
        foreach(string s in suits)
        {
            foreach(string v in value)
            {
                tmp.Add(s + v);
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
            float zOffset = .0f;
            float yOffset = .0f;
            foreach (string card in bottoms[i])
            {   
                GameObject cardObject = Instantiate(cardPrefabs, new Vector3(BottomPos[i].transform.position.x, BottomPos[i].transform.position.y - yOffset, BottomPos[i].transform.position.z - zOffset), Quaternion.identity, BottomPos[i].transform);
                cardObject.name = card;
                yield return new WaitForSeconds(0.01f);
                if (card == bottoms[i][bottoms[i].Count - 1])
                {
                    cardObject.GetComponent<Selectable>().cardFace = true;
                }
                yOffset += 0.1f;
                zOffset += 0.03f;
            }
        }
        
    }
    void SolitaireSort()
    {
        for(int i = 0; i < 7; i++)
        {
            for(int j = i; j < 7; j++)
            {
                try
                {
                    Debug.Log(Deck.Last<string>());
                    bottoms[j].Add(Deck.Last<string>());
                    Deck.RemoveAt(Deck.Count-1);
                }
                catch
                {
                    
                }
            }
        }
    }
}
