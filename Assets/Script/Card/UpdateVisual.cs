using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateVisual : MonoBehaviour
{
    [SerializeField]
    private Sprite cardFace;
    [SerializeField]
    private Sprite cardBack;

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
           
        List<string>deck=Solitaire.Generate();
        for(int i = 0; i < deck.Count; i++)
        {
            if (deck[i] == gameObject.name)
            {
                cardFace = solitaire.cardFaces[i];
                break;
            }
        }

    }
    void Update()
    {
        if (selectable.cardFace)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite=cardBack;
        }
        if (userInput.slot1)
        {
            if (userInput.slot1.name == name)
            {
                spriteRenderer.color = Color.yellow;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}
