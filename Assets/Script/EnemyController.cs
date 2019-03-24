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


    //그래픽
    new SpriteRenderer renderer;
    //AI
    public GameObject player;

    //상태
    Dictionary<EnemyState, bool> state;
    public float detectDistance;

    //속도
    public float walkSpeed;
    public float AttackWalkSpeed;


    //시간
    float attackTimer;
    public float attackInterval;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
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
        if(Mathf.Abs(player.transform.position.x - transform.position.x)<detectDistance)
            state[EnemyState.WALK] = true;
        else
            state[EnemyState.WALK] = false;
    }

    void HandleState()
    {
        if (state[EnemyState.WALK])
        {
            if (player.transform.position.x < transform.position.x)
            {
                transform.Translate(new Vector3(walkSpeed * Time.deltaTime, 0));
                renderer.flipX = false;
            }
            else if (player.transform.position.x > transform.position.x)
            {
                transform.Translate(new Vector3(-walkSpeed * Time.deltaTime, 0));
                renderer.flipX = true;
            }
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
