using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class InputController : MonoBehaviour
{
    private InputSystem_Actions _inputSystemActions;
    private InputActionManager _inputActionManager;
    private Vector2 _direction;
    public Action JumpHandler { get;  set; } 
    public Action AttackLightHandler { get;  set; }
    public Action AttackFootHandler { get; set; }
    public Action SquatHandler { get; set; }
    public Action DashHandler { get; set; }
    
    

    
    public EventHandler<CharacterStateEventArgs> CharacterStateHandler { get; set; }

    public class CharacterStateEventArgs : EventArgs
    {
        public CharacterState State { get; set; }

        public CharacterStateEventArgs(CharacterState state)
        {
            State = state;
        }
    }
   
    
    private void OnEnable()
    {
        _inputSystemActions = new InputSystem_Actions();
        _inputActionManager = new InputActionManager();
        _inputSystemActions.Enable();
        _inputActionManager.Subscribe( _inputSystemActions.Player.Move,OnMove);
        _inputActionManager.Subscribe( _inputSystemActions.Player.Jump,OnJump);
        _inputActionManager.Subscribe( _inputSystemActions.Player.Squat,OnSquat);
        _inputActionManager.Subscribe(_inputSystemActions.Player.LightAttack,OnAttackLight);
        _inputActionManager.Subscribe(_inputSystemActions.Player.FootAttack,OnAttackFoot);
        _inputActionManager.Subscribe(_inputSystemActions.Player.Dash,OnDash);
        
    }
    
    private void OnDisable()
    {
        _inputActionManager.OnDisableAll();
        _inputSystemActions.Disable();
    }
    private void OnAttackFoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AttackFootHandler?.Invoke();
        }
     
    }
    private void OnDash(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            CharacterStateHandler?.Invoke(this, new CharacterStateEventArgs(CharacterState.Dash));
            DashHandler?.Invoke();
        }
        else if (obj.canceled)
        {
            CharacterState newState =  _direction.x != 0 ? CharacterState.Move : CharacterState.Idle;
            CharacterStateHandler?.Invoke(this, new CharacterStateEventArgs(newState));
        }
    }
    private void OnAttackLight(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AttackLightHandler?.Invoke();
        }
     
                
    }

    
    private void OnSquat(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            CharacterStateHandler?.Invoke(this,new CharacterStateEventArgs(CharacterState.Crouching));
          
        }
        else if (obj.canceled)
        {
            CharacterState newState = _direction.x != 0 ? CharacterState.Move : CharacterState.Idle;
            CharacterStateHandler?.Invoke(this,new CharacterStateEventArgs(newState));
        }
        SquatHandler?.Invoke();
        
    }


    private void OnMove(InputAction.CallbackContext obj)
    {
        _direction = obj.performed ? obj.ReadValue<Vector2>() : Vector2.zero;

        CharacterState newState = _direction.x != 0 ? CharacterState.Move : CharacterState.Idle;
        
        CharacterStateHandler?.Invoke(this,new CharacterStateEventArgs(newState));
    }
    private void OnJump(InputAction.CallbackContext obj)
    {
        if (obj.performed)
        {
            CharacterStateHandler?.Invoke(this,new CharacterStateEventArgs(CharacterState.Jumping));
            JumpHandler?.Invoke();
        }
    }
    
    public Vector2 GetDirection() => _direction;

}
