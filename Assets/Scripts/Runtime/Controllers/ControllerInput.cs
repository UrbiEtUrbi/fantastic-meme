using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerInput : GenericSingleton<ControllerInput>
{
    [HideInInspector]
    public UnityEvent<float> Horizontal = new UnityEvent<float>();
    [HideInInspector]
    public UnityEvent<float> Vertical = new UnityEvent<float>();

    [HideInInspector]
    public UnityEvent<bool> Jump = new UnityEvent<bool>();
    [HideInInspector]
    public UnityEvent Attack = new UnityEvent();

    [HideInInspector]
    public UnityEvent Cast = new UnityEvent();


    void OnAttack(InputValue inputValue)
    {
        if (inputValue.Get<float>() > 0)
        {
            if (ControllerGame.Initialized && ControllerGame.Instance.AcceptInput)
            {
                Attack.Invoke();
            }
        }
    }

    void OnCast(InputValue inputValue)
    {
        if (inputValue.Get<float>() > 0)
        {
            if (ControllerGame.Initialized && ControllerGame.Instance.AcceptInput)
            {
                Cast.Invoke();
            }
        }
    }

    void OnJump(InputValue inputValue)
    {
        if (!ControllerGame.Initialized || ControllerGame.Instance.AcceptInput)
        {
            Jump.Invoke(inputValue.Get<float>() > 0);
        }
    }

    void OnHorizontal(InputValue inputValue)
    {
        if (!ControllerGame.Initialized || ControllerGame.Instance.AcceptInput)
        {
            var horizontalInputRaw = inputValue.Get<float>();
            Horizontal.Invoke(horizontalInputRaw);
        }

    }

    void OnVertical(InputValue inputValue)
    {
        if (ControllerGame.Initialized && ControllerGame.Instance.AcceptInput)
        {
            var vertInputRaw = inputValue.Get<float>();
            Vertical.Invoke(vertInputRaw);
        }

    }

    
   
}
