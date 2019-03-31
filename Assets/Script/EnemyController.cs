using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum EnemyState
{
    WALK,
    ATTACK,
    DEAD
}


public class EnemyController : MonoBehaviour
{
    //데미지 관련 변수들
    public float maxHp;
    float currentHp;
    public int damage;
    public float knockbackAmount;
    public Slider slider;

    //물리
    new Rigidbody2D rigidbody;
    CircleCollider2D weaponCollider;

    // 애니메이션
    Animator animator;

    //그래픽
    new SpriteRenderer renderer;
    //AI
    public GameObject player;
    PlayerController playerController;

    //상태
    Dictionary<EnemyState, bool> state;
    public float detectDistance;
    public float attackDistance;

    //속도
    public float walkSpeed;
    float speed;

    //시간
    float attackTimer;


    //소리
    public string effectSound;
    

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        weaponCollider = GetComponent<CircleCollider2D>();
        playerController = player.GetComponent<PlayerController>();


        currentHp = maxHp;
        slider.value = slider.maxValue;

        state = new Dictionary<EnemyState, bool>();
        state[EnemyState.ATTACK] = false;
        state[EnemyState.WALK] = false;
        state[EnemyState.DEAD] = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleState();
        AI();
    }


    void AI()
    {
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distance < detectDistance && !state[EnemyState.ATTACK])
        {
            state[EnemyState.WALK] = true;
            animator.SetBool("isWalk", true);
        }
        else
        {
            state[EnemyState.WALK] = false;
            animator.SetBool("isWalk", false);
        }

        if(currentHp<=0)
        {
            state[EnemyState.DEAD] = true;
            animator.SetBool("isDead", true);
        }
    }

    void HandleState()
    {
        if (state[EnemyState.WALK] && !state[EnemyState.ATTACK] &&!state[EnemyState.DEAD])
        {
            speed = walkSpeed;
            Move();

        }
        if(state[EnemyState.ATTACK] && !state[EnemyState.DEAD])
        {
            attackTimer += Time.deltaTime;
   

            if (attackTimer >= animator.GetCurrentAnimatorStateInfo(0).length/2)
            {
                attackTimer = -animator.GetCurrentAnimatorStateInfo(0).length / 2;
                playerController.GetDamaged(damage, transform.position);             
            }          
        }

        if(state[EnemyState.DEAD])
        {
            if(animator.GetCurrentAnimatorClipInfo(0).Length <= animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                GameManager.instance.OnNotify(gameObject, ActionEnum.DEAD);
                Destroy(gameObject);
            }
        }
    }


    void Move()
    {
        if (player.transform.position.x < transform.position.x)
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0));
            renderer.flipX = false;
            weaponCollider.offset = new Vector2(Mathf.Abs(weaponCollider.offset.x),0);
        }
        else if (player.transform.position.x > transform.position.x)
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0));
            renderer.flipX = true;
            weaponCollider.offset = new Vector2(-Mathf.Abs(weaponCollider.offset.x), 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            state[EnemyState.ATTACK] = true;
            animator.SetBool("isAttack", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            attackTimer = 0.0f;
            state[EnemyState.ATTACK] = false;
            animator.SetBool("isAttack", false);
        }

    }


    //상호작용 함수들
    public void GetDamaged(int damage, Vector3 position)
    {
        rigidbody.AddForce((transform.position - position).normalized * knockbackAmount, ForceMode2D.Impulse);
        currentHp -= damage;
        slider.value = currentHp / maxHp;
    }
}
