using Solitaire_Manager.PointManger;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Solitaire_UI
{
    public class Solitaire_HomeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tx;
        void Update()
        {
            tx.text = Solitaire_ManagerPoint.Instance.point.ToString();
        }
    }
}

