using UnityEngine;
using System.Collections;

public class UIAspectUpdator : MonoBehaviour 
{
	Vector3 thisPosition;

	void Awake()
	{
		if(UIAspectManager.instance == null)
		{
			UIAspectManager.init();
		}
		thisPosition = transform.localPosition;
		UpdateUIPosition ();
	}

	void UpdateUIPosition()
	{
		if(Screen.height > Screen.width)
		{
			transform.localPosition = new Vector3(thisPosition.x, thisPosition.y * UIAspectManager.AspectMultiplier, thisPosition.z); 
		}
		else
		{
			transform.localPosition = new Vector3(thisPosition.x * UIAspectManager.AspectMultiplier, thisPosition.y, thisPosition.z); 
		}
	}

	
	void OnResolutionUpdated ()
	{
		UpdateUIPosition ();
	}
}
