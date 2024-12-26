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
        disCardPilePreviousClear.AddRange(solitaire.discardPile);
        deckPreviousClear.AddRange(solitaire.deck);
        onDisplayPreviousClear.AddRange(solitaire.tripsOnDisplay);
        int offsetMove =(int) solitaire.option;
        if (offsetMove + solitaire.deckButton.transform.childCount > 5)
        {
            for(int i=0;i<offsetMove; i++) 
            {
                Transform child = solitaire.deckButton.transform.GetChild(solitaire.deckButton.transform.childCount - 1 - i);
                float minPos = Mathf.Min(-0.7f, solitaire.deckButton.transform.GetChild(solitaire.deckButton.transform.childCount - 1-i).transform.position.x + offsetMove * 0.3f);
                child.transform.position = new Vector3(minPos, child.position.y, child.position.z);
            }
            
        }
        float lastChildPos =  (solitaire.deckButton.transform.childCount > 2) ? solitaire.deckButton.transform.GetChild(solitaire.deckButton.transform.childCount - 1).position.x : 0.7f;
        Debug.Log(lastChildPos);
        foreach (Transform child in solitaire.deckButton.transform)
        {
            if (child.CompareTag("Card"))
            {
                if (!solitaire.discardPile.Contains(child.name))
                {
                    solitaire.discardPile.Add(child.name);
                    listCardAddPrevious.Add(child.name);
                }
            }
        }
        if (solitaire.deckLocation < solitaire.trips)
        {
            solitaire.deckPlaceHolder.gameObject.SetActive(true);
            float xOffset = 0.3f;
            foreach (string card in solitaire.deckTrips[solitaire.deckLocation])
            {
                listCardAddCurrent.Add(card);
                GameObject newTopCard = solitaire.mapDeck[card];
                newTopCard.SetActive(true);
                newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x-lastChildPos, solitaire.deckButton.transform.position.y, solitaire.deckButton.transform.position.z + solitaire.zOffset);
                newTopCard.transform.parent = solitaire.deckButton.transform;
                lastChildPos = lastChildPos + xOffset;
                //xOffset = xOffset + 0.3f;
                solitaire.zOffset +=- 0.2f;
                solitaire.tripsOnDisplay.Add(card);
                solitaire.deck.Remove(card);
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
            Debug.LogWarning(disCardPilePreviousClear.Count + " " + deckPreviousClear.Count + " " + onDisplayPreviousClear.Count);
            solitaire.deckPlaceHolder.gameObject.SetActive(true);
            solitaire.zOffset =-0.2f;
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
            //float xOffset = 0.7f;
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
            for (int i= 0; i < solitaire.deckButton.transform.childCount;i++)
            {
                Transform child = solitaire.deckButton.transform.GetChild(i); 
                if(listCardAddPrevious.Contains(child.name))
                {
                    int ii=listCardAddPrevious.IndexOf(child.name);
                    child.gameObject.SetActive(true);
                    if (i < 3)
                    {
                        child.transform.position = new Vector3(solitaire.deckButton.transform.position.x - 0.7f, child.transform.position.y, child.transform.position.z);

                    }
                    else if(i>=3 && ii > 0)
                    {
                        child.transform.position = new Vector3(solitaire.deckButton.transform.GetChild(i - 1).position.x - 0.3f, child.transform.position.y, child.transform.position.z);
                    }
                    //xOffset = xOffset + 0.3f;
                    solitaire.discardPile.Remove(child.name);  
                }
            }
        }
        else
        {
            Debug.LogError("UNDO CLEAR"+" "+ onDisplayPreviousClear.Count);
            solitaire.deckPlaceHolder.gameObject.SetActive(false);
            solitaire.tripsOnDisplay.Clear();
            solitaire.deck.Clear();
            solitaire.discardPile.Clear();
            solitaire.tripsOnDisplay.AddRange(onDisplayPreviousClear);
            solitaire.deck.AddRange(deckPreviousClear);
            solitaire.discardPile.AddRange(disCardPilePreviousClear);  
            solitaire.zOffset = -0.2f;
            solitaire.deckLocation = deckcurrent;
            int count = 0;
            int countLoop = onDisplayPreviousClear.Count / 3;
            Debug.Log(countLoop);
            while (count < countLoop)
            {
                foreach (Transform child in solitaire.deckButton.transform)
                {
                    if (child.CompareTag("Card"))
                    {
                        child.position = new Vector3(solitaire.pivotDeck.transform.position.x, child.position.y, child.position.z);
                    }
                }
                float xOffset = 0.7f;
                for (int j=count*3; j <= count*3 + 2 ; j++)
                {
                    Debug.LogError(solitaire.mapDeck[onDisplayPreviousClear[j]].name);
                    GameObject newTopCard = solitaire.mapDeck[onDisplayPreviousClear[j]];
                    newTopCard.SetActive(true);
                    newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x - xOffset, solitaire.deckButton.transform.position.y, solitaire.deckButton.transform.position.z + solitaire.zOffset);
                    newTopCard.transform.parent = solitaire.deckButton.transform;
                    xOffset = xOffset + 0.3f;
                    solitaire.zOffset += -0.2f;
                }
                count += 1;
            }
            int lech = onDisplayPreviousClear.Count-countLoop*3;
            if (lech > 0)
            {
                if (onDisplayPreviousClear.Count > 3)
                {
                    if (lech == 1)
                    {
                        float xOffset = 0.7f;
                        for (int j = onDisplayPreviousClear.Count - lech-2; j < onDisplayPreviousClear.Count; j++)
                        {
                            GameObject newTopCard = solitaire.mapDeck[onDisplayPreviousClear[j]];
                            newTopCard.SetActive(true);
                            newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x - xOffset, solitaire.deckButton.transform.position.y, solitaire.deckButton.transform.position.z + solitaire.zOffset);
                            newTopCard.transform.parent = solitaire.deckButton.transform;
                            xOffset = xOffset + 0.3f;
                            solitaire.zOffset += -0.2f;
                        }
                    }
                    else if (lech == 2)
                    {
                        foreach (Transform child in solitaire.deckButton.transform)
                        {
                            if (child.CompareTag("Card"))
                            {
                                child.position = new Vector3(solitaire.pivotDeck.transform.position.x, child.position.y, child.position.z);
                            }
                        }
                        float xOffset = 1.0f;
                        for (int j = onDisplayPreviousClear.Count - lech; j < onDisplayPreviousClear.Count; j++)
                        {
                            GameObject newTopCard = solitaire.mapDeck[onDisplayPreviousClear[j]];
                            newTopCard.SetActive(true);
                            newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x - xOffset, solitaire.deckButton.transform.position.y, solitaire.deckButton.transform.position.z + solitaire.zOffset);
                            newTopCard.transform.parent = solitaire.deckButton.transform;
                            xOffset = xOffset + 0.3f;
                            solitaire.zOffset += -0.2f;
                        }
                    }
                }

            }

        }
    }
}
