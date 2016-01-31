using System;
using UnityEngine;


[RequireComponent(typeof(AudioListener))]
public class MultiAudioListener : Singleton<MultiAudioListener>
{
	public Transform MyTr { get; private set; }


	protected override void Awake()
	{
		base.Awake();

		MyTr = transform;
	}


	public void PlayClip(AudioClip clip, Vector3 localOffset)
	{
		AudioSource.PlayClipAtPoint(clip, MyTr.localToWorldMatrix.MultiplyPoint(localOffset));
	}
}