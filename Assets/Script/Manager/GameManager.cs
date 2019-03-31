using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionEnum
{
    DEAD
};

public class GameManager : MonoBehaviour
{
    public Sprite[] itemSprites;

    public static GameManager instance =null;

    public GameObject itemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void OnNotify( GameObject obj, ActionEnum action)
    {
        if(obj.tag == "Enemy" && action == ActionEnum.DEAD)
        {
            int index = Random.Range(0,itemSprites.Length);
            var item = Instantiate(itemPrefab, obj.transform.position,Quaternion.identity).GetComponent<SpriteRenderer>();
            item.sprite = itemSprites[index];
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
