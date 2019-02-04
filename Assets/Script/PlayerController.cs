using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rigidbody;
    SpriteRenderer spriteRenderer;
    Animator animator;
    public Vector2 speed;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");


     
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 jumpForce = new Vector2(0, speed.y);
            rigidbody.AddForce(jumpForce);
            animator.SetBool("isJump", true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool("isJump", false);
        }

        Vector2 movement = new Vector2(horizontalInput*Time.deltaTime*speed.x, 0);
        animator.SetFloat("runSpeed", Mathf.Abs(movement.x));

        if(horizontalInput>0)
        {
            transform.Translate(movement);
            spriteRenderer.flipX = false;
        }
        else if(horizontalInput<0)
        {
            transform.Translate(movement);
            spriteRenderer.flipX = true;
        }

    }
}
