using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemCategoryEnum
{
    HEALTH,
    COIN
}


public class ItemScript : MonoBehaviour
{
    SpriteRenderer renderer;
    Color color;

    public ItemCategoryEnum category;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        color = renderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        color.a += Time.deltaTime*0.5f;
        renderer.color = color;

        if(color.a>1)
        {
            color.a = 0;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.GetType() == typeof(BoxCollider2D))
        {
            collision.SendMessage("GrabItem", gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
