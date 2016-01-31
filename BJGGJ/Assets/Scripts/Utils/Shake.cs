using System;
using UnityEngine;


public class Shake : MonoBehaviour
{
	private static float[] randVals = new float[3] { 1231.1231f, 431.1231f, 6254.5135f },
						   randVals2 = new float[3] { 423.123f, 71.234f, 986.128f };

	
	public delegate void ShakeFinishedDelegate(Transform objTr);


	public event ShakeFinishedDelegate OnFinishedShake;

	public AnimationCurve StrengthOverTime = new AnimationCurve(new Keyframe(0.0f, 0.1f),
																new Keyframe(1.0f, 0.0f));
	public float ShakeTime = 0.5f;

	public Vector3 PerAxisMultiplier = Vector3.one;


	[NonSerialized]
	public float TimeLeft;

	public Transform MyTr { get; private set; }

	private Vector3 shakeDelta;


	void Awake()
	{
		MyTr = transform;
	}
	void Start()
	{
		TimeLeft = ShakeTime;
		shakeDelta = Vector3.zero;
	}
	void Update()
	{
		TimeLeft -= Time.deltaTime;
		
		if (TimeLeft == 0.0f)
		{
			if (OnFinishedShake != null)
				OnFinishedShake(MyTr);

			Destroy(this);
			return;
		}


		float t = 1.0f - (TimeLeft / ShakeTime);
		float strength = StrengthOverTime.Evaluate(t);
		Vector3 newShakeDelta = new Vector3(Mathf.Sin((Time.timeSinceLevelLoad * randVals[0]) +
													  randVals2[0]) *
											PerAxisMultiplier.x,
											Mathf.Cos((Time.timeSinceLevelLoad * randVals[1]) +
													  randVals2[1]) *
											PerAxisMultiplier.y,
											Mathf.Sin((Time.timeSinceLevelLoad * randVals[2]) +
													  randVals2[2]) *
											PerAxisMultiplier.z);
		newShakeDelta *= strength;

		MyTr.position = (MyTr.position - shakeDelta) + newShakeDelta;
		shakeDelta = newShakeDelta;
	}
}