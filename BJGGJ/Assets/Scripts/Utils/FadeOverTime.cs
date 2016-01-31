using System;
using UnityEngine;


[RequireComponent(typeof(MeshRenderer))]
public class FadeOverTime : MonoBehaviour
{
	public float FadeTime = 0.75f;
	public AnimationCurve FadeCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);

	public float TimeLeft { get; set; }
	public MeshRenderer MyRnd { get; private set; }
	

	void Awake()
	{
		MyRnd = GetComponent<MeshRenderer>();
	}
	void OnEnable()
	{
		TimeLeft = FadeTime;
	}
	void Update()
	{
		TimeLeft -= Time.deltaTime;

		float alpha = Mathf.Clamp01(TimeLeft / FadeTime);
		MyRnd.material.color = new Color(1.0f, 1.0f, 1.0f,
										 FadeCurve.Evaluate(1.0f - alpha));
	}
}