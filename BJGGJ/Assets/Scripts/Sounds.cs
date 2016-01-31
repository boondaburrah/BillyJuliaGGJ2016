using System;
using System.Collections.Generic;
using UnityEngine;


public class Sounds : Singleton<Sounds>
{
	public AudioClip UIClick, TenSecondWarning,
					 TakePhoto, Warning, PhotoFlip;


	public AudioSource MenuMusic, GameMusic;
	public System.Collections.IEnumerator FadeToMenuCoroutine()
	{
		float volIncrement = 0.01f;
		while (GameMusic.volume > 0.0f || MenuMusic.volume < 1.0f)
		{
			GameMusic.volume = Mathf.Max(0.0f, GameMusic.volume - volIncrement);
			MenuMusic.volume = Mathf.Min(1.0f, MenuMusic.volume + volIncrement);
			yield return null;
		}
	}
	public System.Collections.IEnumerator FadeToGameCoroutine()
	{
		float volIncrement = 0.025f;
		while (MenuMusic.volume > 0.0f || GameMusic.volume < 1.0f)
		{
			MenuMusic.volume = Mathf.Max(0.0f, MenuMusic.volume - volIncrement);
			GameMusic.volume = Mathf.Max(1.0f, GameMusic.volume + volIncrement);
			yield return null;
		}
	}


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
		DontDestroyOnLoad(MenuMusic.gameObject);
		DontDestroyOnLoad(GameMusic.gameObject);
	}

	protected override void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}
}