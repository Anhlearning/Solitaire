using Solitaire_Card;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire_Manager.PointManger
{
    public class Solitaire_ManagerPoint : MonoBehaviour
    {
        public Solitaire_Selectable[] topStacks;
        public static Solitaire_ManagerPoint Instance { private set; get; }
        public int point = 0;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        public bool HasWon()

        {
            int i = 0;
            foreach (Solitaire_Selectable topstack in topStacks)
            {
                i += topstack.value;
            }
            if (i >= 52)
            {
                Win();
                return true;
            }
            else
            {
                return false;
            }
        }
        public void Win()
        {
            Debug.LogError("WWINNN");
        }
    }

}
