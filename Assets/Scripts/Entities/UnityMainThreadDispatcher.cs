using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("UnityMainThreadDispatcher");
                _instance = go.AddComponent<UnityMainThreadDispatcher>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Enqueue(System.Action action)
    {
        lock (queue)
        {
            queue.Enqueue(action);
        }
    }

    private Queue<System.Action> queue = new Queue<System.Action>();

    private void Update()
    {
        lock (queue)
        {
            while (queue.Count > 0)
            {
                queue.Dequeue().Invoke();
            }
        }
    }
}
