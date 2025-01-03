using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Solitaire_Manager.UndoManager
{
    public class Solitaire_UndoManager : MonoBehaviour
    {
        public Stack<Solitaire_IAction> historyStack = new Stack<Solitaire_IAction>();
        private bool isCommanded = false;
        public float countDownTimeUndo;

        public void ExecuteCommand(Solitaire_IAction action)
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

}
