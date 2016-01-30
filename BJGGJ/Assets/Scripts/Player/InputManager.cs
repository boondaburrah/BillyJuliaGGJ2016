using System;
using UnityEngine;


public class InputManager : Singleton<InputManager>
{
	public static readonly int MaxPlayers = 4;


	public Vector2[] MoveInput, TurnInput;
	public bool[] TakePhotoInput;

	public float MouseSensitivity = 5.0f,
				 GamepadSensitivity = 2.75f;


	protected override void Awake()
	{
		base.Awake();

		MoveInput = new Vector2[MaxPlayers];
		TurnInput = new Vector2[MaxPlayers];
		TakePhotoInput = new bool[MaxPlayers];
	}
	void Update()
	{
		for (int i = 0; i < MaxPlayers; ++i)
		{
			MoveInput[i] = Vector2.zero;
			TurnInput[i] = Vector2.zero;
			TakePhotoInput[i] = false;
		}


		if (Input.GetKey(KeyCode.W))
		{
			MoveInput[0].y += 1.0f;
		}
		if (Input.GetKey(KeyCode.S))
		{
			MoveInput[0].y -= 1.0f;
		}
		if (Input.GetKey(KeyCode.A))
		{
			MoveInput[0].x -= 1.0f;
		}
		if (Input.GetKey(KeyCode.D))
		{
			MoveInput[0].x += 1.0f;
		}
		TurnInput[0] = new Vector2(Input.GetAxis("Mouse X"),
								   -Input.GetAxis("Mouse Y"));
		TurnInput[0] *= MouseSensitivity;
		TakePhotoInput[0] = Input.GetMouseButtonDown(0);


		for (int i = 1; i < MaxPlayers; ++i)
		{
			MoveInput[i].x = Input.GetAxis("Left/Right " + i);
			MoveInput[i].y = -Input.GetAxis("Forward/Back " + i);
			MoveInput[i] *= GamepadSensitivity;

			TurnInput[i].x = Input.GetAxis("Yaw " + i);
			TurnInput[i].y = Input.GetAxis("Pitch " + i);
			TurnInput[i] *= GamepadSensitivity;

			TakePhotoInput[i] = Input.GetButton("Fire " + i);
		}
	}
}