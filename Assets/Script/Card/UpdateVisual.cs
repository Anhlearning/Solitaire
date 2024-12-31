using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class UpdateVisual : MonoBehaviour
{
    [SerializeField]
    private Transform cardFace;
    [SerializeField]
    private List<SpriteRenderer> numbers;
    [SerializeField]
    private SpriteRenderer suit;
    [SerializeField]
    private SpriteRenderer suitsmall;
    [SerializeField]
    private Transform cardBack;
    private SpriteRenderer spriteRenderer;
    private Selectable selectable;
    private Solitaire solitaire;
    private UserInput userInput;
    private void Awake()
    {
        userInput = FindObjectOfType<UserInput>();
        solitaire = FindObjectOfType<Solitaire>();
        spriteRenderer=GetComponent<SpriteRenderer>();
        selectable = GetComponent<Selectable>();
    }
    void Start()
    {
           
        List<string>deck=Solitaire.GenerateDeck();
        for(int i = 0; i < deck.Count; i++)
        {
            if (deck[i] == gameObject.name)
            {
                foreach(SpriteRenderer number in numbers) 
                {
                    number.sprite = solitaire.cardSpriteList[i].number;
                    if (deck[i][0] == 'H' || deck[i][0] == 'D')
                    {
                        number.color = new Color32(255, 93, 82, 255);
                    }
                    else
                    {
                        number.color = new Color32(41, 56, 57, 255);
                    }
                    if (deck[i][1]=='J' || deck[i][1]=='Q'|| deck[i][1] == 'K')
                    {
                        suit.color=number.color;    
                    }
                }
                suit.sprite = solitaire.cardSpriteList[i].suitCenter;
                suitsmall.sprite = solitaire.cardSpriteList[i].suitsmall;
                break;
            }
        }

    }
    void Update()
    {
        if (selectable.cardFace)
        {
            cardFace.gameObject.SetActive(true);    
            cardBack.gameObject.SetActive(false);
        }
        else
        {
            cardFace.gameObject.SetActive(false);
            cardBack.gameObject.SetActive(true);   
        }
        if (userInput.slot1)
        {
            if (userInput.slot1.name == name)
            {
                spriteRenderer.color = Color.yellow;
            }
            else
            {
                spriteRenderer.color = new Color32(41,58,58,255);
            }
        }
    }
}
