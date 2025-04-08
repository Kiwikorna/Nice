using System;
using System.Collections;
using UnityEngine;


public enum CharacterState {Idle,Jumping, Crouching,Move,Dash,AttackLight,AttackFoot}
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputController inputController;
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float dashSpeed = 2.0f;
    [SerializeField] private float dashDuration = 0.2f;
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2.0f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistance = 0.2f;
    [SerializeField] private int maxJumps = 2;
    [Header("Gravity")]
    [SerializeField] private float gravity = -9.81f;
    [Header("SetDown")]
    [SerializeField] private float crouchingOffset;
    [SerializeField] private float crouchingHeight;

    public CharacterState CurrentState { get;  set; } = CharacterState.Idle;
    
    private Rigidbody2D _rb;
    private BoxCollider2D _collider;
    private Vector2 _standingOffset;
    private Vector2 _standingSizeCollider;

   
    private bool _isGrounded;
    private int _remainingJumps;
    private int _currentSpeed;
    private void OnEnable()
    {
        inputController.JumpHandler += Jump;
        inputController.SquatHandler += SetDown;
        inputController.CharacterStateHandler += CharacterStateHandler;
        inputController.DashHandler += OnDash;
    }

   


    private void OnDisable()
    {
        inputController.JumpHandler -= Jump;
        inputController.SquatHandler -= SetDown;
        inputController.DashHandler -= OnDash;
        inputController.CharacterStateHandler -= CharacterStateHandler;
      
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
        _standingOffset = _collider.offset;
        _standingSizeCollider = _collider.size;
        _rb.freezeRotation = true;
        _remainingJumps = maxJumps;

    }

    private void Update()
    {
        Move();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
            Gravity();
    }
  
    private void Jump()
    {
        if (_remainingJumps > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, Mathf.Sqrt(2 * jumpHeight * -gravity));
            _remainingJumps--;
            
        }
    }
    private void OnDash()
    {
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        _rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashDuration);
        _rb.linearVelocity = Vector2.zero;
    }


    private void SetDown()
    {
       
        if (CurrentState == CharacterState.Crouching)
        {
            _collider.size = new Vector2(_standingSizeCollider.x, crouchingHeight);
            _collider.offset = new Vector2(_standingOffset.x, _standingOffset.y + crouchingOffset);

        }
        else
        {
               _collider.size = _standingSizeCollider;
               _collider.offset = _standingOffset;
        }

    }

    private void Move()
    {
         float targetSpeed = inputController.GetDirection().x * moveSpeed;
         int acceleration = 10;
         _currentSpeed = (int)Mathf.Lerp(_currentSpeed, targetSpeed, acceleration * Time.deltaTime);
         
         transform.Translate(Vector2.right * (_currentSpeed * Time.deltaTime));
        
     
    }
    private void CharacterStateHandler(object sender, InputController.CharacterStateEventArgs e)
    {
        switch (e.State)
        {
            case CharacterState.Idle:
                CurrentState = CharacterState.Idle;
                break;
            case CharacterState.Move:
                CurrentState = CharacterState.Move;
                break;
            case CharacterState.Jumping:
                CurrentState = CharacterState.Jumping;
                break;
            case CharacterState.Crouching:
                CurrentState = CharacterState.Crouching;
                break;
            case CharacterState.Dash:
                CurrentState = CharacterState.Dash;
                break;
            case CharacterState.AttackLight:
                CurrentState = CharacterState.AttackLight;
                break;
            case CharacterState.AttackFoot:
                CurrentState = CharacterState.AttackFoot;
                break;
        }
        Debug.Log(CurrentState.ToString());
    }
    
    private void Gravity()
    {
        _isGrounded = Physics2D.Raycast(_rb.position, Vector2.down, groundDistance, groundLayer);
        
        if (!_isGrounded)
        {
            _rb.AddForce(new Vector2(0f, gravity * _rb.mass));
           
        }
        else
        {
            _remainingJumps = maxJumps;

            if (CurrentState == CharacterState.Jumping)
            {
                CurrentState = inputController.GetDirection().x != 0 ? CharacterState.Move : CharacterState.Idle;
            }
        
        }
    }
    
    public bool IsGrounded() => _isGrounded;
    public InputController GetInputController() => inputController;

}
