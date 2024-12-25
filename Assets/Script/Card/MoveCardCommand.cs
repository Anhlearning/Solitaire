using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MoveCardCommand : IAction
{
    
    Vector3 oldPos, newPos;
    Selectable s1, s2;
    Transform oldParent;
    Solitaire solitaire;
    bool inDeckPile;
    bool s1Top;
    bool s2Top;
    int indexOfS1;
    public MoveCardCommand(Vector3 oldPostion,Vector3 newPostion,Selectable s1,Selectable s2,Transform oldParent,Solitaire solitaire) 
    {
        this.oldPos = oldPostion;   
        this.newPos = newPostion;
        this.s1 = s1;
        inDeckPile = s1.inDeckPile;
        this.s1Top = s1.isTop;
        this.s2Top = s2.isTop;
        this.oldParent = oldParent;
        this.s2 = s2;
        this.solitaire = solitaire;
        if (s1.inDeckPile)
        {
            indexOfS1 = solitaire.tripsOnDisplay.IndexOf(s1.gameObject.name);
        }
        else
        {
            indexOfS1 = solitaire.bottoms[s1.row].IndexOf(s1.name);
        }
    }


    public void ExecuteCommand()
    {
        s1.transform.parent = s2.transform; 
        s1.transform.position = newPos;
        s1.GetComponent<SortingGroup>().sortingOrder = s2.GetComponent<SortingGroup>().sortingOrder + 4;
        if (s1.inDeckPile) // removes the cards from the top pile to prevent duplicate cards
        {
            solitaire.tripsOnDisplay.Remove(s1.gameObject.name);
            if(solitaire.tripsOnDisplay.Count > 3)
            {
                for (int i = solitaire.tripsOnDisplay.Count - 1; i >= solitaire.tripsOnDisplay.Count - 2; i--)
                {
                    Transform tmp = GameObject.Find(solitaire.tripsOnDisplay[i]).transform;
                    tmp.transform.position = new Vector3(tmp.position.x - 0.3f, tmp.transform.position.y, tmp.transform.position.z);
                }
            }
        }
        else if (s1.isTop) // keeps track of the current value of the top decks as a card has been removed
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value - 1;
        }
        else // removes the card string from the appropriate bottom list
        {
            solitaire.bottoms[s1.row].Remove(s1.gameObject.name);
        }

        s1.inDeckPile = false; // you cannot add cards to the trips pile so this is always fine
        s1.row = s2.row;

        if (s2.isTop) // moves a card to the top and assigns the top's value and suit
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            solitaire.topPos[s1.row].GetComponent<Selectable>().suit = s1.suit;
            s1.isTop = true;
        }
        else
        {
            s1.isTop = false;
        }
    }

    public void UndoCommand()
    {
        s1.transform.position=oldPos;
        s1.transform.parent=oldParent;
        s1.GetComponent<SortingGroup>().sortingOrder = 0;
        if (this.inDeckPile) 
        {
            Debug.Log(indexOfS1);
            if (solitaire.tripsOnDisplay.Count > 3)
            {
                for (int i = solitaire.tripsOnDisplay.Count - 1; i >= solitaire.tripsOnDisplay.Count - 2; i--)
                {
                    Transform tmp = GameObject.Find(solitaire.tripsOnDisplay[i]).transform;
                    tmp.transform.position = new Vector3(tmp.position.x + 0.3f, tmp.transform.position.y, tmp.transform.position.z);
                }
            }
            solitaire.tripsOnDisplay.Add(s1.name);    
            
            s1.inDeckPile = true;
            s1.row = 0;
        }
        else if (this.s1Top)
        {
            solitaire.topPos[s1.row].GetComponent<Selectable>().value = s1.value;
            s1.isTop = true;
            s1.row = oldParent.GetComponent<Selectable>().row;
        }
        else // removes the card string from the appropriate bottom list
        {
            Debug.Log(indexOfS1);
            s1.row = oldParent.GetComponent<Selectable>().row;
            solitaire.bottoms[s1.row].Add(s1.name);
        }
        // you cannot add cards to the trips pile so this is always fine
        if (this.s2Top) // moves a card to the top and assigns the top's value and suit
        {
            solitaire.topPos[s2.row].GetComponent<Selectable>().value = s1.value-1;
            solitaire.topPos[s2.row].GetComponent<Selectable>().suit = s2.suit;
            s1.isTop = false;
        }
    }

}
