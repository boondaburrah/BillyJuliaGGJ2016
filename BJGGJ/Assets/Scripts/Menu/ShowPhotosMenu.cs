using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class ShowPhotosMenu : Singleton<ShowPhotosMenu>
{
	private void PutObjectInCanvas(Transform tr)
	{
		Transform cvTr = cv.transform;
		Transform buttonTr = cvTr.GetChild(cvTr.childCount - 1);

		buttonTr.SetParent(null, false);

		tr.SetParent(cvTr, false);
		buttonTr.SetParent(cvTr, false);
	}

	private GameObject CreatePhoto(Texture2D photo, float score)
	{
		GameObject photoGO = Instantiate(PhotoPrefab);
		PutObjectInCanvas(photoGO.transform);

		photoGO.GetComponent<RawImage>().texture = photo;

		score = Mathf.RoundToInt(score * 1000) / 10.0f;
		foreach (Text t in photoGO.GetComponentsInChildren<Text>())
			t.text = score.ToString();

		return photoGO;
	}
	private GameObject CreateText(string _text, int fontSize)
	{
		GameObject go = new GameObject("Text");
		Text txt = go.AddComponent<Text>();
		txt.text = _text;
		txt.font = TextFont;
		txt.fontSize = fontSize;
		txt.color = Color.white;
		txt.alignment = TextAnchor.MiddleCenter;
		txt.horizontalOverflow = HorizontalWrapMode.Overflow;
		txt.verticalOverflow = VerticalWrapMode.Overflow;

		PutObjectInCanvas(go.transform);

		return go;
	}
	private GameObject CreateScore(float score, int fontSize)
	{
		return CreateText((Mathf.RoundToInt(score * 1000) / 10.0f).ToString(),
						  fontSize);
	}


	public GameObject PhotoPrefab, GreyBackdrop;
	public int NPhotosWide = 5;
	public float PhotoSpacing = 2.5f;

	public Image DividerImg;

	public Font TextFont;
	public int BigFontSize = 50;

	private Canvas cv;
	private Rect GetCanvasSubRect(Rect lerpRect)
	{
		Rect pixelR = cv.pixelRect;
		return new Rect(Mathf.Lerp(pixelR.xMin, pixelR.xMax, lerpRect.x),
						Mathf.Lerp(pixelR.yMin, pixelR.yMax, lerpRect.y),
						lerpRect.width * pixelR.width,
						lerpRect.height * pixelR.height);
	}


	void Start()
	{
		cv = FindObjectOfType<Canvas>();
		StartCoroutine(RunSceneCoroutine());

		DividerImg.sprite = GameController.GetScreenDivider(GameController.PhotosByPlayer.Count);
		
		StartCoroutine(Sounds.Instance.FadeToMenuCoroutine());
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

		Rect[] playerRects = new Rect[GameController.PhotosByPlayer.Count];
		for (int i = 0; i < playerRects.Length; ++i)
		{
			int nPhotos = GameController.PhotosByPlayer[i].Count;

			if (nPhotos > 0)
			{
				//Get the pixel-coordinates rectangle for this player's view.
				Rect r = GameController.GetScreenRectFlipY(i, playerRects.Length);
				r = GetCanvasSubRect(r);


				//Get the scale factor for the photos to fit.

				int nPhotosWide = Mathf.Min(NPhotosWide, nPhotos),
					nPhotosTall = (nPhotos / nPhotosWide) +
									(nPhotos % nPhotosWide == 0 ?
										0 :
										1);
				float photoWidth = (r.width - PhotoSpacing - (PhotoSpacing * nPhotosWide)) / (float)nPhotosWide;
				float photoScale = photoWidth / (float)GameController.PhotosByPlayer[i][0].width;
				float photoHeight = GameController.PhotosByPlayer[i][0].height * photoScale;

				//Make sure they don't extend too far vertically.
				float fullYExtents = PhotoSpacing + (nPhotosTall * (photoHeight + PhotoSpacing));
				if (fullYExtents > r.height)
				{
					photoHeight = (r.height - PhotoSpacing - (PhotoSpacing * nPhotosTall)) / (float)nPhotosTall;
					photoScale = photoHeight / (float)GameController.PhotosByPlayer[i][0].height;
					photoWidth = GameController.PhotosByPlayer[i][0].width * photoScale;

					fullYExtents = PhotoSpacing + (nPhotosTall * (photoHeight + PhotoSpacing));
				}

				
				//Create and position each photo.
				for (int j = 0; j < nPhotos; ++j)
				{
					GameObject photo = CreatePhoto(GameController.PhotosByPlayer[i][j],
												   GameController.PhotoScoresByPlayer[i][j]);
					AudioSource.PlayClipAtPoint(Sounds.Instance.PhotoFlip, Vector3.zero);
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

					yield return new WaitForSeconds(0.35f);
				}

				yield return new WaitForSeconds(1.5f);
			}
		}

		yield return new WaitForSeconds(1.5f);

		//Turn on the grey backdrop.
		Transform backdropTr = GreyBackdrop.transform;
		backdropTr.SetParent(null, false);
		PutObjectInCanvas(backdropTr);
		GreyBackdrop.SetActive(true);

		yield return new WaitForSeconds(0.25f);

		//Show the score for each player.
		List<float> scores = GameController.PhotoScoresByPlayer.Select(l => l.Sum()).ToList();
		int winner = 0;
		for (int i = 0; i < scores.Count; ++i)
		{
			Rect r = GameController.GetScreenRect(i, GameController.PhotosByPlayer.Count);
			r = GetCanvasSubRect(r);

			Transform scoreNumber = CreateScore(scores[i], BigFontSize).transform;
			scoreNumber.position = r.center;

			if (scores[i] > scores[winner])
				winner = i;
			
			AudioSource.PlayClipAtPoint(Sounds.Instance.PhotoFlip, Vector3.zero);

			yield return new WaitForSeconds(1.0f);
		}

		yield return new WaitForSeconds(1.5f);

		//Put the grey backdrop back in front.
		backdropTr.SetParent(null, false);
		PutObjectInCanvas(backdropTr);

		//Show the winner.
		Rect winnerRect = GetCanvasSubRect(GameController.GetScreenRect(winner, scores.Count));
		CreateText("You win!", BigFontSize).transform.position = winnerRect.center;
		
		AudioSource.PlayClipAtPoint(Sounds.Instance.PhotoFlip, Vector3.zero);
	}
}