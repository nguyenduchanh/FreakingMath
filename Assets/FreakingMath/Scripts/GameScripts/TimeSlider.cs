using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeSlider : MonoBehaviour 
{
	public static TimeSlider instance;
	public Image TimeSliderImage;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public void ResetTimeSlider()
	{
		iTween.Stop (gameObject);
		TimeSliderImage.fillAmount = 1F;
	}

	public void UpdateTimeSlider()
	{
		iTween.Stop (gameObject);
		TimeSliderImage.fillAmount = 1F;
		iTween.ValueTo (gameObject, iTween.Hash ("from", 100, "to", 0, "easeType", iTween.EaseType.linear, "onupdate", "OnUpdateTimeSlider", "time",  GamePlay.instance.AnswerTime, "oncomplete", "OnTimeOver", "oncompletetarget", gameObject));
	}

	public void PauseTimer()
	{
		iTween.Pause (gameObject);
	}

	public void ResumeTimer()
	{
		iTween.Resume (gameObject);
	}
	
	void OnUpdateTimeSlider(int val)
	{
		TimeSliderImage.fillAmount = (float)(val / 100F);
	}
	
	void OnTimeOver()
	{
		if(GamePlay.instance.isGamePlay)
		{
			if(GameController.instance.isSoundAvailble)
			{
				GamePlay.instance.GetComponent<AudioSource>().PlayOneShot(GamePlay.instance.soundIncorrect);
			}
			GamePlay.instance.OnIncorrectOrLateAnswer();
		}
	}
}
