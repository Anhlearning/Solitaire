using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using DG.Tweening;
using Solitaire_Manager.AudioManager;
using Solitaire_Manager.PointManger;
using Solitaire_Card;


namespace Solitaire_Manager.UndoManager
{
    public class Solitaire_MoveCardCommand : Solitaire_IAction
    {

        Vector3 oldPos, newPos;
        Solitaire_Selectable s1, s2;
        Transform oldParent;
        Solitaire solitaire;
        bool inDeckPile;
        bool s1Top;
        bool s2Top;
        int indexOfS1;
        int orderLayer;
        bool wasInDeckPile;
        int originalRow;
        bool wasTopS1;
        float durationMove;
        int currentPoint;
        public Solitaire_MoveCardCommand(Vector3 oldPostion, Vector3 newPostion, Solitaire_Selectable s1, Solitaire_Selectable s2, Transform oldParent, Solitaire solitaire, float durationMove)
        {
            this.oldPos = oldPostion;
            this.newPos = newPostion;
            orderLayer = s1.GetComponent<SortingGroup>().sortingOrder;
            this.s1 = s1;
            inDeckPile = s1.inDeckPile;
            this.s1Top = s1.isTop;
            this.s2Top = s2.isTop;
            this.oldParent = oldParent;
            this.s2 = s2;
            this.solitaire = solitaire;
            wasInDeckPile = s1.inDeckPile;
            originalRow = s1.row;
            wasTopS1 = s1.isTop;
            this.durationMove = durationMove;
            currentPoint = Solitaire_ManagerPoint.Instance.point;
        }


        public void ExecuteCommand()
        {
            s1.transform.parent = null;
            s1.GetComponent<SortingGroup>().sortingOrder = 1000;
            Solitaire_AudioManager.Instance.PlayCardMove();
            s1.transform.DOMove(newPos, durationMove).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                if (s2.isTop)
                {
                    Solitaire_AudioManager.Instance.PlayCardMoveHome();
                }
                s1.GetComponent<SortingGroup>().sortingOrder = s2.GetComponent<SortingGroup>().sortingOrder + 4;
                s1.transform.parent = s2.transform;
            });
            if (s1.inDeckPile) // removes the cards from the top pile to prevent duplicate cards
            {
                solitaire.tripsOnDisplay.Remove(s1.gameObject.name);
                if (solitaire.tripsOnDisplay.Count >= 3)
                {
                    for (int i = solitaire.tripsOnDisplay.Count - 1; i >= solitaire.tripsOnDisplay.Count - 2; i--)
                    {
                        Transform tmp = GameObject.Find(solitaire.tripsOnDisplay[i]).transform;
                        Vector3 newPos = new Vector3(tmp.position.x - solitaire.distanceCard, tmp.transform.position.y, tmp.transform.position.z);
                        tmp.DOMove(newPos, 0.15f).SetEase(Ease.InOutSine);
                    }
                }
            }
            else if (s1.isTop)
            {
                solitaire.topPos[s1.row].GetComponent<Solitaire_Selectable>().value = s1.value - 1;
                Solitaire_ManagerPoint.Instance.point -= 10;
            }
            else
            {
                solitaire.bottoms[s1.row].Remove(s1.gameObject.name);
            }

            s1.inDeckPile = false;
            s1.row = s2.row;
            if (s2.isTop)
            {
                solitaire.topPos[s1.row].GetComponent<Solitaire_Selectable>().value = s1.value;
                solitaire.topPos[s1.row].GetComponent<Solitaire_Selectable>().suit = s1.suit;
                Solitaire_ManagerPoint.Instance.point += 10;
                s1.isTop = true;
            }
            else
            {
                s1.isTop = false;
            }
        }

        public void UndoCommand()
        {
            s1.transform.parent = null;
            s1.GetComponent<SortingGroup>().sortingOrder = 1000;
            Solitaire_ManagerPoint.Instance.point = currentPoint;
            Solitaire_AudioManager.Instance.PlayCardMove();
            s1.transform.DOMove(oldPos, durationMove).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                if (wasTopS1)
                {
                    Solitaire_AudioManager.Instance.PlayCardMoveHome();
                }
                s1.GetComponent<SortingGroup>().sortingOrder = orderLayer;
                s1.transform.parent = oldParent;
            });

            if (wasInDeckPile)
            {
                Debug.Log(indexOfS1);
                if (solitaire.tripsOnDisplay.Count >= 3)
                {
                    for (int i = solitaire.tripsOnDisplay.Count - 1; i >= solitaire.tripsOnDisplay.Count - 2; i--)
                    {
                        Transform tmp = GameObject.Find(solitaire.tripsOnDisplay[i]).transform;
                        Vector3 newPosTmp = new Vector3(tmp.position.x + solitaire.distanceCard, tmp.transform.position.y, tmp.transform.position.z);
                        tmp.DOMove(newPosTmp, 0.15f).SetEase(Ease.InOutSine);
                    }
                }
                solitaire.tripsOnDisplay.Add(s1.name);

                s1.inDeckPile = wasInDeckPile;
                s1.row = 0;
            }
            else if (wasTopS1)
            {
                //1 bug cho nay -----------------------
                s1.row = originalRow;
                solitaire.topPos[s1.row].GetComponent<Solitaire_Selectable>().value = s1.value;
                s1.isTop = wasTopS1;
            }
            else // removes the card string from the appropriate bottom list
            {
                solitaire.bottoms[s1.row].Remove(s1.name);
                s1.row = originalRow;
                solitaire.bottoms[originalRow].Add(s1.name);
            }
            // you cannot add cards to the trips pile so this is always fine
            if (this.s2Top) // moves a card to the top and assigns the top's value and suit
            {
                solitaire.topPos[s2.row].GetComponent<Solitaire_Selectable>().value = s1.value - 1;
                solitaire.topPos[s2.row].GetComponent<Solitaire_Selectable>().suit = s2.suit;
                s1.isTop = wasTopS1;
            }
        }

    }
}

