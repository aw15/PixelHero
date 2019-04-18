using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum ActionEvent
{
    JUMP
}

public interface IObserver
{
    void OnNotify(GameObject obj, ActionEvent action);
}

public class Subject
{ 
    public List<IObserver> observers;

    public Subject()
    {
        observers = new List<IObserver>();
    }

    public void Notify(GameObject obj, ActionEvent action)
    {
        foreach (var observer in observers)
        {
            observer.OnNotify(obj, action);
        }
    }
}
