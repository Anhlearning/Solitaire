using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using Solitaire_Input;
using Solitaire_Input.UserInput;
namespace Solitaire_Card
{
    public class Solitaire_UpdateVisual : MonoBehaviour
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
        private Solitaire_Selectable selectable;
        private Solitaire solitaire;
        private Solitaire_UserInput userInput;
        private void Awake()
        {
            userInput = FindObjectOfType<Solitaire_UserInput>();
            solitaire = FindObjectOfType<Solitaire>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            selectable = GetComponent<Solitaire_Selectable>();
        }
        void Start()
        {

            List<string> deck = Solitaire.GenerateDeck();
            for (int i = 0; i < deck.Count; i++)
            {
                if (deck[i] == gameObject.name)
                {
                    foreach (SpriteRenderer number in numbers)
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
                        if (deck[i][1] == 'J' || deck[i][1] == 'Q' || deck[i][1] == 'K')
                        {
                            suit.color = number.color;
                        }
                    }
                    suit.sprite = solitaire.cardSpriteList[i].suitCenter;
                    suitsmall.sprite = solitaire.cardSpriteList[i].suitsmall;
                    break;
                }
            }
        }
        public void TurnOnCardFace()
        {

            cardFace.gameObject.SetActive(true);
            cardBack.gameObject.SetActive(false);

        }
        public void TurnOffCardFace()
        {

            cardFace.gameObject.SetActive(false);
            cardBack.gameObject.SetActive(true);

        }
        void Update()
        {
            if (userInput.slot1)
            {
                if (userInput.slot1.name == name)
                {
                    spriteRenderer.color = new Color32(221, 182, 108, 255);
                }
                else
                {
                    spriteRenderer.color = new Color32(41, 58, 58, 255);
                }
            }
        }
    }
}

