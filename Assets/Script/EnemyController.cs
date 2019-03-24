using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum EnemyState
{
    WALK,
    ATTACK
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

    // 애니메이션
    new Animator animator;

    //그래픽
    new SpriteRenderer renderer;
    //AI
    public GameObject player;

    //상태
    Dictionary<EnemyState, bool> state;
    public float detectDistance;
    public float attackDistance;

    //속도
    public float walkSpeed;
    float speed;

    //시간
    float attackTimer;
    public float attackInterval;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHp = maxHp;
        slider.value = slider.maxValue;

        state = new Dictionary<EnemyState, bool>();
        state[EnemyState.ATTACK] = false;
        state[EnemyState.WALK] = false;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer += Time.deltaTime;

        HandleState();
        AI();
    }


    void AI()
    {
        float distance = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distance < detectDistance && distance >= attackDistance)
        {
            state[EnemyState.WALK] = true;
            animator.SetBool("isWalk", true);
        }
        else
        {
            state[EnemyState.WALK] = false;
            animator.SetBool("isWalk", false);
        }


        if(distance < attackDistance)
        {
            state[EnemyState.ATTACK] = true;
            animator.SetBool("isAttack", true);
        }
        else
        {
            state[EnemyState.ATTACK] = false;
            animator.SetBool("isAttack", false);
        }
    }

    void HandleState()
    {
        if (state[EnemyState.WALK])
        {
            speed = walkSpeed;
            Move();
        }
        if(state[EnemyState.ATTACK])
        {

        }
    }


    void Move()
    {
        if (player.transform.position.x < transform.position.x)
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0));
            renderer.flipX = false;
        }
        else if (player.transform.position.x > transform.position.x)
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0));
            renderer.flipX = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player")
        {
            var player = collision.collider.GetComponent<PlayerController>();
            player.GetDamaged(damage, transform.position);
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
