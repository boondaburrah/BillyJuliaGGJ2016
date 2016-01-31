using System;
using System.Collections.Generic;
using UnityEngine;


public class Sounds : Singleton<Sounds>
{
	public AudioClip UIClick, TenSecondWarning, TakePhoto;


	protected override void Awake()
	{
		//This singleton doesn't destroy on load,
		//    so it will have duplicates when going back to the main menu.
		if (Instance != null)
		{
			Destroy(gameObject);
			return;
		}


		base.Awake();

		DontDestroyOnLoad(gameObject);
	}

	protected override void OnDestroy()
	{
		if (Instance == this)
			Instance = null;

		base.OnDestroy();
	}
}