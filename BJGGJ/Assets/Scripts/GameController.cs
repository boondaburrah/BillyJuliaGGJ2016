using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameController : Singleton<GameController>
{
	public static List<List<Texture2D>> PhotosByPlayer;
	public static List<List<float>> PhotoScoresByPlayer;


	public static Rect GetScreenRect(int player, int totalPlayers)
	{
		switch (totalPlayers)
		{
			case 2:
				return new Rect(0.0f, player * 0.5f, 1.0f, 0.5f);
			case 3:
			case 4:
				return new Rect((player % 2) * 0.5f,
								(player / 2) * 0.5f,
								0.5f, 0.5f);
			case 5:
			case 6:
				return new Rect((player % 3) / 3.0f,
								(player / 3) / 3.0f,
								1.0f / 3.0f,
								1.0f / 3.0f);

			default:
				throw new NotImplementedException("Unknown number of players: " + totalPlayers);
		}
	}


	public GameObject[] PlayerPrefabs = new GameObject[5];
	public Transform[] SpawnPoses = new Transform[5];

	public float GameMins = 5.0f;

	public GUIStyle TextStyle = new GUIStyle();
	
	
	public TimeSpan TimeLeft;
	
	[NonSerialized]
	public List<PlayerController> Players;


	void Start()
	{
		TimeLeft = TimeSpan.FromMinutes(GameMins);

		Players = new List<PlayerController>();
		for (int i = 0; i < ControlsMenu.PlayerControls.Count; ++i)
		{
			Transform tr = Instantiate(PlayerPrefabs[i]).transform;
			tr.position = SpawnPoses[i].position;
			tr.forward = SpawnPoses[i].forward;

			Camera c = tr.GetComponent<Camera>();
			c.rect = GetScreenRect(i, ControlsMenu.PlayerControls.Count);

			Players.Add(tr.GetComponent<PlayerController>());
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
			PhotosByPlayer = Players.Select(p => p.Photos).ToList();
			PhotoScoresByPlayer = Players.Select(p => p.PhotoScores).ToList();

			UnityEngine.SceneManagement.SceneManager.LoadScene("Show Photos");
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