using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameController : Singleton<GameController>
{
	public static List<List<Texture2D>> PhotosByPlayer;
	public static List<List<float>> PhotoScoresByPlayer;


	public GameObject[] PlayerPrefabs = new GameObject[5];
	public Transform[] SpawnPoses = new Transform[5];

	public float GameMins = 5.0f;

	public GUIStyle TextStyle = new GUIStyle();
	public TimeSpan TimeLeft;


	void Start()
	{
		TimeLeft = TimeSpan.FromMinutes(GameMins);

		for (int i = 0; i < ControlsMenu.PlayerControls.Count; ++i)
		{
			Transform tr = Instantiate(PlayerPrefabs[i]).transform;
			tr.position = SpawnPoses[i].position;
			tr.forward = SpawnPoses[i].forward;

			Camera c = tr.GetComponent<Camera>();

			switch (ControlsMenu.PlayerControls.Count)
			{
				case 2:
					c.rect = new Rect(0.0f, i * 0.5f, 1.0f, 0.5f);
					break;

				case 3:
				case 4:
					c.rect = new Rect((i % 2) * 0.5f,
									  (i / 2) * 0.5f,
									  0.5f, 0.5f);
					break;

				case 5:
					c.rect = new Rect((i % 3) / 3.0f,
									  (i / 3) / 3.0f,
									  1.0f / 3.0f,
									  1.0f / 3.0f);
					break;

				default:
					throw new NotImplementedException("Unprepared for " +
													  ControlsMenu.PlayerControls.Count +
													  " players");
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
		}

		TimeLeft -= TimeSpan.FromSeconds(Time.deltaTime);
		if (TimeLeft.TotalSeconds <= 0.0f)
		{

		}
	}

	void OnGUI()
	{
		string mins = TimeLeft.Minutes.ToString();
		string secs = TimeLeft.Seconds.ToString();
		string subSecs = (TimeLeft.Milliseconds / 100).ToString();

		GUI.Label(new Rect(Screen.width / 2.0f, 0.0f,
						   0.5f, 0.5f),
				  mins + ":" + secs + "." + subSecs,
				  TextStyle);
	}
}