﻿using UnityEngine;


/// <summary>
/// Inherit from this class and use your own class for "T" to make it a singleton.
/// Example: "public class AudioContent : Singleton<AudioContent>".
/// Uses the "Awake()" event, but makes it virtual so extra behavior can be added.
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	public static T Instance { get; protected set; }

	protected virtual void Awake()
	{
		UnityEngine.Assertions.Assert.IsNull(Instance, "There are two instances of " + this.GetType());
		Instance = (T)this;
	}
	protected virtual void OnDestroy()
	{
		UnityEngine.Assertions.Assert.AreEqual(this, Instance,
											   "There is a different instance of " + this.GetType());
		Instance = null;
	}
}