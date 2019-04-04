using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AchieveEvent
{
    FIRSTJUMP
}

public class AchievementManager : MonoBehaviour, IObserver
{
    public  static AchievementManager instance = null;

    public Dictionary<AchieveEvent,bool> achievement;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        achievement = new Dictionary<AchieveEvent, bool>();
        achievement[AchieveEvent.FIRSTJUMP] = false;

        DontDestroyOnLoad(gameObject);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnNotify(GameObject obj, ActionEvent action)
    {
        Debug.Log("test");
        if(obj.tag == "player"&&action == ActionEvent.JUMP)
        {
            Unlock(AchieveEvent.FIRSTJUMP);
        }
    }

    private void Unlock(AchieveEvent achieveEvent)
    {
        achievement[achieveEvent] = true;
    }
}
