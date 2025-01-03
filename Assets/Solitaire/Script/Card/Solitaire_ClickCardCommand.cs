using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Solitaire_Manager.AudioManager;
using Solitaire_Manager.PointManger;
using Solitaire_Card;

namespace Solitaire_Manager.UndoManager
{
    public class Solitaire_ClickCardCommand : Solitaire_IAction
    {
        Solitaire_Selectable selected;
        Solitaire solitaire;
        public Solitaire_ClickCardCommand(Solitaire_Selectable selectable, Solitaire solitaire)
        {
            this.selected = selectable;
            this.solitaire = solitaire;
        }
        public void ExecuteCommand()
        {
            solitaire.CountCardFace++;
            Debug.LogError(solitaire.CountCardFace);
            Solitaire_AudioManager.Instance.TurnOnCardFace();
            Solitaire_ManagerPoint.Instance.point += 5;
            selected.transform.DOScale(new Vector3(0.02f, 0.02f, 0.02f), 0.15f)
            .OnComplete(() =>
            {
                selected.transform.DORotate(new Vector3(0, 90, 0), 0.15f, RotateMode.FastBeyond360)
                    .OnComplete(() =>
                    {
                        selected.TurnOnCardFace();
                        selected.transform.DORotate(new Vector3(0, 0, 0), 0.15f, RotateMode.FastBeyond360)
                            .OnComplete(() =>
                            {

                                selected.transform.DOScale(new Vector3(0.017f, 0.017f, 0.016f), 0.15f);

                            });
                    });
            });
        }

        public void UndoCommand()
        {
            Solitaire_AudioManager.Instance.TurnOnCardFace();
            solitaire.CountCardFace--;
            Solitaire_ManagerPoint.Instance.point -= 5;
            Debug.LogWarning(solitaire.CountCardFace);
            selected.transform.DOScale(new Vector3(0.02f, 0.02f, 0.02f), 0.15f)
            .OnComplete(() =>
            {
                selected.transform.DORotate(new Vector3(0, 90, 0), 0.3f, RotateMode.FastBeyond360)
                    .OnComplete(() =>
                    {
                        selected.TurnOffCardFace();
                        selected.transform.DORotate(new Vector3(0, 0, 0), 0.3f, RotateMode.FastBeyond360)
                            .OnComplete(() =>
                            {

                                selected.transform.DOScale(new Vector3(0.017f, 0.017f, 0.016f), 0.15f);
                            });
                    });
            });
        }
    }
}

