using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoManager : MonoBehaviour
{
    public Stack<IAction> historyStack= new Stack<IAction>();  
    
    public void ExecuteCommand(IAction action)
    {
        action.ExecuteCommand();
        historyStack.Push(action);  
    }

    public void UndoCommand()
    {
        if(historyStack.Count > 0)
        {
            historyStack.Pop().UndoCommand();
        }
    }

}
