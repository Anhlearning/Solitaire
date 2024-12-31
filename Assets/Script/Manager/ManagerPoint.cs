using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerPoint : MonoBehaviour
{
    public Selectable[] topStacks;

    public static ManagerPoint Instance {private set;get;}
    private void Awake()
    {
        if( Instance == null)
        {
            Instance = this;   
        }
    }
    private void Update()
    {
        if(HasWon()) {
            Win();
        }
    }

    public bool HasWon()

    {
        int i = 0;
        foreach (Selectable topstack in topStacks)
        {
            i += topstack.value;
        }
        if (i >= 52)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Win()
    {
        Debug.Log("WWINNN");
    }
}
