using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface Node
{
   bool Invoke();
}



enum PlayerState
{
    JUMP,
    RUN,
    IDLE
}


delegate void Action();

public class PlayerController : MonoBehaviour
{
    new Rigidbody2D rigidbody;
    new BoxCollider2D collider;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public Vector2 speed;

    //레이어
    public LayerMask groundLayer;

    //플레이어 상태
    PlayerState state = PlayerState.IDLE;
    //이동
    Vector2 moveDirection;

    Dictionary<string, Action> KeyDownAction;
    Dictionary<KeyCode, Action> KeyUpAction;

    private void Awake()
    {
        KeyDownAction = new Dictionary<string, Action>();
        KeyUpAction = new Dictionary<KeyCode, Action>();

        KeyDownAction[" "] = Jump;
        KeyDownAction["a"] = MoveLeft;
        KeyDownAction["d"] = MoveRight;



        KeyUpAction[KeyCode.A] = Idle;
        KeyUpAction[KeyCode.D] = Idle;


    }


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
    }
 
    // Update is called once per frame
    void Update()
    {
        InputHandle();
        HandleState();
    }

    private void HandleState()
    {
        if(state ==  PlayerState.JUMP)
        {
            if (IsGrounded())
            {
                state = PlayerState.IDLE;
                animator.SetBool("isJump", false);
            }
        }
        else if(state == PlayerState.RUN)
        {
            Vector2 movement = moveDirection * Time.deltaTime * speed.x;
            Debug.Log(movement);
            animator.SetFloat("runSpeed", Mathf.Abs(movement.x));
            transform.Translate(movement);
        }
        else if(state == PlayerState.IDLE)
        {
            animator.SetFloat("runSpeed", 0);
        }
    }
    private void InputHandle()
    {
        if(Input.anyKeyDown&&KeyDownAction.ContainsKey(Input.inputString))
        {
            KeyDownAction[Input.inputString]();
        }
        foreach (var command in KeyUpAction)
        {
            if (Input.GetKeyUp(command.Key))
            {
                command.Value();
            }
        }
    }

    public void Jump()
    {
        if (state != PlayerState.JUMP)
        {
            Vector2 jumpForce = new Vector2(0, speed.y);
            rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
            state = PlayerState.JUMP;
            animator.SetBool("isJump", true);
        }
    }
    public void Idle()
    {
        state = PlayerState.IDLE;
    }
    public void MoveLeft()
    {
        state = PlayerState.RUN;
        spriteRenderer.flipX = true;
        moveDirection = Vector2.left;
    }
    public void MoveRight()
    {
        state = PlayerState.RUN;
        spriteRenderer.flipX = false;
        moveDirection = Vector2.right;
    }


    bool IsGrounded()
    {
        return collider.IsTouchingLayers(groundLayer)&&rigidbody.velocity.y<=0;
    }
}



