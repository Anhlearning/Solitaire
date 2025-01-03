using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Solitaire_Manager.AudioManager;

namespace Solitaire_Manager.Manager
{
    public enum StateSolitaire
    {
        WattingToStart,
        CountDownToStart,
        GamePlaying,
        GameWin,
    }
    public enum Option
    {
        easy = 1,
        normal = 2,
        hard = 3
    }
    public class Solitaire_GameManager : MonoBehaviour
    {
        public float timeCountDown;
        public static Solitaire_GameManager Instance;
        public event EventHandler OnStateChanged;
        public Option option;
        private StateSolitaire state;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
        private void Update()
        {
            switch (state)
            {
                case StateSolitaire.WattingToStart:
                    state = StateSolitaire.CountDownToStart;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                    break;
                case StateSolitaire.CountDownToStart:
                    timeCountDown -= Time.deltaTime;
                    if (timeCountDown < 0)
                    {
                        Solitaire_AudioManager.Instance.PlayStart();
                        state = StateSolitaire.GamePlaying;
                        OnStateChanged?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case StateSolitaire.GamePlaying:
                    break;
                case StateSolitaire.GameWin:

                    break;
            }
        }
        public bool IsCountDown()
        {
            return state == StateSolitaire.CountDownToStart;
        }
        public bool IsPlaying()
        {
            return state == StateSolitaire.GamePlaying;
        }
        public bool IsEnd()
        {
            return state == StateSolitaire.GameWin;
        }
        public void ChangedState(StateSolitaire state)
        {
            this.state = state;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
        public int GetOption()
        {
            return (int)option;
        }
    }
}

