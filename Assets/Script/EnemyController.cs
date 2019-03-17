using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    //데미지 관련 변수들
    public float maxHp;
    float currentHp;
    public int damage;
    public float knockbackAmount;
    public Slider slider;

    //물리
    Rigidbody2D rigidbody;
    public float walkSpeed;

    //그래픽
    SpriteRenderer renderer;
    //AI
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        currentHp = maxHp;
        slider.value = slider.maxValue;

    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x <  transform.position.x)
        {
            transform.Translate(new Vector3(walkSpeed * Time.deltaTime,0));
            renderer.flipX = false;
        }
        else if (player.transform.position.x > transform.position.x)
        {
            transform.Translate(new Vector3(-walkSpeed * Time.deltaTime,0));
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
