using System;
using System.Collections.Generic;
using UnityEngine;


public class MainMenu : Singleton<MainMenu>
{
	public void OnButton_Play()
	{

	}
	public void OnButton_Quit()
	{
		Application.Quit();
	}
}