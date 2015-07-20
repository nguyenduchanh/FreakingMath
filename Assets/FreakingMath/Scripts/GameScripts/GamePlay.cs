using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class GamePlay : MonoBehaviour 
{
	public static GamePlay instance;

	public float AnswerTime = 1.2F;
	public bool isGamePlay = true;

	public Text txtScore;
	public Text txtBestScore;
	public Text txtEquation;
	public Text txtEquationResult;

	public Text txtTitle_GameOver;
	public Text txtScore_GameOver;
	public Text txtBestScore_GameOver;

	public Image RootPanelImage;

	public RectTransform GameOverScreen;
	public RectTransform RescueScreen;

	public GameObject LoadingScreen;
	public GameObject MainScreen;
	
	public AudioClip soundCorrect;
	public AudioClip soundIncorrect;
	public AudioClip soundTap;

	public Image SoundImage;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;

	int score = 0;
	int bestScore = 0;
	int totalMatches = 1;

	EquationGenerator equation;

	int GameOverReason = 0; 

	public bool AllowRescueGame = true;
	public int MinScoreToRescue = 4;
	public int MaxAvailableRescueInOneGame = 1;
	
	int RescueCount = 0;
	


	// Awake this instance.
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

	/// Raises the enable event.
	void OnEnable()
	{
		InputManager.OnBackButtonPressedEvent += OnBackButtonPressed;
		score = 0;
		txtScore.text = "SCORE : " + score.ToString ();
		isGamePlay = true;
		bestScore = PlayerPrefs.GetInt ("bestScore", 0);
		totalMatches = PlayerPrefs.GetInt ("totalMatches", 1);
		txtBestScore.text = "BEST : " + bestScore.ToString ();
		StartCoroutine(GenerateNewEquation (true));
		SoundImage.sprite = (GameController.instance.isSoundAvailble) ? soundOnSprite : soundOffSprite;
	}

	/// Raises the disable event.
	void OnDisable()
	{
		InputManager.OnBackButtonPressedEvent -= OnBackButtonPressed;
	}
		
	/// Raises the back button pressed event.
	void OnBackButtonPressed()
	{
		if(GameController.instance.isTouchAvailable)
		{
			GameController.instance.DisableTouchForDelay();
			StartCoroutine(LoadMainSceenFromGamePlay());
		}
	}

	/// Updates the score.
	public void UpdateScore()
	{
		score += 1;
		txtScore.text = "SCORE : " + score.ToString ();

		if(score > bestScore)
		{
			bestScore = score;
			PlayerPrefs.SetInt("bestScore",bestScore);
			txtBestScore.text = "BEST : " + bestScore.ToString();
		}
	}

	//Checks for rescue.

	public void CheckForRescue(int reason)
	{
		GameOverReason = reason;
		TimeSlider.instance.ResetTimeSlider();
		StartCoroutine(OnGameOver(reason));
	}

	/// Loads the rescue screen.

	public IEnumerator LoadRescueScreen(int reason)
	{
		if(isGamePlay)
		{
			isGamePlay = false;
			GameController.instance.DisableTouchForDelay();
			RescueScreen.gameObject.SetActive(true);
			iTween.MoveTo(RescueScreen.gameObject, iTween.Hash("y",4.5F * UIAspectManager.AspectMultiplier, "time",0.3F,"easetype",iTween.EaseType.linear));
			yield return new WaitForSeconds(0.3F);
		}
	}

	/// Raises the game over event.

	public IEnumerator OnGameOver(int reason, bool RejectedRescue = false)
	{
		if(isGamePlay || RejectedRescue)
		{
			totalMatches += 1;
			PlayerPrefs.SetInt("totalMatches",totalMatches);

			TimeSlider.instance.ResetTimeSlider();
			GameController.instance.DisableTouchForDelay();

			txtTitle_GameOver.text = (reason == 0) ? "GAME OVER" :"TIME OVER";
			txtScore_GameOver.text = score.ToString();
			txtBestScore_GameOver.text = bestScore.ToString();

			isGamePlay = false;
			GameOverScreen.gameObject.SetActive(true);

			for(int posY  = 600; posY >= -30; posY -= 15)
			{
				GameOverScreen.anchoredPosition = new Vector2(0,posY);
				yield return new WaitForSeconds(0.001F);
			}
			RescueCount = 0;
		}
	}

	// Loads the main sceen from game play.

	public IEnumerator LoadMainSceenFromGamePlay()
	{
		MainScreen.SetActive (true);
		iTween.MoveTo(MainScreen, iTween.Hash("y",0F ,"time",0.5F,"easeType",iTween.EaseType.linear));
		yield return new WaitForSeconds (0.5F);

		GameOverScreen.anchoredPosition = new Vector2 (0, 600);
		GameOverScreen.gameObject.SetActive (false);

		gameObject.SetActive (false);
	}

	/// Raises the correct button pressed event.
	public void OnCorrectButtonPressed()
	{
		if(isGamePlay)
		{
			if(equation.isEquationCorrect)
			{
				if(GameController.instance.isSoundAvailble)
				{
					GetComponent<AudioSource>().PlayOneShot(soundCorrect);
				}
				UpdateScore();
				StartCoroutine(GenerateNewEquation ());
				TimeSlider.instance.UpdateTimeSlider();
			}
			else
			{
				if(GameController.instance.isSoundAvailble)
				{
					GetComponent<AudioSource>().PlayOneShot(soundIncorrect);
				}
				OnIncorrectOrLateAnswer();
			}
		}
	}

	/// Raises the incorrect button pressed event.
	public void OnIncorrectButtonPressed()
	{
		if(isGamePlay)
		{
			if(!equation.isEquationCorrect)
			{
				if(GameController.instance.isSoundAvailble)
				{
					GetComponent<AudioSource>().PlayOneShot(soundCorrect);
				}
				UpdateScore();
				StartCoroutine(GenerateNewEquation ());
				TimeSlider.instance.UpdateTimeSlider();
			}
			else
			{
				if(GameController.instance.isSoundAvailble)
				{
					GetComponent<AudioSource>().PlayOneShot(soundIncorrect);
				}
				OnIncorrectOrLateAnswer();
			}
		}
	}

	public void OnIncorrectOrLateAnswer()
	{
		if(AllowRescueGame)
		{
			CheckForRescue(0);
		}
		else
		{
			StartCoroutine(OnGameOver(0));
		}
	}

	/// Raises the play button pressed event.
	public void OnPlayButtonPressed()
	{
		if(GameController.instance.isTouchAvailable)
		{
			GameController.instance.DisableTouchForDelay();
			GameController.instance.AddButtonTouchEffect();
			StartCoroutine(RestartGamePlay ());
		}
	}

	/// Raises the menu button pressed event.

	public void OnMenuButtonPressed()
	{
		if(GameController.instance.isTouchAvailable)
		{
			GameController.instance.DisableTouchForDelay();
			GameController.instance.AddButtonTouchEffect();
			StartCoroutine(LoadMainSceenFromGamePlay());
		}
	}

	/// Raises the sound button pressed event.

	public void OnSoundButtonPressed(Image image)
	{
		if(GameController.instance.isTouchAvailable)
		{
			GameController.instance.DisableTouchForDelay();
			GameController.instance.AddButtonTouchEffect();

			GameController.instance.ToggleSound ();
			SoundImage.sprite = (GameController.instance.isSoundAvailble) ? soundOnSprite : soundOffSprite;
		}
	}



	
	/// Raises the give up button pressed event.
	public void OnGiveUpButtonPressed()
	{
		RescueScreen.anchoredPosition = new Vector2 (0, 600);
		RescueScreen.gameObject.SetActive (false);
		StartCoroutine (OnGameOver (GameOverReason,true));
	}

	/// Raises the rescue event.
	public IEnumerator OnRescue()
	{
		isGamePlay = true;
		LoadingScreen.SetActive (true);
		GameController.instance.DisableTouchForDelay(2F);
		
		iTween.MoveFrom(LoadingScreen, iTween.Hash("y",-(10F * UIAspectManager.AspectMultiplier),"time",0.5F,"easeType",iTween.EaseType.linear));
		iTween.MoveTo (LoadingScreen, iTween.Hash ("y", -(10F * UIAspectManager.AspectMultiplier),"delay",0.5F, "time", 0.5F, "easeType", iTween.EaseType.linear));
		
		yield return new WaitForSeconds (0.5F);
		
		RescueScreen.anchoredPosition = new Vector2 (0, 600);
		RescueScreen.gameObject.SetActive (false);
		
		yield return new WaitForSeconds (0.5F);
		
		LoadingScreen.transform.localPosition = Vector3.zero;
		LoadingScreen.SetActive (false);
		RescueCount ++;
		
        
	}

	/// Restarts the game play.
	public IEnumerator RestartGamePlay()
	{
		//hasRescuedGame = false;
		LoadingScreen.SetActive (true);
		GameController.instance.DisableTouchForDelay(2F);

		iTween.MoveFrom(LoadingScreen, iTween.Hash("y",-(10F * UIAspectManager.AspectMultiplier),"time",0.5F,"easeType",iTween.EaseType.linear));
		iTween.MoveTo (LoadingScreen, iTween.Hash ("y", -(10F * UIAspectManager.AspectMultiplier),"delay",0.5F, "time", 0.5F, "easeType", iTween.EaseType.linear));

		yield return new WaitForSeconds (0.5F);

		Color panelColor = new Color (UnityEngine.Random.Range (0.15F, 0.8F), UnityEngine.Random.Range (0.35F, 0.8F), UnityEngine.Random.Range (0.35F, 0.8F), 1F);
		RootPanelImage.color = panelColor;

		GameOverScreen.anchoredPosition = new Vector2 (0, 600);
		GameOverScreen.gameObject.SetActive (false);
		StartCoroutine(GenerateNewEquation (true));
		yield return new WaitForSeconds (0.5F);
		score = 0;
		txtScore.text = "SCORE : " + score.ToString ();
		isGamePlay = true;
		LoadingScreen.transform.localPosition = Vector3.zero;
		LoadingScreen.SetActive (false);
	}

	/// Generates the new equation.
	IEnumerator GenerateNewEquation(bool isFirstEquation = false)
	{
		if(!isFirstEquation)
		{
			iTween.MoveTo(txtEquation.gameObject,iTween.Hash("x",-5F,"time",0.1F,"easeType",iTween.EaseType.linear));
			iTween.MoveTo(txtEquationResult.gameObject,iTween.Hash("x",-5F,"time",0.1F,"easeType",iTween.EaseType.linear));
			yield return new WaitForSeconds(0.1F);
			txtEquation.rectTransform.anchoredPosition = new Vector2(500,txtEquation.rectTransform.anchoredPosition.y);
			txtEquationResult.rectTransform.anchoredPosition = new Vector2(500,txtEquationResult.rectTransform.anchoredPosition.y);
			iTween.MoveTo(txtEquation.gameObject,iTween.Hash("x",0F,"time",0.1F,"easeType",iTween.EaseType.linear));
			iTween.MoveTo(txtEquationResult.gameObject,iTween.Hash("x",0F,"time",0.1F,"easeType",iTween.EaseType.linear));
		}
		equation = new EquationGenerator (score);
		txtEquation.text = equation.equationValue1 + "" + equation.equationSign + "" + equation.equationValue2;
		txtEquationResult.text = "=" + equation.equationResult;
	}
}

/// Equation generator.
public class EquationGenerator
{
	public int equationValue1{ get; set; }
	public int equationValue2{ get; set; }

	public int equationResult{ get; set; }
	int equationType { get; set; }

	public bool isEquationCorrect{ get; set; }
	public string equationSign{ get; set; }


	int currentProgress{ get; set; }

	public EquationGenerator(int _currentProgress)
	{
		currentProgress = _currentProgress;

		/// if total success count is less than 10 - ADDITION equation will be generated
		/// if total success count is between 10 and 20 - ADDITION or SUBTRACTION equation will be generated
		/// if total success count is between 20 and 30 - ADDITION, SUBTRACTION or MULTIPLICATION equation will be generated
		/// if total success count is more than 30 - ADDITION, SUBTRACTION,MULTIPLICATION or DIVISION equation will be generated

		///for all the equation the number and difficulty will be adjusted randomly  based on the current user progress. 

		equationType = GenerateRandomEquation ();
		equationValue1 = GenerateRandomNumber ();
		equationValue2 = GenerateRandomNumber ();

		GenerateEquationResult ();
	}

	int GenerateRandomEquation()
	{
		//0 - ADDITION
		//1 - SUBTRACTION
		//2 - MULTIPLICATION
		//3 - DIVISION
		if(currentProgress <= 10)
		{
			return 0;
		}
		if(currentProgress > 10 && currentProgress <= 20)
		{
			return (UnityEngine.Random.Range(0,2));
		}
		else if(currentProgress > 20 && currentProgress <= 30)
		{
			return (UnityEngine.Random.Range(0,3));
		}
		else
		{
			return (UnityEngine.Random.Range(0,4));
		}		
	}

	int GenerateRandomNumber()
	{
		switch(equationType)
		{
		case 0:
			//ADDITION
			if(currentProgress <= 3)
			{
				return (UnityEngine.Random.Range(1,4));	
			}
			else if(currentProgress > 3 && currentProgress <= 10)
			{
				return (UnityEngine.Random.Range(1,7));	
			}
			else
			{
				return (UnityEngine.Random.Range(1,10));	
			}
		case 1:
			//SUBTRACTION
			if(currentProgress <= 20)
			{
				return (UnityEngine.Random.Range(1,4));	
			}
			else if(currentProgress <= 30)
			{
				return (UnityEngine.Random.Range(1,7));	
			}
			else
			{
				return (UnityEngine.Random.Range(1,10));	
			}
		case 2:
			//MULTIPLICATION
			if(currentProgress <= 30)
			{
				return (UnityEngine.Random.Range(1,4));	
			}
			else if(currentProgress > 30 && currentProgress <= 40)
			{
				return (UnityEngine.Random.Range(1,7));	
			}
			else
			{
				return (UnityEngine.Random.Range(1,10));	
			}
		case 3:
			//DIVISION
			if(currentProgress <= 40)
			{
				return (UnityEngine.Random.Range(1,4));	
			}
			else
			{
				return (UnityEngine.Random.Range(1,6));	
			}
		}
		return 0;
	}

	void GenerateEquationResult()
	{
		isEquationCorrect = (UnityEngine.Random.Range (0, 2) == 0) ? true : false;

		switch(equationType)
		{
		case 0:
			//ADDITION
			equationSign = "+";
			equationResult = equationValue1 + equationValue2;
			break;
		case 1:
			//SUBTRACTION
			//value 1 should be larger than value two to avoid nagative result

			equationSign = "-";
			if(equationValue1 < equationValue2)
			{
				equationValue1 = equationValue1 + equationValue2; 
				equationValue2 = equationValue1 - equationValue2; 
				equationValue1 = equationValue1 - equationValue2; 
			}
			equationResult = equationValue1 - equationValue2;
			//SUBTRACTION
			break;
		case 2:
			//MULTIPLICATION
			equationSign = "x";
			equationResult = equationValue1 * equationValue2;
			break;
		case 3:
			//DIVISION
			//value 1 should be divisable by value two to avoid floating point result.
			equationSign = "/";

			if(equationValue1 < equationValue2)
			{
				equationValue1 = equationValue1 + equationValue2; 
				equationValue2 = equationValue1 - equationValue2; 
				equationValue1 = equationValue1 - equationValue2; 
			}

			equationValue1 = equationValue1 * equationValue2;
			equationResult = equationValue1 / equationValue2;
			break;
		}

		if(!isEquationCorrect)
		{
			int resultDifference = (UnityEngine.Random.Range(1,4));
			equationResult = (equationResult > 7) ? (equationResult - resultDifference) : (equationResult + resultDifference);
		}
	}
}