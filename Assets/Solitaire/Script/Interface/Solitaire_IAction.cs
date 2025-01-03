using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Solitaire_IAction 
{
    void ExecuteCommand();
    void UndoCommand();
}
