using System;
using System.Collections.Generic;
using UnityEngine;



public class ControlsMenu : Singleton<ControlsMenu>
{
	static List<int> PlayerControls = new List<int>();


	public GameObject DoneButton;
	public UnityEngine.UI.Text InstructionsText;

	public float TimeBetweenInputs = 0.75f;
	private float timeLeft = 0.5f;


	public void OnButton_Back()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
	}
	public void OnButton_Done()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("TestMat");
	}


	protected override void Awake()
	{
		base.Awake();

		PlayerControls = new List<int>();
		DoneButton.SetActive(false);

		GenerateInstructions();
	}
	void Update()
	{
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0.0f)
		{
			for (int i = 0; i < InputManager.MaxPlayers; ++i)
			{
				if (!PlayerControls.Contains(i) &&
					(InputManager.Instance.MoveInput[i] != Vector2.zero ||
					 InputManager.Instance.TurnInput[i] != Vector2.zero ||
					 InputManager.Instance.TakePhotoInput[i]))
				{
					PlayerControls.Add(i);

					if (PlayerControls.Count > 1)
					{
						DoneButton.SetActive(true);
					}

					GenerateInstructions();

					timeLeft = TimeBetweenInputs;

					break;
				}
			}
		}
	}

	private void GenerateInstructions()
	{
		InstructionsText.text = "Player " + (PlayerControls.Count + 1).ToString() + ": Press Input";
	}
}