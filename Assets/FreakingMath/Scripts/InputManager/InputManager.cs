using UnityEngine;
using System.Collections;
using System;

public class InputManager : MonoBehaviour 
{
	public static event Action<Vector2> OnMouseDownEvent;
	public static event Action<Vector2> OnMouseUpEvent;
	public static event Action OnBackButtonPressedEvent;

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if(OnBackButtonPressedEvent != null)
			{
				OnBackButtonPressedEvent();
			}
		}
	}

	void OnGUI()
	{
		#if UNITY_EDITOR || UNITY_METRO || UNITY_STANDALONE
		if(Event.current.type == EventType.MouseDown)
		{
			if(OnMouseDownEvent != null)
			{
				OnMouseDownEvent(Input.mousePosition);
			}
		}
		if(Event.current.type == EventType.MouseUp)
		{
			if(OnMouseUpEvent != null)
			{
				OnMouseUpEvent(Input.mousePosition);
			}
		}
		#endif
	}
}
