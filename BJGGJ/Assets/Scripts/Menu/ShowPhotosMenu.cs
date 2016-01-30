using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ShowPhotosMenu : Singleton<ShowPhotosMenu>
{
	private Transform CreatePhoto(Texture2D photo, float score)
	{
		GameObject photoGO = Instantiate(PhotoPrefab);
		photoGO.GetComponent<RawImage>().texture = photo;

		score = Mathf.RoundToInt(score * 1000) / 10.0f;
		photoGO.GetComponent<Text>().text = score.ToString();

		return photoGO.transform;
	}


	public GameObject PhotoPrefab;

	private Canvas cv;


	void Start()
	{
		cv = FindObjectOfType<Canvas>();
		StartCoroutine(RunSceneCoroutine());
	}


	public void OnButton_MainMenu()
	{
		GameController.PhotosByPlayer.Clear();
		GameController.PhotoScoresByPlayer.Clear();

		UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
	}


	public System.Collections.IEnumerator RunSceneCoroutine()
	{
		yield break;
	}
}