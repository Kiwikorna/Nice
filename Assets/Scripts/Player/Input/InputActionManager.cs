using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Windows.Input;

public class InputActionManager 
{
    private Dictionary<InputAction,List<Action<InputAction.CallbackContext>>> _actionHandlers = new Dictionary<InputAction, List<Action<InputAction.CallbackContext>>>();
    public void Subscribe(InputAction action, Action<InputAction.CallbackContext> handler)
    {
        if (!_actionHandlers.ContainsKey(action))
        {
            _actionHandlers[action] = new List<Action<InputAction.CallbackContext>>();
            
            action.performed += InvokerHandler;
            action.canceled += InvokerHandler;

            if (!_actionHandlers[action].Contains(handler))
            {
                _actionHandlers[action].Add(handler);
            }
        }
    }

    /*public void Unsubscribe(InputAction action, Action<InputAction.CallbackContext> handler)
    {
        if(_actionHandlers.TryGetValue(action, out var handlers ))
        {
            handlers.Remove(handler);

            if (handlers.Count == 0)
            {
                action.performed -= InvokerHandler;
                action.canceled -= InvokerHandler;
                
               _actionHandlers.Remove(action);
            }
        }
    }*/
    private void InvokerHandler( InputAction.CallbackContext context)
    {
        if (_actionHandlers.TryGetValue(context.action, out var handlers))
        {
            foreach (var handler in handlers)
            {
                handler?.Invoke(context);
            }
        }
    }

    public void OnDisableAll()
    {
        foreach (var action in _actionHandlers.Keys)
        {
            action.Disable();
        }
    }
    
    public void OnEnableAll()
    {
        foreach (var action in _actionHandlers.Keys)
        {
            action.Enable();
        }
    }
}
