using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
public class DealCommand : IAction
{
    Solitaire solitaire;
    List<string> listCardAddCurrent=new List<string>();
    List<string> listCardAddPrevious=new List<string>(); 
    List<Transform> transAdd=new List<Transform>(); 
    int deckcurrent;
    List<string> disCardPilePreviousClear=new List<string>();
    List<string> deckPreviousClear=new List<string>();
    List<string> onDisplayPreviousClear=new List<string>(); 
    public DealCommand(Solitaire solitaire)
    {
        this.solitaire = solitaire;
        deckcurrent = solitaire.deckLocation;
    }
    
    public void ExecuteCommand()
    {
        foreach (Transform child in solitaire.deckButton.transform)
        {
            if (child.CompareTag("Card") )
            {
                child.position = new Vector3(solitaire.pivotDeck.transform.position.x,child.position.y,child.position.z);
                if (!solitaire.discardPile.Contains(child.name))
                {
                 
                    solitaire.discardPile.Add(child.name);
                    listCardAddPrevious.Add(child.name);
                    solitaire.deck.Remove(child.name);
                    //child.gameObject.SetActive(false);
                }
            }
        }
        if (solitaire.deckLocation < solitaire.trips)
        {
            solitaire.deckPlaceHolder.gameObject.SetActive(true);
            // draw 3 new cards
            float xOffset = 0.7f;

            foreach (string card in solitaire.deckTrips[solitaire.deckLocation])
            {
               
                listCardAddCurrent.Add(card);
                GameObject newTopCard = solitaire.mapDeck[card];
                newTopCard.SetActive(true);
                newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x - xOffset, solitaire.deckButton.transform.position.y, solitaire.deckButton.transform.position.z + solitaire.zOffset);
                newTopCard.transform.parent = solitaire.deckButton.transform;
                xOffset = xOffset + 0.3f;
                solitaire.zOffset +=- 0.2f;
                solitaire.tripsOnDisplay.Add(card);
                newTopCard.GetComponent<Selectable>().cardFace = true;
                newTopCard.GetComponent<Selectable>().inDeckPile = true;
            }
            solitaire.deckLocation++;
            if(solitaire.deckLocation == solitaire.trips)
            {
                solitaire.deckPlaceHolder.gameObject.SetActive(false);
            }
        }
        else
        {
            solitaire.deckPlaceHolder.gameObject.SetActive(true);
            solitaire.zOffset =-0.2f;
            disCardPilePreviousClear.AddRange(solitaire.discardPile);
            deckPreviousClear.AddRange(solitaire.deck);
            onDisplayPreviousClear.AddRange(solitaire.tripsOnDisplay);
            deckcurrent = solitaire.deckLocation;
            List<Transform> tmp = new List<Transform>();
            foreach (Transform child in solitaire.deckButton.transform)
            {
                if (child.CompareTag("Card"))
                {
                    child.position = new Vector3(solitaire.deckButton.transform.position.x, solitaire. deckButton.transform.position.y, solitaire.deckButton.transform.position.z);
                    tmp.Add(child);
                    child.gameObject.SetActive(false);
                }
            }
            foreach (Transform child in tmp)
            {
                child.parent = solitaire.pivotDeck.transform;
            }
            solitaire.RestackTopDeck();
        }
    }

    public void UndoCommand()
    {
       
        if (deckcurrent < solitaire.trips)
        {
            if(deckcurrent == solitaire.deckLocation - 1)
            {
                solitaire.deckPlaceHolder.gameObject.SetActive(true);
            }
            solitaire.deckLocation = deckcurrent;
            float xOffset = 0.7f;
            foreach (Transform child in solitaire.deckButton.transform)
            {
                if (child.CompareTag("Card") && listCardAddCurrent.Contains(child.name))
                {
                    child.position = solitaire.pivotDeck.transform.position;
                    solitaire.deck.Add(child.name);
                    child.gameObject.SetActive(false);
                    solitaire.tripsOnDisplay.Remove(child.name);
                    transAdd.Add(child);
                }
            }
            foreach(Transform child in transAdd)
            {
                child.transform.parent = solitaire.pivotDeck.transform;
            }
            foreach (Transform child in solitaire.deckButton.transform)
            { 
                if(listCardAddPrevious.Contains(child.name))
                {
                    child.gameObject.SetActive(true);
                    child.transform.position = new Vector3(solitaire.deckButton.transform.position.x - xOffset, child.transform.position.y, child.transform.position.z);
                    xOffset = xOffset + 0.3f;
                    solitaire.discardPile.Remove(child.name);  
                }
            }
        }
        else
        {
            Debug.LogError("UNDO CLEAR"+" "+ onDisplayPreviousClear.Count);
            solitaire.deckPlaceHolder.gameObject.SetActive(false);
            solitaire.tripsOnDisplay.AddRange(onDisplayPreviousClear);
            solitaire.deck.AddRange(deckPreviousClear);
            solitaire.discardPile.AddRange(disCardPilePreviousClear);  
            solitaire.zOffset = -0.2f;
            solitaire.deckLocation = deckcurrent;
            int count = 0;
            while (count < disCardPilePreviousClear.Count)
            {
                Debug.Log(count);
                float xOffset = 0.7f;
                for (int j=count; j <= count + 2 && j < disCardPilePreviousClear.Count; j++)
                {
                    GameObject newTopCard = solitaire.mapDeck[disCardPilePreviousClear[j]];
                    newTopCard.SetActive(true);
                    newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x - xOffset, solitaire.deckButton.transform.position.y, solitaire.deckButton.transform.position.z + solitaire.zOffset);
                    newTopCard.transform.parent = solitaire.deckButton.transform;
                    xOffset = xOffset + 0.3f;
                    solitaire.zOffset += -0.2f;
                }
                count += 3;
            }
            int lech = count - disCardPilePreviousClear.Count;
            Debug.Log(lech);
          

        }
    }
}