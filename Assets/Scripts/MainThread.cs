using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainThread : MonoBehaviour
{
    private static MainThread MainInstance;

    private Queue<Action> m_requests = new();

    private void Awake()
    {
        if (MainInstance != null)
            Destroy(MainInstance);

        MainInstance = this;
    }

    private void Update()
    {
        while (m_requests.Count > 0)
            m_requests.Dequeue()?.Invoke();
    }

    public static void Request(Action action)
    {
        MainInstance.m_requests.Enqueue(action);
    }
}
