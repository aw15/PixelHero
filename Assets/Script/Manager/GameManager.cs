using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ActionEnum
{
    DEAD
};


[Serializable]
public struct ItemStruct
{
    public ItemCategoryEnum category;
    public Sprite sprite;
}


public class GameManager : MonoBehaviour
{
    [SerializeField]
    public ItemStruct[] items;

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
            int index = UnityEngine.Random.Range(0,items.Length);
            var item = Instantiate(itemPrefab, obj.transform.position, Quaternion.identity);
            var itemSpite = item.GetComponent<SpriteRenderer>();
            var itemScript = item.GetComponent<ItemScript>();

            itemSpite.sprite = items[index].sprite;
            itemScript.category = items[index].category;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
