using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObstacleScript : MonoBehaviour
{
    
    TilemapCollider2D tilemapCollider;
    float damageTimer = 0.0f;
    public float damageCoolTime = 1.0f;
    public float damage = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        damageTimer = damageCoolTime;
        tilemapCollider = GetComponent<TilemapCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        damageTimer += Time.deltaTime;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.collider.CompareTag("Player") && damageTimer > damageCoolTime)
        {
            damageTimer = 0.0f;
            collision.gameObject.GetComponent<PlayerController>().GetDamaged(damage, collision.collider.transform.position, 3.0f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player") && damageTimer > damageCoolTime)
        {
            damageTimer = 0.0f;
            collision.gameObject.GetComponent<PlayerController>().GetDamaged(damage,  transform.position, 3.0f);
        }
    }
}
