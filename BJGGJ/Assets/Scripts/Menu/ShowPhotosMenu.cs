using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class ShowPhotosMenu : Singleton<ShowPhotosMenu>
{
	private GameObject CreatePhoto(Texture2D photo, float score)
	{
		GameObject photoGO = Instantiate(PhotoPrefab);
		photoGO.transform.SetParent(cv.transform, false);

		photoGO.GetComponent<RawImage>().texture = photo;

		score = Mathf.RoundToInt(score * 1000) / 10.0f;
		photoGO.GetComponentInChildren<Text>().text = score.ToString();

		return photoGO;
	}


	public GameObject PhotoPrefab;
	public int NPhotosWide = 5;
	public float PhotoSpacing = 10.0f;

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
		yield return null;

		Rect pixelR = cv.pixelRect;

		Rect[] playerRects = new Rect[GameController.PhotosByPlayer.Count];
		for (int i = 0; i < playerRects.Length; ++i)
		{
			int nPhotos = GameController.PhotosByPlayer[i].Count;

			if (nPhotos > 0)
			{
				Rect r = GameController.GetScreenRect(i, playerRects.Length);
				r = new Rect(Mathf.Lerp(pixelR.xMin, pixelR.xMax, r.x),
							 Mathf.Lerp(pixelR.yMin, pixelR.yMax, r.y),
							 r.width * pixelR.width,
							 r.height * pixelR.height);

				float photoWidth = (r.width / (float)NPhotosWide) -
								   ((nPhotos + 1) * PhotoSpacing);
				float photoScale = photoWidth / (float)GameController.PhotosByPlayer[i][0].width;
				float photoHeight = GameController.PhotosByPlayer[i][0].height * photoScale;

				
				for (int j = 0; j < nPhotos; ++j)
				{
					GameObject photo = CreatePhoto(GameController.PhotosByPlayer[i][j],
												   GameController.PhotoScoresByPlayer[i][j]);
					yield return null;

					int pX = (j % NPhotosWide),
						pY = (j / NPhotosWide);
					float x = PhotoSpacing + (pX * (PhotoSpacing + photoWidth)),
						  y = PhotoSpacing + (pY * (PhotoSpacing + photoHeight));
					
					RectTransform rectTr = (RectTransform)photo.transform;
					rectTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
														 r.xMin + x, photoWidth);
					yield return null;
					rectTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,
														 r.yMin + y, photoHeight);

					yield return new WaitForSeconds(0.2f);
				}

				yield return new WaitForSeconds(1.5f);
			}
		}

		yield break;
	}
}