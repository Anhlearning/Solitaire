using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Solitaire_Manager.AudioManager;
using Solitaire_Manager.PointManger;
using Solitaire_Card;

namespace Solitaire_Manager.UndoManager
{
    public class Soliraire_DealCommand : Solitaire_IAction
    {
        Solitaire solitaire;
        List<string> listCardAddCurrent = new List<string>();
        List<string> listCardAddPrevious = new List<string>();
        List<Transform> transAdd = new List<Transform>();
        List<List<string>> currentDeckTrips = new List<List<string>>();
        int deckcurrent;
        int tripsCurrent;
        List<string> disCardPilePreviousClear = new List<string>();
        List<string> deckPreviousClear = new List<string>();
        List<string> onDisplayPreviousClear = new List<string>();
        Dictionary<string, float> offsetZOfList = new Dictionary<string, float>();
        int offsetMove;
        int currentPoint;
        public Soliraire_DealCommand(Solitaire solitaire)
        {
            this.solitaire = solitaire;
            deckcurrent = solitaire.deckLocation;
            currentDeckTrips.AddRange(solitaire.deckTrips);
            offsetMove = (int)solitaire.option;
            tripsCurrent = solitaire.trips;
            currentPoint = Solitaire_ManagerPoint.Instance.point;
        }

        public void ExecuteCommand()
        {
            Solitaire_AudioManager.Instance.PlayDealDeck();
            disCardPilePreviousClear.AddRange(solitaire.discardPile);
            deckPreviousClear.AddRange(solitaire.deck);
            onDisplayPreviousClear.AddRange(solitaire.tripsOnDisplay);

            float posChildLastX = solitaire.deckButton.transform.GetChild(solitaire.deckButton.transform.childCount - 1).localPosition.x;
            if (offsetMove + solitaire.deckButton.transform.childCount > 5)
            {
                for (int i = solitaire.deckButton.transform.childCount - 2; i <= solitaire.deckButton.transform.childCount - 1; i++)
                {
                    Transform child = solitaire.deckButton.transform.GetChild(i);
                    if (child.CompareTag("Card"))
                    {
                        if (offsetMove <= solitaire.deck.Count)
                        {
                            float minPos = Mathf.Min(-0.7f, solitaire.deckButton.transform.GetChild(i).transform.localPosition.x + offsetMove * 0.25f);

                            Vector3 newPos = new Vector3(solitaire.deckButton.transform.position.x + minPos, child.position.y, child.position.z);
                            posChildLastX = minPos;
                            child.DOMove(newPos, 0.25f).SetEase(Ease.InOutSine);
                        }
                        else
                        {
                            float minPos = Mathf.Min(-0.7f, solitaire.deckButton.transform.GetChild(i).transform.localPosition.x + solitaire.deck.Count * 0.25f);
                            Vector3 newPos = new Vector3(solitaire.deckButton.transform.position.x + minPos, child.position.y, child.position.z);
                            posChildLastX = minPos;
                            child.DOMove(newPos, 0.25f).SetEase(Ease.InOutSine);
                        }
                    }
                }
            }

            float lastChildPos = 0;
            if (offsetMove <= solitaire.deck.Count)
            {
                lastChildPos = posChildLastX - 0.25 * offsetMove >= -1.3f ? posChildLastX : -0.45f;

            }
            else
            {
                lastChildPos = posChildLastX - 0.25 * solitaire.deck.Count >= -1.3f ? posChildLastX : -0.45f;
            }
            lastChildPos -= 0.25f;
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
                    Vector3 newPos = new Vector3(solitaire.deckButton.transform.position.x + lastChildPos, solitaire.deckButton.transform.position.y, newTopCard.transform.position.z);
                    newTopCard.transform.parent = solitaire.deckButton.transform;
                    newTopCard.transform.DOMove(newPos, 0.25f).SetEase(Ease.InOutSine);
                    lastChildPos = lastChildPos - 0.25f;
                    solitaire.tripsOnDisplay.Add(card);
                    solitaire.deck.Remove(card);
                    newTopCard.GetComponent<Solitaire_Selectable>().TurnOnCardFace();
                    newTopCard.GetComponent<Solitaire_Selectable>().inDeckPile = true;
                }
                solitaire.deckLocation++;
                if (solitaire.deckLocation == solitaire.trips)
                {
                    solitaire.deckPlaceHolder.gameObject.SetActive(false);
                }
            }
            else
            {
                solitaire.deckPlaceHolder.gameObject.SetActive(true);
                List<Transform> tmp = new List<Transform>();
                Solitaire_ManagerPoint.Instance.point = Mathf.Max(0, Solitaire_ManagerPoint.Instance.point - 100);
                foreach (string stringDic in solitaire.mapDeck.Keys)
                {
                    offsetZOfList[stringDic] = solitaire.mapDeck[stringDic].transform.position.z;
                }
                foreach (Transform child in solitaire.deckButton.transform)
                {
                    if (child.CompareTag("Card"))
                    {
                        Vector3 newPos = new Vector3(solitaire.deckButton.transform.position.x, solitaire.deckButton.transform.position.y, child.position.z);
                        child.DOMove(newPos, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            child.gameObject.SetActive(false);
                        });
                        tmp.Add(child);

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
            Solitaire_AudioManager.Instance.PlayUndoDeal();
            if (deckcurrent < tripsCurrent)
            {
                solitaire.deckPlaceHolder.gameObject.SetActive(true);
                solitaire.deckLocation = deckcurrent;
                if (deckcurrent == solitaire.deckLocation - 1)
                {
                    solitaire.deckPlaceHolder.gameObject.SetActive(true);
                }
                foreach (Transform child in solitaire.deckButton.transform)
                {
                    if (child.CompareTag("Card") && listCardAddCurrent.Contains(child.name))
                    {
                        solitaire.deck.Add(child.name);
                        solitaire.tripsOnDisplay.Remove(child.name);
                        transAdd.Add(child);
                    }
                }
                foreach (Transform child in transAdd)
                {
                    child.transform.parent = solitaire.pivotDeck.transform;
                    Vector3 newPos = new Vector3(solitaire.pivotDeck.transform.position.x + 0.45f, child.position.y, child.position.z);
                    child.DOMove(newPos, 0.25f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        child.gameObject.SetActive(false);
                    });
                }
                int count = solitaire.deckButton.transform.childCount;
                float lastChildPos = -0.7f;
                for (int i = count - 3; i < count && count >= 3; i++)
                {
                    Transform child = solitaire.deckButton.transform.GetChild(i);
                    if (child.gameObject.CompareTag("Card"))
                    {
                        Vector3 newPos = new Vector3(solitaire.deckButton.transform.position.x + lastChildPos, solitaire.deckButton.transform.position.y, child.transform.position.z);
                        child.DOMove(newPos, 0.25f).SetEase(Ease.InOutSine);
                        lastChildPos -= 0.25f;
                        solitaire.discardPile.Remove(child.name);
                    }
                }
            }
            else
            {
                Solitaire_ManagerPoint.Instance.point = currentPoint;
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
                solitaire.trips = tripsCurrent;
                int count = 0;
                int countLoop = onDisplayPreviousClear.Count / 3;
                foreach (string tmpString in onDisplayPreviousClear)
                {
                    solitaire.mapDeck[tmpString].transform.position = new Vector3(solitaire.mapDeck[tmpString].transform.position.x, solitaire.mapDeck[tmpString].transform.position.y, offsetZOfList[tmpString]);
                }
                int lech = onDisplayPreviousClear.Count - countLoop * 3;
                while (count < countLoop)
                {

                    foreach (Transform child in solitaire.deckButton.transform)
                    {
                        if (child.CompareTag("Card"))
                        {
                            Vector3 newPos = new Vector3(solitaire.pivotDeck.transform.position.x - 0.25f, child.position.y, child.position.z);
                            child.DOMove(newPos, 0.25f).SetEase(Ease.InOutSine);
                        }
                    }
                    float xOffset = 0.7f;
                    for (int j = count * 3; j <= count * 3 + 2; j++)
                    {
                        GameObject newTopCard = solitaire.mapDeck[onDisplayPreviousClear[j]];
                        newTopCard.SetActive(true);
                        Vector3 newPos = new Vector3(solitaire.deckButton.transform.position.x - xOffset, solitaire.deckButton.transform.position.y, newTopCard.transform.position.z);
                        newTopCard.transform.parent = solitaire.deckButton.transform;
                        newTopCard.transform.DOMove(newPos, 0.25f).SetEase(Ease.InOutSine);
                        xOffset = xOffset + 0.25f;
                    }
                    count += 1;
                }
                if (lech > 0)
                {
                    DOVirtual.DelayedCall(0.255f, () =>
                    {
                        float posChildLastX = solitaire.deckButton.transform.GetChild(solitaire.deckButton.transform.childCount - 1).localPosition.x;
                        if (lech + solitaire.deckButton.transform.childCount > 5)
                        {
                            for (int i = solitaire.deckButton.transform.childCount - lech - 1; i <= solitaire.deckButton.transform.childCount - 1; i++)
                            {
                                Transform child = solitaire.deckButton.transform.GetChild(i);
                                if (child.CompareTag("Card"))
                                {

                                    float minPos = Mathf.Min(-0.7f, solitaire.deckButton.transform.GetChild(i).transform.localPosition.x + lech * 0.25f);
                                    Vector3 newPos = new Vector3(solitaire.deckButton.transform.position.x + minPos, child.position.y, child.position.z);
                                    posChildLastX = minPos;
                                    child.DOMove(newPos, 0.25f).SetEase(Ease.InOutSine);
                                }
                            }
                        }
                        float tmp = posChildLastX - 0.25 * lech >= -1.3f ? posChildLastX : -0.45f;
                        tmp -= 0.25f;
                        for (int j = onDisplayPreviousClear.Count - lech; j < onDisplayPreviousClear.Count; j++)
                        {
                            GameObject newTopCard = solitaire.mapDeck[onDisplayPreviousClear[j]];
                            newTopCard.SetActive(true);
                            Vector3 newPos = new Vector3(solitaire.deckButton.transform.position.x + tmp, solitaire.deckButton.transform.position.y, newTopCard.transform.position.z);
                            newTopCard.transform.parent = solitaire.deckButton.transform;
                            newTopCard.transform.DOMove(newPos, 0.25f).SetEase(Ease.InOutSine);
                            tmp -= 0.25f;
                        }
                    });

                }

            }
        }
    }

}
