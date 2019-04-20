using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;


enum PlayerState
{
    JUMP,
    RUN,
    BASIC_GROUNDATTACK,
    AIRATTACK,
    DEAD
}


delegate void KeyAction(bool isDown);
delegate void StateAction();


public class PlayerController : MonoBehaviour
{
    Vector2 speed;
    public float basicWalkSpeed;
    public float JumpSpeed;
    public float attackWalkSpeed;

    new Rigidbody2D rigidbody;
    BoxCollider2D bodyCollider;
    CapsuleCollider2D weaponCollider;


    SpriteRenderer spriteRenderer;
    Animator animator;
    Vector2 moveForce;
    

    //데미지 관련 변수들
    public float maxHp;
    public float currentHp;
    public int damage;
    public float knockbackAmount;

    //플레이어 상태
    Dictionary<PlayerState, bool> state;
    Dictionary<PlayerState, StateAction> stateAction;
    //GUI
    public Slider slider;

    //INPUT
    Dictionary<KeyCode, KeyAction> KeyDownAction;
    Dictionary<KeyCode, KeyAction> KeyUpAction;

    //콤보 공격
    float comboTimer = 0f;
    public float comboDelay;



    //공격
    public bool attackAvailable = true;
    public float attackInterval = 0.0f;
    float attackTimer;

    //아이템
    public float potionHealAmount = 10.0f;
    public int coinAmount = 10;
    int coinCount = 0;


    private void Awake()
    {
        currentHp = maxHp;
        slider.value = slider.maxValue;

        speed.x = basicWalkSpeed;
        speed.y = JumpSpeed;


        KeyDownAction = new Dictionary<KeyCode, KeyAction>();
        KeyUpAction = new Dictionary<KeyCode, KeyAction>();



        KeyDownAction[KeyCode.Space] = Jump;
        KeyDownAction[KeyCode.Q] = BasicGroundAttack;

        state = new Dictionary<PlayerState, bool>();
        stateAction = new Dictionary<PlayerState, StateAction>();

        state[PlayerState.RUN] = false;
        state[PlayerState.JUMP] = false;
        state[PlayerState.BASIC_GROUNDATTACK] = false;
        state[PlayerState.AIRATTACK] = false;
        state[PlayerState.DEAD] = false;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<BoxCollider2D>();
        weaponCollider = GetComponentInChildren<CapsuleCollider2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (!state[PlayerState.DEAD])
        {
            attackTimer += Time.deltaTime;

            StateHandle();

            InputHandle();
        }
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" && state[PlayerState.BASIC_GROUNDATTACK] && attackAvailable)
        {
            var enemy = collision.GetComponent<EnemyController>();
            enemy.GetDamaged(damage, transform.position);
            attackAvailable = false;
        }
    }

    public void GrabItem(GameObject obj)
    {
        var category = obj.GetComponent<ItemScript>().category;
        switch(category)
        {
            case ItemCategoryEnum.HEALTH:
                if (currentHp == maxHp)
                    break;
                else if (currentHp + potionHealAmount > maxHp)
                {
                    currentHp = maxHp;
                    Destroy(obj);
                }
                else
                {
                    currentHp = currentHp + potionHealAmount;
                    Destroy(obj);
                }
                slider.value = currentHp / maxHp;
                break;
            case ItemCategoryEnum.COIN:
                coinCount += coinAmount;
                Destroy(obj);
                break;
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
            if (Input.GetKeyUp(data.Key))
                KeyUpAction[data.Key](false);
        }
    }
    
    private void StateHandle()
    {

        if (state[PlayerState.BASIC_GROUNDATTACK])
        {
            comboTimer += Time.deltaTime;
            if (comboTimer>comboDelay)
            {
                animator.SetTrigger("resetTrigger");
                state[PlayerState.BASIC_GROUNDATTACK] = false;//상태 바꿈
                comboTimer = 0.0f;
            }
        }
        if(!state[PlayerState.BASIC_GROUNDATTACK])
        {
            animator.SetTrigger("resetTrigger");
            animator.SetBool("isFirstAttack", false);
        }
        if(state[PlayerState.JUMP])
        {
            if(IsGrounded())
            {
                state[PlayerState.JUMP] = false;
                animator.SetBool("isJump", state[PlayerState.JUMP]);
            }
        }
        if(state[PlayerState.RUN])
        {
            speed.x = basicWalkSpeed;
        }
    }


    void Jump(bool isDown)
    {
        if (!state[PlayerState.JUMP] && isDown)
        {
            rigidbody.AddForce(new Vector2(0, speed.y), ForceMode2D.Impulse);

            state[PlayerState.JUMP] = true;
            animator.SetBool("isJump", state[PlayerState.JUMP]);
        }
    }

    void BasicGroundAttack(bool isDown)
    {
        if (!state[PlayerState.JUMP] && isDown)
        {
            animator.SetTrigger("attackTrigger");
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("BasicGroundAttack3"))
                comboTimer = 0.0f;


            state[PlayerState.BASIC_GROUNDATTACK] = true;
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
            weaponCollider.transform.rotation = Quaternion.Euler(0,0, 0);

            state[PlayerState.RUN] = true;
        }
        else if (moveInput < 0)
        {
            moveForce.x = -speed.x;
            spriteRenderer.flipX = true;
            animator.SetBool("isRun", true);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 180, 0);
            

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
        var contact = new ContactFilter2D();
        
        return bodyCollider.IsTouching(contact.NoFilter()) && Mathf.Abs(rigidbody.velocity.y) <= 0.001;
    }

    //애니메이션이 플레이 중인지 확인.
    bool AnimatorIsPlaying()
    {
        
        return animator.GetCurrentAnimatorClipInfo(0).Length >
       animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }


    //상호작용 함수들
    public void GetDamaged(int damage,Vector3 position)
    {
        rigidbody.AddForce((transform.position - position).normalized * knockbackAmount, ForceMode2D.Impulse);
        currentHp -= damage;
        slider.value = currentHp / maxHp;

        if(currentHp<=0)
        {
            animator.SetBool("isDead", true);
            state[PlayerState.DEAD] = true;
        }
    }
}
