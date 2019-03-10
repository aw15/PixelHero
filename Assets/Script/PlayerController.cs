using System.Collections;
using System.Collections.Generic;
using UnityEngine;




enum PlayerState
{
    JUMP,
    RUN,
    BASIC_GROUNDATTACK,
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
        KeyDownAction[KeyCode.Q] = BasicGroundAttack;




        state = new Dictionary<PlayerState, bool>();
        

        state[PlayerState.RUN] = false;
        state[PlayerState.JUMP] = false;
        state[PlayerState.BASIC_GROUNDATTACK] = false;
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
        

        if (IsGrounded() && state[PlayerState.JUMP])
        {
            state[PlayerState.JUMP] = false;
            animator.SetBool("isJump", state[PlayerState.JUMP]);
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("BasicGroundAttack") && !AnimatorIsPlaying())
        {
            state[PlayerState.BASIC_GROUNDATTACK] = false;
            animator.SetBool("isBasicAttack", state[PlayerState.BASIC_GROUNDATTACK]);
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

        MoveInput();



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
        if (!state[PlayerState.JUMP] && isDown)
        {
            rigidbody.AddForce(new Vector2(0,speed.y), ForceMode2D.Impulse);

            state[PlayerState.JUMP] = true;
            animator.SetBool("isJump", state[PlayerState.JUMP]);
        }
    }

    void BasicGroundAttack(bool isDown)
    {
        if (!state[PlayerState.JUMP] && isDown)
        {
            state[PlayerState.BASIC_GROUNDATTACK] = true;
            animator.SetBool("isBasicAttack", state[PlayerState.BASIC_GROUNDATTACK]);
         
        }
    }


    void MoveInput()
    {
        //이동은 GetAxis를 이용해서 처리
        float moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput > 0)
        {
            moveForce.x = speed.x;
            spriteRenderer.flipX = false;
            animator.SetBool("isRun", true);

            state[PlayerState.RUN] = true;
        }
        else if (moveInput < 0)
        {
            moveForce.x = -speed.x;
            spriteRenderer.flipX = true;
            animator.SetBool("isRun", true);

            state[PlayerState.RUN] = true;
        }
        else
        {
            moveForce.x = 0;
            animator.SetBool("isRun", false);

            state[PlayerState.RUN] = false;
        }


        transform.Translate(moveForce * Time.deltaTime);
    }

    //땅에 있는지 확인.
    bool IsGrounded()
    {
        return bodyCollider.IsTouchingLayers(groundLayer)&&rigidbody.velocity.y<=0;
    }
    //애니메이션이 플레이 중인지 확인.
    bool AnimatorIsPlaying()
    {
        //return animator.GetCurrentAnimatorStateInfo(0).normalizedTime<0.95;

        return animator.GetCurrentAnimatorStateInfo(0).length >
       animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }


    //상호작용 함수들
    public void GetDamaged(int damage,Vector3 position)
    {
        rigidbody.AddForce((transform.position - position).normalized * knockbackAmount, ForceMode2D.Impulse);
        hp -= damage;
    }
}
