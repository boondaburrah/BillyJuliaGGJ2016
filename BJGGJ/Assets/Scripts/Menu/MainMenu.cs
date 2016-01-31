using System;
using System.Collections.Generic;
using UnityEngine;


public class MainMenu : Singleton<MainMenu>
{
	public void OnButton_Play()
	{
		AudioSource.PlayClipAtPoint(Sounds.Instance.UIClick, Vector3.zero);
		UnityEngine.SceneManagement.SceneManager.LoadScene("Controls Menu");
	}
	public void OnButton_Quit()
	{
		AudioSource.PlayClipAtPoint(Sounds.Instance.UIClick, Vector3.zero);
		Application.Quit();
	}
}