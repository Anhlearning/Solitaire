using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Solitaire_Manager.AudioManager
{
    public class Solitaire_AudioManager : MonoBehaviour
    {
        [SerializeField] AudioClip CountDownClip;
        [SerializeField] AudioClip Fischio;
        [SerializeField] AudioClip Deal;
        [SerializeField] AudioClip pageFlip;
        [SerializeField] AudioClip quickTrans;
        [SerializeField] AudioClip OnTop;
        [SerializeField] AudioClip Victoria;
        [SerializeField] AudioClip startDeal;
        [SerializeField] AudioClip Undo;
        [SerializeField] AudioClip IllegalClip;
        private AudioSource audioSource;
        public static Solitaire_AudioManager Instance;
        private void Awake()
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayCountDown()
        {
            audioSource.PlayOneShot(CountDownClip);
        }
        public void PlayStart()
        {
            audioSource.PlayOneShot(Fischio);
        }
        public void PlayDealDeck()
        {
            audioSource.PlayOneShot(Deal);
        }
        public void PlayCardMove()
        {
            audioSource.PlayOneShot(quickTrans, 0.5f);
        }
        public void PlayCardMoveHome()
        {
            audioSource.PlayOneShot(OnTop, 0.5f);
        }
        public void PlayVictoria()
        {
            audioSource.PlayOneShot(Victoria);
        }
        public void PlayStartDeal()
        {
            audioSource.PlayOneShot(startDeal);
        }
        public void PlayUndoDeal()
        {
            audioSource.PlayOneShot(Undo);
        }
        public void IllegalMove()
        {
            audioSource.PlayOneShot(IllegalClip, 0.5f);
        }
        public void StopAudio()
        {
            audioSource.Stop();
        }
        public void TurnOnCardFace()
        {
            audioSource.PlayOneShot(pageFlip, 0.5f);
        }
    }

}
