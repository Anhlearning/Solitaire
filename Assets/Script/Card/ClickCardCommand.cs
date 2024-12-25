using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickCardCommand : IAction
{
    Selectable selected;

    public ClickCardCommand(Selectable selectable)
    {
        this.selected = selectable; 
    }
    public void ExecuteCommand()
    {
        selected.cardFace = true;
    }

    public void UndoCommand()
    {
        selected.cardFace = false;
    }
}
