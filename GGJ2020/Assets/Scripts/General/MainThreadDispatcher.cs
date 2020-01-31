using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Run
{
    public static bool IsAvailable => MainThreadDispatcher.Active;

    public static void OnMainThread(IEnumerator action)
    {
        try
        {
            MainThreadDispatcher.Enqueue(action);
        }
        catch (Exception)
        {
            Debug.LogWarning("Couldn't execute an action on the main thread since there is no dispatcher instance");
        }
    }

    public static void OnMainThread(Action action)
    {
        try
        {
            MainThreadDispatcher.Enqueue(action);
        }
        catch (Exception)
        {
            Debug.LogWarning("Couldn't execute an action on the main thread since there is no dispatcher instance");
        }
    }

    public static void Async(Action action)
    {
        try
        {
            new Thread(() => { action(); }).Start();
        }
        catch (Exception)
        {
            Debug.LogWarning("Couldn't execute an action on a new thread since there is no dispatcher instance");
        }
    }
}

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher instance;

    private readonly Queue<Action> queue = new Queue<Action>();

    public static bool Active
    {
        get
        {
            try
            {
                return Instance != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    private static MainThreadDispatcher Instance
    {
        get
        {
            if (instance == null)
            {
                throw new Exception("Instance of MainThreadDispatcher not found");
            }
            return instance;
        }
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void Update()
    {
        lock (queue)
        {
            while (queue.Count > 0)
            {
                queue.Dequeue().Invoke();
            }
        }
    }

    public static void Enqueue(IEnumerator action)
    {
        lock (Instance.queue)
        {
            Instance.queue.Enqueue(() => { Instance.StartCoroutine(action); });
        }
    }

    public static void Enqueue(Action action)
    {
        Enqueue(Instance.ActionWrapper(action));
    }

    private IEnumerator ActionWrapper(Action action)
    {
        action();
        yield return null;
    }
}