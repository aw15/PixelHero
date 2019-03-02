using System.Collections;
using System.Collections.Generic;
using UnityEngine;




enum PlayerState
{
    JUMP,
    LEFTRUN,
    RIGHTRUN,
    GROUNDATTACK,
    AIRATTACK
}


delegate void 행동(bool isDown);

public class PlayerController : MonoBehaviour
{
    public Vector2 speed;


    new Rigidbody2D rigidbody;
    BoxCollider2D bodyCollider;
    CapsuleCollider2D weaponCollider;

    SpriteRenderer spriteRenderer;
    Animator animator;
    Vector2 moveForce;

    //레이어
    public LayerMask groundLayer;

    //데미지 관련 변수들
    public float hp;
    public int damage;
    public float knockbackAmount;

    //플레이어 상태
    Dictionary<PlayerState, bool> state;

   

    //INPUT
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
        state[PlayerState.GROUNDATTACK] = false;
        state[PlayerState.AIRATTACK] = false;

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Enemy")
        {
            var player = collision.collider.GetComponent<EnemyController>();
            player.GetDamaged(damage, transform.position);
        }
    }

    private void InputHandle()
    {
        //이동은 GetAxis를 이용해서 처리
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput > 0)
            KeyDownAction[KeyCode.RightArrow](true);
        else if(moveInput<0)
            KeyDownAction[KeyCode.LeftArrow](true);


        foreach (var data in KeyDownAction)
        {
            if (Input.GetKeyUp(data.Key))
                KeyDownAction[data.Key](true);
        }

        foreach (var data in KeyUpAction)
        {
           if(Input.GetKeyUp(data.Key))
              KeyUpAction[data.Key](false);
        }
    }

    void Jump(bool isDown)
    {
        if (!state[PlayerState.JUMP])
        {
            rigidbody.AddForce(new Vector2(0,speed.y), ForceMode2D.Impulse);

            state[PlayerState.JUMP] = true;
            animator.SetBool("isJump", state[PlayerState.JUMP]);
        }
    }
    void RunLeft(bool isDown)
    {
        if (isDown)
        {
            moveForce.x = -speed.x;
            spriteRenderer.flipX = true;
            animator.SetBool("isRun",true);

            state[PlayerState.LEFTRUN] = true;
        }
        else
        {
            moveForce.x = 0;
            animator.SetBool("isRun", false);

            state[PlayerState.LEFTRUN] = false;
        }
    }

    void RunRight(bool isDown)
    {
        if (isDown)
        {
            moveForce.x = speed.x;
            spriteRenderer.flipX = false;
            animator.SetBool("isRun", true);

            state[PlayerState.RIGHTRUN] = true;
        }
        else
        {
            moveForce.x = 0;
            animator.SetBool("isRun", false);
          
            state[PlayerState.RIGHTRUN] = false;
        }
    }


    //땅에 있는지 확인.
    bool IsGrounded()
    {
        return bodyCollider.IsTouchingLayers(groundLayer)&&rigidbody.velocity.y<=0;
    }

    //상호작용 함수들
    public void GetDamaged(int damage,Vector3 position)
    {
        rigidbody.AddForce((transform.position - position).normalized * knockbackAmount, ForceMode2D.Impulse);
        hp -= damage;
    }
}
