using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public struct AudioClipStruct
{
    public string name;
    public AudioClip clip;
}


public class SoundManager : MonoBehaviour
{

    public static SoundManager instance = null;

    public AudioSource efxSource;
    public AudioSource musicSource;

    [SerializeField]
    public AudioClipStruct[] audioArray;
    public Dictionary<string, AudioClip> audioList;
    

    // Start is called before the first frame update
    void Start()
    {

        audioList = new Dictionary<string, AudioClip>();

        foreach(var data in audioArray)
        {
            audioList[data.name] = data.clip;
        }
        

        if (instance == null)
        {
            instance = this;
        }
        else if(instance !=this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }


    public void PlayEffectSound(string name)
    {
        efxSource.clip = audioList[name];
        efxSource.Play();
    }
        
}
