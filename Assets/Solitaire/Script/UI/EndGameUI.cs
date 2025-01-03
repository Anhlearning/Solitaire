using DG.Tweening;
using Solitaire_Manager;
using Solitaire_Manager.Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Solitaire_UI
{
    public class Solitaire_EndGameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textWin;
        [SerializeField] private Image Bg;
        [SerializeField] private float duration;
        CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
        }
        void Start()
        {
            Solitaire_GameManager.Instance.OnStateChanged += EndGameUI_StateChanged;

            Hide();
        }

        private void EndGameUI_StateChanged(object sender, EventArgs e)
        {
            if (Solitaire_GameManager.Instance.IsEnd())
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
        private void Hide()
        {
            gameObject.SetActive(false);
        }
        void Show()
        {
            gameObject.SetActive(true);
            canvasGroup.alpha = 0f;
            rectTransform.transform.localPosition = new Vector3(0, -1000f, 0);
            rectTransform.DOAnchorPos(new Vector2(0f, 0f), duration, false).SetEase(Ease.InOutQuint);
            canvasGroup.DOFade(1, duration).OnComplete(() =>
            {
                //PenKick_AudioManager.Instance.PlayVictory();
            });
        }
    }


}
