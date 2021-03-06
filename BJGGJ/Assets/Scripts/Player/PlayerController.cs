﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
	public static List<PlayerController> Players = new List<PlayerController>();


	public static Vector3 MakeHorz(Vector3 full)
	{
		return new Vector3(full.x, 0.0f, full.z).normalized;
	}


	public CharacterController MyContr { get; private set; }
	public Transform MyTr { get; private set; }

	public Vector2 MoveInput { get { return InputManager.Instance.MoveInput[InputIndex]; } }
	public Vector2 TurnInput { get { return InputManager.Instance.TurnInput[InputIndex]; } }
	public bool IsShooting { get { return InputManager.Instance.TakePhotoInput[InputIndex]; } }
	public bool IsJumping { get { return InputManager.Instance.JumpInput[InputIndex]; } }


	public int InputIndex = 0;

	public float MoveSpeed = 2.0f;
	public float JumpSpeed = 10.0f;

	public float Gravity = 5.0f;
	public float GravitySpeed = 0.0f;

	public int PhotosLeft = 15;
	public float PhotoInterval = 5.0f;
	public Camera PhotoCam;
	public GameObject PhotoEffects;
	public Transform PhotoReticule;
	public MeshRenderer PolaroidObj;
	public int RaycastsPerSide = 5;
	public LayerMask CameraRaycastLayers;

	public AudioClip JumpSound;


	[NonSerialized]
	public bool CanTakePhotos = true;

	[NonSerialized]
	public List<Texture2D> Photos;
	[NonSerialized]
	public List<float> PhotoScores;


	void Awake()
	{
		MyContr = GetComponent<CharacterController>();
		MyTr = transform;

		Photos = new List<Texture2D>();
		PhotoScores = new List<float>();

		Players.Add(this);
	}
	void OnDestroy()
	{
		Players.Remove(this);
	}

	void Update()
	{
		//Input.

		MyTr.Rotate(new Vector3(0.0f, 1.0f, 0.0f), TurnInput.x, Space.World);
		MyTr.Rotate(MyTr.right, TurnInput.y, Space.World);

		Vector3 moveDelta = (MoveInput.x * MyTr.right) +
							(MoveInput.y * MakeHorz(MyTr.forward));
		MyContr.Move(moveDelta * MoveSpeed * Time.deltaTime);
		
		if (CanTakePhotos && IsShooting)
		{
			StartCoroutine(TakePhotoCoroutine());
		}


		//Gravity.

		GravitySpeed -= Time.deltaTime * Gravity;

		Vector3 gravityDir = new Vector3(0.0f, GravitySpeed * Time.deltaTime, 0.0f);
		CollisionFlags hitResult = MyContr.Move(gravityDir);

		if ((hitResult & CollisionFlags.Below) != CollisionFlags.None)
		{
			GravitySpeed = 0.0f;

			//Jumping.
			if (IsJumping)
			{
				GravitySpeed = JumpSpeed;
				MultiAudioListener.Instance.PlayClip(JumpSound, Vector3.zero);
			}
		}
	}


	private IEnumerator TakePhotoCoroutine()
	{
		CanTakePhotos = false;

		//Let the camera render a photo.
		PhotoCam.gameObject.SetActive(true);
		yield return null;
		PhotoCam.gameObject.SetActive(false);


		//Capture it.

		RenderTexture tex = PhotoCam.targetTexture;
		Texture2D photo = new Texture2D(tex.width, tex.height, TextureFormat.RGBAHalf, false);
		
		RenderTexture.active = tex;
		photo.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		photo.Apply(true, true);
		RenderTexture.active = null;

		Photos.Add(photo);
		PhotoEffects.SetActive(true);


		//Update GameObjects.

		PhotoReticule.gameObject.SetActive(false);
		PolaroidObj.gameObject.SetActive(true);
		PolaroidObj.material.mainTexture = photo;
		
		MultiAudioListener.Instance.PlayClip(Sounds.Instance.TakePhoto, Vector3.zero);


		//Cast rays to see how exposed the players are in the photo.
		Vector3 centerPos = PhotoReticule.position,
				up = PhotoReticule.up * PhotoReticule.localScale.y,
				side = PhotoReticule.right * PhotoReticule.localScale.x;
		Vector3 rayStart = PhotoCam.transform.position;
		
		float score = 0.0f,
			  invNCasts = 1.0f / (float)(RaycastsPerSide * RaycastsPerSide);

		for (int x = 0; x < RaycastsPerSide; ++x)
		{
			float posX = ((float)x / (float)(RaycastsPerSide - 1)) - 0.5f;

			for (int y = 0; y < RaycastsPerSide; ++y)
			{
				float posY = ((float)y / (float)(RaycastsPerSide - 1)) - 0.5f;
				//Stagger the casts a bit.
				if (y % 2 == 0)
					posX += 0.1f;
				else posX -= 0.1f;

				RaycastHit hit = new RaycastHit();

				Vector3 rayPoint = centerPos + (up * posY) + (side * posX);
				Vector3 rayDir = (rayPoint - rayStart).normalized;

				if (Physics.Raycast(new Ray(rayStart, rayDir), out hit,
									999999.0f, CameraRaycastLayers.value))
				{
					PlayerController player = hit.collider.GetComponent<PlayerController>();
					if (player != null && player != this)
					{
						score += invNCasts;
					}
				}
			}
		}

		//If this wasn't a scoring photograph, remove it.
		if (score == 0.0f)
		{
			Photos.RemoveAt(Photos.Count - 1);
		}
		//Otherwise, add the score to the total.
		else
		{
			PhotoScores.Add(score);
		}


		//Wait for the cooldown to finish, then reset.
		yield return new WaitForSeconds(PhotoInterval);
		CanTakePhotos = true;
		PhotoReticule.gameObject.SetActive(true);
		PolaroidObj.gameObject.SetActive(false);
	}
}