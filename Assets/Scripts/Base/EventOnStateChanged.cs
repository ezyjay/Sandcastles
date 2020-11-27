using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnStateChanged : MonoBehaviour
{
    public UnityEvent onEnable, onDisable;

	public void OnEnable()
	{
		onEnable.Invoke();
	}

	public void OnDisable()
	{
		onDisable.Invoke();
	}
}
