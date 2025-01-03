using DG.Tweening;
using Solitaire_Manager;
using Solitaire_Manager.AudioManager;
using Solitaire_Manager.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Solitaire_UI
{
    public class Solitaire_CountDownUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        [SerializeField] private float durationScale;
        [SerializeField] private float amountScale;
        int previousTime;
        void Start()
        {
            Solitaire_GameManager.Instance.OnStateChanged += UI_CoundDown;
            Hide();
        }

        private void UI_CoundDown(object sender, EventArgs e)
        {
            if (Solitaire_GameManager.Instance.IsCountDown())
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        // Update is called once per frame
        void Update()
        {
            int countDownCurrent = Mathf.CeilToInt(Solitaire_GameManager.Instance.timeCountDown);
            if (previousTime != countDownCurrent)
            {
                textMeshProUGUI.text = countDownCurrent.ToString();
                Solitaire_AudioManager.Instance.PlayCountDown();
                previousTime = countDownCurrent;
                AnimationToText();

            }

        }
        private void AnimationToText()
        {
            Sequence mySequence = DOTween.Sequence();
            textMeshProUGUI.alpha = 1f;
            mySequence.Append(textMeshProUGUI.transform.DOPunchScale(Vector3.one * amountScale, durationScale).SetEase(Ease.InOutQuart));
            mySequence.Append(textMeshProUGUI.DOFade(0, 0.25f).SetEase(Ease.InOutQuart));
            mySequence.Play();
        }
        void Hide()
        {
            gameObject.SetActive(false);
        }
        void Show()
        {
            gameObject.SetActive(true);
        }
    }

}
