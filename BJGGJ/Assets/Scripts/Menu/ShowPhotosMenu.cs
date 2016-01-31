using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class ShowPhotosMenu : Singleton<ShowPhotosMenu>
{
	private GameObject CreatePhoto(Texture2D photo, float score)
	{
		Transform cvTr = cv.transform;
		Transform buttonTr = cvTr.GetChild(cvTr.childCount - 1);

		buttonTr.parent = null;

		GameObject photoGO = Instantiate(PhotoPrefab);
		photoGO.transform.SetParent(cvTr, false);

		buttonTr.SetParent(cvTr, true);

		photoGO.GetComponent<RawImage>().texture = photo;

		score = Mathf.RoundToInt(score * 1000) / 10.0f;
		foreach (Text t in photoGO.GetComponentsInChildren<Text>())
			t.text = score.ToString();

		return photoGO;
	}


	public GameObject PhotoPrefab;
	public int NPhotosWide = 5;
	public float PhotoSpacing = 2.5f;

	public Image DividerImg;

	private Canvas cv;


	void Start()
	{
		cv = FindObjectOfType<Canvas>();
		StartCoroutine(RunSceneCoroutine());

		DividerImg.sprite = GameController.GetScreenDivider(GameController.PhotosByPlayer.Count);
	}


	public void OnButton_MainMenu()
	{
		AudioSource.PlayClipAtPoint(Sounds.Instance.UIClick, Vector3.zero);

		GameController.PhotosByPlayer.Clear();
		GameController.PhotoScoresByPlayer.Clear();

		UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu");
	}


	public System.Collections.IEnumerator RunSceneCoroutine()
	{
		DividerImg.gameObject.SetActive(false);
		DividerImg.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
															   0.0f,
															   cv.pixelRect.width);

		yield return null;
		
		DividerImg.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,
															   0.0f,
															   cv.pixelRect.height);
		DividerImg.gameObject.SetActive(true);

		yield return null;

		Rect pixelR = cv.pixelRect;

		Rect[] playerRects = new Rect[GameController.PhotosByPlayer.Count];
		for (int i = 0; i < playerRects.Length; ++i)
		{
			int nPhotos = GameController.PhotosByPlayer[i].Count;

			if (nPhotos > 0)
			{
				//Get the pixel-coordinates rectangle for this player's view.
				Rect r = GameController.GetScreenRectFlipY(i, playerRects.Length);
				r = new Rect(Mathf.Lerp(pixelR.xMin, pixelR.xMax, r.x),
							 Mathf.Lerp(pixelR.yMin, pixelR.yMax, r.y),
							 r.width * pixelR.width,
							 r.height * pixelR.height);


				//Get the scale factor for the photos to fit.

				int nPhotosWide = Mathf.Min(NPhotosWide, nPhotos),
					nPhotosTall = nPhotos / nPhotosWide;
				float photoWidth = (r.width - PhotoSpacing - (PhotoSpacing * nPhotosWide)) / (float)nPhotosWide;
				float photoScale = photoWidth / (float)GameController.PhotosByPlayer[i][0].width;
				float photoHeight = GameController.PhotosByPlayer[i][0].height * photoScale;

				//Make sure they don't extend too far vertically.
				float fullYExtents = PhotoSpacing + (nPhotosTall * (photoHeight + PhotoSpacing));
				if (fullYExtents > r.height)
				{
					float scale = r.height / fullYExtents;

					photoWidth *= scale;
					photoHeight *= scale;
					photoScale *= scale;

					fullYExtents = PhotoSpacing + (nPhotosTall * (photoHeight + PhotoSpacing));
				}

				
				//Create and position each photo.
				for (int j = 0; j < nPhotos; ++j)
				{
					GameObject photo = CreatePhoto(GameController.PhotosByPlayer[i][j],
												   GameController.PhotoScoresByPlayer[i][j]);
					photo.SetActive(false);

					yield return null;

					int pX = (j % nPhotosWide),
						pY = (j / nPhotosWide);
					float x = PhotoSpacing + (pX * (PhotoSpacing + photoWidth)),
						  y = PhotoSpacing + (pY * (PhotoSpacing + photoHeight));
					
					RectTransform rectTr = (RectTransform)photo.transform;
					rectTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left,
														 r.xMin + x, photoWidth);
					yield return null;
					rectTr.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top,
														 r.yMin + y, photoHeight);
					photo.SetActive(true);

					AudioSource.PlayClipAtPoint(Sounds.Instance.PhotoFlip, Vector3.zero);

					yield return new WaitForSeconds(0.2f);
				}

				yield return new WaitForSeconds(1.5f);
			}
		}

		yield break;
	}
}