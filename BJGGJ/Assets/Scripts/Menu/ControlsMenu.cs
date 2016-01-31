using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ControlsMenu : Singleton<ControlsMenu>
{
	public static List<int> PlayerControls = new List<int>();


	public GameObject DoneButton;
	public UnityEngine.UI.Text InstructionsText;

	public float TimeBetweenInputs = 0.75f;
	private float timeLeft = 0.5f;


	public void OnButton_Back()
	{
		AudioSource.PlayClipAtPoint(Sounds.Instance.UIClick, Vector3.zero);
		UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
	}
	public void OnButton_Done()
	{
		AudioSource.PlayClipAtPoint(Sounds.Instance.UIClick, Vector3.zero);
		UnityEngine.SceneManagement.SceneManager.LoadScene("Match Scene");
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
					 (i != 0 && InputManager.Instance.TurnInput[i] != Vector2.zero) ||
					 InputManager.Instance.TakePhotoInput[i] ||
					 InputManager.Instance.JumpInput[i]))
				{
					AudioSource.PlayClipAtPoint(Sounds.Instance.UIClick, Vector3.zero);
					timeLeft = TimeBetweenInputs;
					StartCoroutine(AddPlayerCoroutine(i));

					break;
				}
			}
		}
	}

	private void GenerateInstructions()
	{
		InstructionsText.text = "Player " + (PlayerControls.Count + 1).ToString() + ": Press Input";
	}

	private IEnumerator AddPlayerCoroutine(int inputIndex)
	{
		//Wait a bit in case the player was just clicking a button.
		yield return new WaitForSeconds(0.25f);
		
		PlayerControls.Add(inputIndex);

		if (PlayerControls.Count > 1)
		{
			DoneButton.SetActive(true);
		}

		GenerateInstructions();
	}
}