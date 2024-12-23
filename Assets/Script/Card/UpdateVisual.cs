using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateVisual : MonoBehaviour
{
    [SerializeField]
    private Transform cardFace;
    [SerializeField]
    private SpriteRenderer number;
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
                number.sprite = solitaire.cardSpriteList[i].number;
                suit.sprite = solitaire.cardSpriteList[i].suit;
                suitsmall.sprite = solitaire.cardSpriteList[i].suit;
                if (deck[i][0] == 'H' || deck[i][0] == 'D')
                {
                    number.color = new Color32(250, 108, 100, 255);
                }
                else
                {
                    number.color = new Color32(40, 58, 56, 255);
                }
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
