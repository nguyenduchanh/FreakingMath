using UnityEngine;
using System.Collections;
using System;

public class UIAspectManager : MonoBehaviour 
{
	public static UIAspectManager instance;

	public static float AspectMultiplier = 1F;

	float BaseAspectRatio = 1.5F;

	public static void init()
	{
		if(instance == null)
		{
			GameObject aspectManager = new GameObject ("UIAspectManager");
			aspectManager.hideFlags = HideFlags.HideInHierarchy;
			aspectManager.AddComponent<UIAspectManager> ();
		}
	}

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		CalculateAspect ();
	}

	void CalculateAspect()
	{
		if(Screen.height > Screen.width)
		{
			AspectMultiplier = ((( float ) ( Screen.height ) / ( float ) ( Screen.width )) / ( BaseAspectRatio ));
		}
		else
		{
			AspectMultiplier = ((( float ) ( Screen.width ) / ( float ) ( Screen.height )) / ( BaseAspectRatio ));
		}
	}
}
