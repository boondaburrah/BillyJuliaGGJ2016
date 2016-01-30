using System;
using UnityEngine;


public class DisableAfterTime : MonoBehaviour
{
	public float KillTime = 2.5f;

	public float TimeLeft { get; private set; }


	void OnEnable()
	{
		TimeLeft = KillTime;
	}
	void Update()
	{
		TimeLeft -= Time.deltaTime;
		if (TimeLeft <= 0.0f)
		{
			gameObject.SetActive(false);
		}
	}
}