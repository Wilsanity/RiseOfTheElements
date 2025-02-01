using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvent : ScriptableObject
{
    private readonly List<GameEventListener> gameEventListeners = new List<GameEventListener>();

    public void Raise()
    {
        for (int i = gameEventListeners.Count - 1; i >= 0; i--)
        {
            gameEventListeners[i].OnRaise();
        }
    }

    public void RegisterGameEventListener(GameEventListener listener)
    {
        if(!gameEventListeners.Contains(listener))
        {
            gameEventListeners.Add(listener);
        }
    }

    public void UnregisterGameEventListener(GameEventListener listener)
    {
        if (gameEventListeners.Contains(listener))
        {
            gameEventListeners.Remove(listener);
        }
    }

}
