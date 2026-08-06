using UnityEngine;
using System;

public class MobileMainThreadDispatcher : MonoBehaviour
{
    public static MobileMainThreadDispatcher instance;

    private static readonly System.Collections.Generic.Queue<Action> executionQueue = new System.Collections.Generic.Queue<Action>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Update()
    {
        lock (executionQueue)
        {
            while (executionQueue.Count > 0)
            {
                executionQueue.Dequeue().Invoke();
            }
        }
    }

    public static void ExecuteOnMainThread(Action action)
    {
        lock (executionQueue)
        {
            executionQueue.Enqueue(action);
        }
    }
}

