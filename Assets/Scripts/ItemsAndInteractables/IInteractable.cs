using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    // interface that is used by all the other interactable objects
    // All contain the InteractLogic method but the method is defined in the individual scrips 
    void InteractLogic();
}
