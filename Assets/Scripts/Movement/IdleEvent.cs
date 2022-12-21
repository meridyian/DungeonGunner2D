using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class IdleEvent : MonoBehaviour
{
    public event Action<IdleEvent> OnIdle;
    
    //wrapper event that calls the idle 
    public void CallIdleEvent()
    {
        OnIdle?.Invoke(this);
    }

}
