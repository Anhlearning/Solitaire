using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ClickCardCommand : IAction
{
    Selectable selected;
    Solitaire solitaire;
    public ClickCardCommand(Selectable selectable, Solitaire solitaire)
    {
        this.selected = selectable;
        this.solitaire = solitaire; 
    }
    public void ExecuteCommand()
    {
        solitaire.CountCardFace++;
        
        Debug.LogWarning(solitaire.CountCardFace);
        selected.transform.DOScale(new Vector3(0.02f, 0.02f, 0.02f), 0.15f) 
        .OnComplete(() =>
        {
            selected.transform.DORotate(new Vector3(0, 90, 0), 0.15f, RotateMode.FastBeyond360) 
                .OnComplete(() =>
                {
                    selected.cardFace = true; 
                    selected.transform.DORotate(new Vector3(0, 0, 0), 0.15f, RotateMode.FastBeyond360) 
                        .OnComplete(() =>
                        {
                           
                            selected.transform.DOScale(new Vector3(0.017f,0.017f,0.016f), 0.15f);

                        });
                });
        });
    }

    public void UndoCommand()
    {
        solitaire.CountCardFace--;

        Debug.LogWarning(solitaire.CountCardFace);
        selected.transform.DOScale(new Vector3(0.02f, 0.02f, 0.02f), 0.15f) 
        .OnComplete(() =>
        {
            selected.transform.DORotate(new Vector3(0, 90, 0), 0.3f, RotateMode.FastBeyond360) 
                .OnComplete(() =>
                {
                    selected.cardFace = false; 
                    selected.transform.DORotate(new Vector3(0, 0, 0), 0.3f, RotateMode.FastBeyond360) 
                        .OnComplete(() =>
                        {
                           
                            selected.transform.DOScale(new Vector3(0.017f, 0.017f, 0.016f), 0.15f);
                        });
                });
        });
    }
}
