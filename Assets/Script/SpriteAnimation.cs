using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation : MonoBehaviour
{
    SpriteRenderer renderer;
    Color color;
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
}
