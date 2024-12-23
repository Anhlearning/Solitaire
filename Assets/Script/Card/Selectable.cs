using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public bool cardFace = false;
    public bool isTop = false;
    public string suit;
    public int value;
    public int row;
    public bool inDeckPile = false;

    private string valueString;
    void Start()
    {
        if (gameObject.CompareTag("Card"))
        {
            suit = transform.name[0].ToString();
            for(int i = 1; i < name.Length; i++)
            {
                char c= name[i];
                valueString += c.ToString();
            }
            if (valueString == "A")
            {
                value = 1;
            }
            if (valueString == "2")
            {
                value = 2;
            }
            if (valueString == "3")
            {
                value = 3;
            }
            if (valueString == "4")
            {
                value = 4;
            }
            if (valueString == "5")
            {
                value = 5;
            }
            if (valueString == "6")
            {
                value = 6;
            }
            if (valueString == "7")
            {
                value = 7;
            }
            if (valueString == "8")
            {
                value = 8;
            }
            if (valueString == "9")
            {
                value = 9;
            }
            if (valueString == "10")
            {
                value = 10;
            }
            if (valueString == "J")
            {
                value = 11;
            }
            if (valueString == "Q")
            {
                value = 12;
            }
            if (valueString == "K")
            {
                value = 13;
            }

        }
    }
}
