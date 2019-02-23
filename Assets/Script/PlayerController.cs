using System.Collections;
using System.Collections.Generic;
using UnityEngine;




enum PlayerState
{
    JUMP,
    RUN,
    IDLE
}

delegate void Action();
delegate void 행동(bool isDown);

public class PlayerController : MonoBehaviour
{
    public Vector2 speed;


    new Rigidbody2D rigidbody;
    new BoxCollider2D collider;
    SpriteRenderer spriteRenderer;
    Animator animator;

    Vector2 moveForce;

    //레이어
    public LayerMask groundLayer;

    //플레이어 상태
    PlayerState state = PlayerState.IDLE;
    //이동
    Vector2 moveDirection;

    Dictionary<KeyCode, 행동> KeyDownAction;
    Dictionary<KeyCode, 행동> KeyUpAction;

    //Dictionary<string, Action> KeyDownAction;
    //Dictionary<KeyCode, Action> KeyUpAction;

    private void Awake()
    {

        KeyDownAction = new Dictionary<KeyCode, 행동>();
        KeyUpAction = new Dictionary<KeyCode, 행동>();

        KeyDownAction[KeyCode.Space] = Jump;
        KeyDownAction[KeyCode.LeftArrow] = RunLeft;
        KeyDownAction[KeyCode.RightArrow] = RunRight;


        KeyUpAction[KeyCode.LeftArrow] = RunLeft;
        KeyUpAction[KeyCode.RightArrow] = RunRight;

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


        transform.Translate(moveForce * Time.deltaTime);
    }

    private void InputHandle()
    {
       if(Input.GetKeyDown(KeyCode.Space))
       {
            KeyDownAction[KeyCode.Space](true);
       }
       if (Input.GetKeyDown(KeyCode.LeftArrow))
       {
           KeyDownAction[KeyCode.LeftArrow](true);
       }
       if (Input.GetKeyDown(KeyCode.RightArrow))
       {
           KeyDownAction[KeyCode.RightArrow](true);
       }

       if(Input.GetKeyUp(KeyCode.RightArrow))
        {
            KeyUpAction[KeyCode.RightArrow](false);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            KeyUpAction[KeyCode.LeftArrow](false);
        }



    }

    void Jump(bool isDown)
    {
        Vector2 jumpForce = new Vector2(0, speed.y);
        rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);
        state = PlayerState.JUMP;
        animator.SetBool("isJump", true);

    }
    void RunLeft(bool isDown)
    {
        if (isDown)
        {
            moveForce = new Vector2(-speed.x, 0);
            spriteRenderer.flipX = true;
            animator.SetFloat("runSpeed", Mathf.Abs(moveForce.x));
        }
        else
        {
            moveForce = new Vector2(0, 0);
            animator.SetFloat("runSpeed", Mathf.Abs(moveForce.x));
        }
    }
    void RunRight(bool isDown)
    {
        if (isDown)
        {
            moveForce = new Vector2(speed.x, 0);
            spriteRenderer.flipX = false;
            animator.SetFloat("runSpeed", Mathf.Abs(moveForce.x));
        }
        else
        {
            moveForce = new Vector2(0, 0);
            animator.SetFloat("runSpeed", Mathf.Abs(moveForce.x));
        }
    }



    bool IsGrounded()
    {
        return collider.IsTouchingLayers(groundLayer)&&rigidbody.velocity.y<=0;
    }
}
