using System.Collections;
using System.Collections.Generic;
using UnityEngine;




enum PlayerState
{
    JUMP,
    LEFTRUN,
    RIGHTRUN
}


delegate void 행동(bool isDown);

public class PlayerController : MonoBehaviour
{
    public Vector2 speed;


    new Rigidbody2D rigidbody;
    new BoxCollider2D bodyCollider;
    new CapsuleCollider2D weaponCollider;

    SpriteRenderer spriteRenderer;
    Animator animator;
    Vector2 moveForce;

    //레이어
    public LayerMask groundLayer;

    //플레이어 상태
    Dictionary<PlayerState, bool> state;

   

    //이동
    Vector2 moveDirection;

    Dictionary<KeyCode, 행동> KeyDownAction;
    Dictionary<KeyCode, 행동> KeyUpAction;

    private void Awake()
    {

        KeyDownAction = new Dictionary<KeyCode, 행동>();
        KeyUpAction = new Dictionary<KeyCode, 행동>();

        KeyDownAction[KeyCode.Space] = Jump;
        KeyDownAction[KeyCode.LeftArrow] = RunLeft;
        KeyDownAction[KeyCode.RightArrow] = RunRight;


        KeyUpAction[KeyCode.LeftArrow] = RunLeft;
        KeyUpAction[KeyCode.RightArrow] = RunRight;

        state = new Dictionary<PlayerState, bool>();
        

        state[PlayerState.RIGHTRUN] = false;
        state[PlayerState.LEFTRUN] = false;
        state[PlayerState.JUMP] = false;



    }


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<BoxCollider2D>();
        weaponCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        InputHandle();
 
        transform.Translate(moveForce * Time.deltaTime);

    

        if (IsGrounded() && state[PlayerState.JUMP])
        {
            state[PlayerState.JUMP] = false;
            animator.SetBool("isJump", state[PlayerState.JUMP]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {


        }
    }

    private void InputHandle()
    {
        if (Input.GetKeyDown(KeyCode.Space))
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




        if (Input.GetKeyUp(KeyCode.RightArrow))
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
        if (!state[PlayerState.JUMP])
        {
            Vector2 jumpForce = new Vector2(0, speed.y);
            rigidbody.AddForce(jumpForce, ForceMode2D.Impulse);

            state[PlayerState.JUMP] = true;
            animator.SetBool("isJump", state[PlayerState.JUMP]);
        }
    }
    void RunLeft(bool isDown)
    {
        if (isDown)
        {
            moveForce = new Vector2(-speed.x, 0);
            spriteRenderer.flipX = true;
            animator.SetFloat("runSpeed", Mathf.Abs(moveForce.x));

            state[PlayerState.LEFTRUN] = true;
        }
        else
        {
            if (!state[PlayerState.RIGHTRUN])
            {
                moveForce = new Vector2(0, 0);
                animator.SetFloat("runSpeed", Mathf.Abs(moveForce.x));
            }
            state[PlayerState.LEFTRUN] = false;
        }
    }

    void RunRight(bool isDown)
    {
        if (isDown)
        {
            moveForce = new Vector2(speed.x, 0);
            spriteRenderer.flipX = false;
            animator.SetFloat("runSpeed", Mathf.Abs(moveForce.x));

            state[PlayerState.RIGHTRUN] = true;
        }
        else
        {
            if (!state[PlayerState.LEFTRUN])
            {
                moveForce = new Vector2(0, 0);
                animator.SetFloat("runSpeed", Mathf.Abs(moveForce.x));
            }
            state[PlayerState.RIGHTRUN] = false;
        }
    }



    bool IsGrounded()
    {
        return bodyCollider.IsTouchingLayers(groundLayer)&&rigidbody.velocity.y<=0;
    }
}
