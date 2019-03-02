using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //데미지 관련 함수들
    public float hp;
    public int damage;
    public float knockbackAmount;


    //물리
    Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        hp -= damage;
    }
}
