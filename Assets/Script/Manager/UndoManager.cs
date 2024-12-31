using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public Stack<IAction> historyStack= new Stack<IAction>();
    private bool isCommanded = false;
    public float countDownTimeUndo;
   
    public void ExecuteCommand(IAction action)
    {
        action.ExecuteCommand();
        historyStack.Push(action);  
    }

    public void UndoCommand()
    {
        if (!isCommanded)
        {
            isCommanded = true;
            if (historyStack.Count > 0)
            {
                historyStack.Pop().UndoCommand();
            }
            DOVirtual.DelayedCall(countDownTimeUndo, () =>
            {
                isCommanded = false;
            });
        }
    }

}
