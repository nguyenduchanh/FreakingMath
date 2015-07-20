using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class UICamera : MonoBehaviour 
{
	float thisCameraOrthoSize = 5F;

	void Awake()
	{
		if(UIAspectManager.instance == null)
		{
			UIAspectManager.init();
		}

		thisCameraOrthoSize = GetComponent<Camera>().orthographicSize;
		UpdateCameraOrthoSize ();
	}

	void UpdateCameraOrthoSize()
	{
		if(Screen.height > Screen.width)
		{
			if(GetComponent<Camera>().orthographic)
			{
				GetComponent<Camera>().orthographicSize = (thisCameraOrthoSize * UIAspectManager.AspectMultiplier);
			}
		}
	}


	void OnResolutionUpdated ()
	{
		UpdateCameraOrthoSize ();
	}
}
