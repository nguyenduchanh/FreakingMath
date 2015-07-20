using UnityEngine;
using System.Collections;

/// Main screen.

public class MainScreen : MonoBehaviour 
{
	public static MainScreen instance;
	public GameObject GamePlayObject;

	/// Awake this instance.
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
	}

	/// Raises the enable event.
	void OnEnable()
	{
		InputManager.OnBackButtonPressedEvent += OnBackButtonPressed;

		if(PlayerPrefs.GetInt("RemoveAds",0) == 1)
		{
			GameObject.Find("btn-RemoveAds").SetActive(false);
		}
	}

	/// Raises the disable event.
	void OnDisable()
	{
		InputManager.OnBackButtonPressedEvent -= OnBackButtonPressed;
	}

	/// Raises the back button pressed event.
	void OnBackButtonPressed ()
	{
		if(GameController.instance.isTouchAvailable)
		{
			Application.Quit ();
		}
	}

	/// Raises the play button pressed event.
	public void OnPlayButtonPressed()
	{
		if(GameController.instance.isTouchAvailable)
		{
			GameController.instance.DisableTouchForDelay();
			GameController.instance.AddButtonTouchEffect();
			StartCoroutine(LoadGamePlayFromMainScreen());
		}
	}
	

	IEnumerator LoadGamePlayFromMainScreen()
	{
		GamePlayObject.SetActive(true);
		iTween.MoveTo(gameObject, iTween.Hash("y",-10F * UIAspectManager.AspectMultiplier ,"time",0.5F,"easeType",iTween.EaseType.linear));
		yield return new WaitForSeconds(0.6F);
		gameObject.SetActive (false);
	}
}
