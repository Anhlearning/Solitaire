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
    List<List<string>> currentDeckTrips=new List<List<string>>();
    int deckcurrent;
    int tripsCurrent;
    List<string> disCardPilePreviousClear=new List<string>();
    List<string> deckPreviousClear=new List<string>();
    List<string> onDisplayPreviousClear=new List<string>(); 
    Dictionary<string,float> offsetZOfList=new Dictionary<string, float>();
    int offsetMove;
    public DealCommand(Solitaire solitaire)
    {
        this.solitaire = solitaire;
        deckcurrent = solitaire.deckLocation;

        currentDeckTrips.AddRange(solitaire.deckTrips);
        offsetMove =(int) solitaire.option;
        tripsCurrent=solitaire.trips;
    }
    
    public void ExecuteCommand()
    {
        disCardPilePreviousClear.AddRange(solitaire.discardPile);
        deckPreviousClear.AddRange(solitaire.deck);
        onDisplayPreviousClear.AddRange(solitaire.tripsOnDisplay);
        
        if (offsetMove + solitaire.deckButton.transform.childCount > 5)
        {
            for(int i=solitaire.deckButton.transform.childCount-2;i<=solitaire.deckButton.transform.childCount-1; i++) 
            {
                Transform child = solitaire.deckButton.transform.GetChild(i);
                if(child.CompareTag("Card")){
                float minPos = Mathf.Min(-0.7f, solitaire.deckButton.transform.GetChild(i).transform.localPosition.x + offsetMove * 0.3f);
                Debug.Log(child.transform.name+" "+minPos);
                child.transform.position = new Vector3(solitaire.deckButton.transform.position.x+minPos, child.position.y, child.position.z);
                }
            }
        }
        Debug.LogWarning(solitaire.deckButton.transform.GetChild(solitaire.deckButton.transform.childCount - 1).localPosition.x);
        float lastChildPos = solitaire.deckButton.transform.GetChild(solitaire.deckButton.transform.childCount - 1).localPosition.x-0.3*offsetMove >= -1.35f ? solitaire.deckButton.transform.GetChild(solitaire.deckButton.transform.childCount - 1).localPosition.x:-0.4f ;
        Debug.Log(solitaire.deckButton.transform.GetChild(solitaire.deckButton.transform.childCount - 1).localPosition.x-0.3*offsetMove);
        lastChildPos-=0.3f;
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
        if (deckcurrent < tripsCurrent)
        {
            solitaire.deckPlaceHolder.gameObject.SetActive(true);
            foreach (string card in solitaire.deckTrips[deckcurrent])
            {
                listCardAddCurrent.Add(card);
                GameObject newTopCard = solitaire.mapDeck[card];
                newTopCard.SetActive(true);
                newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x+lastChildPos, solitaire.deckButton.transform.position.y,newTopCard.transform.position.z);
                newTopCard.transform.parent = solitaire.deckButton.transform;
                lastChildPos = lastChildPos - 0.3f;
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
            List<Transform> tmp = new List<Transform>();
            foreach(string stringDic in solitaire.mapDeck.Keys){
                offsetZOfList[stringDic]=solitaire.mapDeck[stringDic].transform.position.z;
            }
            foreach (Transform child in solitaire.deckButton.transform)
            {
                if (child.CompareTag("Card"))
                {
                    child.position = new Vector3(solitaire.deckButton.transform.position.x, solitaire. deckButton.transform.position.y,child.position.z);
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
       
        if (deckcurrent < tripsCurrent)
        {
            solitaire.deckPlaceHolder.gameObject.SetActive(true);
            solitaire.deckLocation = deckcurrent;
            if(deckcurrent == solitaire.deckLocation - 1)
            {
                solitaire.deckPlaceHolder.gameObject.SetActive(true);
            }
            foreach (Transform child in solitaire.deckButton.transform)
            {
                if (child.CompareTag("Card") && listCardAddCurrent.Contains(child.name))
                {
                    solitaire.deck.Add(child.name);
                    child.gameObject.SetActive(false);
                    solitaire.tripsOnDisplay.Remove(child.name);
                    transAdd.Add(child);
                }
            }
            foreach(Transform child in transAdd)
            {
                child.position = new Vector3(solitaire.pivotDeck.transform.position.x-0.3f, child.position.y, child.position.z);
                child.transform.parent = solitaire.pivotDeck.transform;
            }
            int count=solitaire.deckButton.transform.childCount;
            float lastChildPos = -0.7f;
            for (int i= count-3; i < count && count >=3;i++)
            {
                Transform child = solitaire.deckButton.transform.GetChild(i); 
                if(child.gameObject.CompareTag("Card")){
                    Debug.Log(child.name);
                    child.transform.position = new Vector3(solitaire.deckButton.transform.position.x+lastChildPos, solitaire.deckButton.transform.position.y, child.transform.position.z);
                    lastChildPos -= 0.3f;
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
            solitaire.deckTrips.Clear();
            solitaire.tripsOnDisplay.AddRange(onDisplayPreviousClear);
            solitaire.deck.AddRange(deckPreviousClear);
            solitaire.discardPile.AddRange(disCardPilePreviousClear);  
            solitaire.deckTrips.AddRange(currentDeckTrips);
            solitaire.deckLocation = deckcurrent;
            solitaire.trips=tripsCurrent;
            int count = 0;
            int countLoop = onDisplayPreviousClear.Count / 3;
            foreach(string tmpString in onDisplayPreviousClear){
                solitaire.mapDeck[tmpString].transform.position= new Vector3( solitaire.mapDeck[tmpString].transform.position.x, solitaire.mapDeck[tmpString].transform.position.y, offsetZOfList[tmpString]);
            }
            Debug.Log(countLoop);
            while (count < countLoop)
            {
                foreach (Transform child in solitaire.deckButton.transform)
                {
                    if (child.CompareTag("Card"))
                    {
                        child.position = new Vector3(solitaire.pivotDeck.transform.position.x-0.3f, child.position.y, child.position.z);
                        Debug.Log("NEW POS OF"+" "+child.name+" "+child.position);
                    }
                }
                float xOffset = 0.7f;
                for (int j=count*3; j <= count*3 + 2 ; j++)
                {
                    Debug.LogError(solitaire.mapDeck[onDisplayPreviousClear[j]].name);
                    GameObject newTopCard = solitaire.mapDeck[onDisplayPreviousClear[j]];
                    newTopCard.SetActive(true);
                    newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x - xOffset, solitaire.deckButton.transform.position.y, newTopCard.transform.position.z);
                    newTopCard.transform.parent = solitaire.deckButton.transform;
                    Debug.Log("NEW POS OF s2 "+" "+newTopCard.name+" "+newTopCard.transform.position);
                    xOffset = xOffset + 0.3f;
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
                            newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x - xOffset, solitaire.deckButton.transform.position.y, newTopCard.transform.position.z);
                            newTopCard.transform.parent = solitaire.deckButton.transform;
                            xOffset = xOffset + 0.3f;
                        }
                    }
                    else if (lech == 2)
                    {
                        foreach (Transform child in solitaire.deckButton.transform)
                        {
                            if (child.CompareTag("Card"))
                            {
                                child.position = new Vector3(solitaire.pivotDeck.transform.position.x-0.3f, child.position.y, child.position.z);
                            }
                        }
                        float xOffset = 1.0f;
                        for (int j = onDisplayPreviousClear.Count - lech; j < onDisplayPreviousClear.Count; j++)
                        {
                            GameObject newTopCard = solitaire.mapDeck[onDisplayPreviousClear[j]];
                            newTopCard.SetActive(true);
                            newTopCard.transform.position = new Vector3(solitaire.deckButton.transform.position.x - xOffset, solitaire.deckButton.transform.position.y,newTopCard.transform.position.z);
                            newTopCard.transform.parent = solitaire.deckButton.transform;
                            xOffset = xOffset + 0.3f;
                        }
                    }
                }

            }

        }
    }
}
