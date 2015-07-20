using UnityEngine;
using System.Collections;


/// Game controller.

public class GameController : MonoBehaviour 
{
	public static GameController instance;

	public Camera UICamera;
	public AudioClip ClickSound;
	public AudioSource BackgroundMusic;

	public bool isSoundAvailble = true;
	public bool isTouchAvailable = true;

	public int sessionCount = 0;

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

		isSoundAvailble = (PlayerPrefs.GetInt ("isSoundAvailble", 0) == 0) ? true : false;

		if(isSoundAvailble)
		{
			BackgroundMusic.Play();
		}
	}
	/// Start this instance.
	
	void Start()
	{
		sessionCount = PlayerPrefs.GetInt ("sessionCount", 0) + 1;
		PlayerPrefs.SetInt ("sessionCount", sessionCount);
	}

	/// Disables the touch for delay.
	public void DisableTouchForDelay(float delay = 0.7F)
	{
		StopCoroutine ("DeactivateTouch");
		StartCoroutine ("DeactivateTouch", delay);
	}

	/// Deactivates the touch.
	IEnumerator DeactivateTouch(float delay)
	{
		isTouchAvailable = false;
		yield return new WaitForSeconds(delay);
		isTouchAvailable = true;
	}

	/// Adds the button touch effect.
	public void AddButtonTouchEffect()
	{
		if(isSoundAvailble)
		{
			GetComponent<AudioSource>().PlayOneShot(ClickSound);
		}
	}

	/// Adds the button touch effect.
	public void AddButtonTouchEffect(GameObject btn)
	{
		if(isSoundAvailble)
		{
			GetComponent<AudioSource>().PlayOneShot(ClickSound);
		}
		iTween.PunchScale (btn, iTween.Hash ("x", -0.3F, "y", 0.3, "time", 0.7F, "easetype", "linear"));
	}

	
	/// Spawns the prefab from resources.
	public GameObject SpawnPrefabFromResources(string path)
	{
		GameObject thisObject = (GameObject)Instantiate (Resources.Load (path));
		thisObject.name = thisObject.name.Replace ("(Clone)", "");
		return thisObject;
	}

	/// Toggles the sound.
	public void ToggleSound()
	{
		isSoundAvailble = !isSoundAvailble;
		PlayerPrefs.SetInt ("isSoundAvailble", (isSoundAvailble) ? 0 : 1);

		if(isSoundAvailble)
		{
			BackgroundMusic.Play();
		}
		else
		{
			BackgroundMusic.Stop();
		}
	}

	/// Raises the application pause event.
	void OnApplicationPause(bool paused)
	{
		if(!paused)
		{
			if(GameObject.Find("MainScreen") != null)
			{
				sessionCount = PlayerPrefs.GetInt ("sessionCount", 0) + 1;
				PlayerPrefs.SetInt ("sessionCount", sessionCount);
			}
		}
		else
		{
		}
	}
}
