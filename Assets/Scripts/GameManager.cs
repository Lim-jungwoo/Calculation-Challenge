using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

static class Constants
{
	public const string oneDayChallengeSignStr = "OneDayChallenge";
	public const string additionSignStr = "Addition";
	public const string subtractionSignStr = "Subtraction";
	public const string multiplicationSignStr = "Multiplication";
	public const string divisionSignStr = "Division";
	public const string level = "level";
	public const string level0 = "Hardworking Person";
	public const string level1 = "Normal Person";
	public const string level2 = "Little Smart Person";
	public const string level3 = "Smart Person";
	public const string level4 = "Very Smart Person";
	public const string level5 = "Extremely Smart Person";
	public const string level6 = "The Smartest Person";
	public const char plusSign = '+';
	public const char minusSign = '-';
	public const char multipleSign = 'x';
	public const char divideSign = '÷';
	public const string backgroundSoundVolume = "BackgroundSoundVolume";
	public const string clockSoundVolume = "ClockSoundVolume";
	public const string soundEffectVolume = "SoundEffectVolume";
	public const int maxOneDayChallengeQuizStage = 20;
	public const int maxQuizStage = 10;
	public const string today = "Today";
	public const string total = "Total";
	public const string totalClearNum = "Total Clear Number";
	public const string totalCorrectNum = "Total Correct Number";
	public const string totalWrongNum = "Total Wrong Number";
	public const string totalCorrectAnswerRatio = "Total Correct Answer Ratio";
	public const string todayClearNum = "tTday Clear Number";
	public const string todayCorrectNum = "Today Correct Number";
	public const string todayWrongNum = "Today Wrong Number";
	public const string todayCorrectAnswerRatio = "Today Correct Answer Ratio";
	public const string todayDateDay = "Today Date Day";
	public const string chance = "chance";
}

public class GameManager : MonoBehaviour
{
	public GameObject mainCamera, panel, coverImage;
	public GameObject mainMenu;
	public GameObject sideMenu;
	public GameObject operationChoiceMenu;
	public GameObject quizMenu, quizChoiceMenu, quiz;
	public GameObject quizContent, answerObject;
	public GameObject additionQuizChoice, subtractionQuizChoice, multiplicationQuizChoice, divisionQuizChoice, oneDayChallengeQuizChoice;
	public GameObject operationSign;
	public Button enterButton;
	public GameObject correctText, answerText;
	public Slider stageSlider, timerSlider;
	public TextMeshProUGUI stageText, timerText;
	public GameObject finishWindow, quizName;
	public TextMeshProUGUI finalMentText, scoreAndTimeText, quizNameText;
	private int answer = 0;
	private string selectQuiz = "", challengeLevel = "", currentLevel = "";
	private int stage = 1, wrongCount = 0;
	private bool isPlaying = false;
	private float timer = 60f;
	private bool isTimerWorking = false, isWrongMessageWorking = false, isCorrectMessageWorking = false;
	private int num1Digit = 0, num2Digit = 0;
	private char operation = ' ';
	private float playingTime = 0;

	Coroutine timerCoroutine, correctMsgCoroutine, wrongMsgCoroutine;
	AudioSource audioSource, clockAudioSource, backgroundSource;
	public AudioClip correctAudio, wrongAudio, buttonClickAudio, numberButtonClickAudio, clockAudio, backgroundAudio, finishAudio, failAudio;
	public GameObject optionMenu;
	public Button optionButton, closeOptionButton, startOptionButton;
	public Slider backgroundMusicVolumeSlider, soundEffectVolumeSlider, clockSoundVolumeSlider;
	public GameObject checkExitMenu;
	public Button yesButton, noButton;
	public GameObject profileMenu;
	public Button profileTotalButton, profileTodayButton;
	public TextMeshProUGUI profileContentText;
	private int totalClearNum, totalCorrectNum, totalWrongNum;
	private float totalCorrectAnswerRatio;
	private int todayClearNum, todayCorrectNum, todayWrongNum;
	private float todayCorrectAnswerRatio;
	private bool isAlreadySetProfileTotalContent, isAlreadySetProfileTodayContent;
	public TextMeshProUGUI levelText;
	private int todayDateDay, playerPrefsDay;
	public Button copyrightButton, closeCopyrightButton;
	public GameObject copyrightContent;

	private int chance;
	public GameObject foregroundAD;

	private void Awake()
	{
		// PlayerPrefs.DeleteAll();

		// Get Component
		audioSource = mainCamera.GetComponent<AudioSource>();
		clockAudioSource = quizMenu.GetComponent<AudioSource>();
		backgroundSource = panel.GetComponent<AudioSource>();

		GoMainMenu();

		//
		enterButton.onClick.AddListener(EnterButtonClick);
		optionButton.onClick.AddListener(OptionButtonClick);
		closeOptionButton.onClick.AddListener(CloseOptionButtonClick);
		startOptionButton.onClick.AddListener(OptionButtonClick);
		backgroundMusicVolumeSlider.onValueChanged.AddListener(BackgroundMusicVolumeSliderChange);
		backgroundSource.clip = backgroundAudio;
		clockAudioSource.clip = clockAudio;
		clockSoundVolumeSlider.onValueChanged.AddListener(ClockSoundVolumeSliderChange);
		soundEffectVolumeSlider.onValueChanged.AddListener(SoundEffectVolumeSliderChange);
		BackgroundAudioPlay();

		//* PlayerPrefs에 저장되어 있던 음량 값을 불러온다.
		GetPlayerPrefsVolume(Constants.backgroundSoundVolume, backgroundSource, backgroundMusicVolumeSlider);
		GetPlayerPrefsVolume(Constants.clockSoundVolume, clockAudioSource, clockSoundVolumeSlider);
		GetPlayerPrefsVolume(Constants.soundEffectVolume, audioSource, soundEffectVolumeSlider);

		// set check exit menu
		yesButton.onClick.AddListener(CheckExitYesButtonClick);
		noButton.onClick.AddListener(CheckExitNoButtonClick);

		// get total profile
		GetProfileTotalContent();
		isAlreadySetProfileTotalContent = false;

		// get today profile
		//* If PlayerPrefsToday is not today, today Profile Content set zero
		todayDateDay = DateTime.Today.Day;
		GetPlayerPrefsTodayDateDay();
		if (todayDateDay != playerPrefsDay)
		{
			PlayerPrefs.SetInt(Constants.todayDateDay, todayDateDay);
			PlayerPrefs.DeleteKey(Constants.todayClearNum);
			PlayerPrefs.DeleteKey(Constants.todayCorrectNum);
			PlayerPrefs.DeleteKey(Constants.todayWrongNum);
			PlayerPrefs.DeleteKey(Constants.level);
		}
		GetProfileTodayContent();
		isAlreadySetProfileTodayContent = false;

		// set level
		GetPlayerPrefsLevel();
		challengeLevel = Constants.level0;

		copyrightButton.onClick.AddListener(CopyrightButtonClick);
		closeCopyrightButton.onClick.AddListener(CloseCopyrightButtonClick);

		// set chance
		GetChance();
	}

	public void PlayButtonClick()
	{
		ButtonClickAudioPlay();
		GoOperationChoiceMenu();
	}

	public void GoBackButtonClick()
	{
		ButtonClickAudioPlay();

		// operationChoiceMenu가 Active인 상태라면 operation을 고르는 메뉴이므로 뒤로 가면 메인 메뉴로 나가야 한다.
		if (operationChoiceMenu.activeSelf == true)
		{
			GoMainMenu();
		}
		// quizChoiceMenu가 Active인 상태라면 operation을 고르는 메뉴로 나간다.
		else if (quizChoiceMenu.activeSelf == true)
		{
			GoOperationChoiceMenu();
		}
		// profileMenu가 Active일 때 메인 메뉴로 넘어간다.
		else if (profileMenu.activeSelf == true)
		{
			GoMainMenu();
		}
		// 문제를 풀고 있을 때 뒤로 가면 quizChoiceMenu로 나간다.
		// quizChoiceMenu로 나가기 전에 진짜로 문제를 그만 풀지 확인하는 창을 띄운다.
		else
		{
			// 그만 풀지 확인하는 창을 띄운다.
			CheckExitMenu();
		}
	}

	private void GoOperationChoiceMenu()
	{
		AllObjectDown();
		sideMenu.SetActive(true);
		operationChoiceMenu.SetActive(true);
	}

	/// <summary>모든 오브젝트를 끄고, 메인 메뉴만 활성화한다.</summary>
	private void GoMainMenu()
	{
		AllObjectDown();
		mainMenu.SetActive(true);
	}

	private void GoQuizChoiceMenu()
	{
		AllObjectDown();
		sideMenu.SetActive(true);
		quizChoiceMenu.SetActive(true);
		quizName.SetActive(true);
		InitializeQuiz();
		switch (selectQuiz)
		{
			case Constants.oneDayChallengeSignStr:
				oneDayChallengeQuizChoice.SetActive(true);
				quizNameText.text = Constants.oneDayChallengeSignStr;
				break;
			case Constants.additionSignStr:
				additionQuizChoice.SetActive(true);
				quizNameText.text = Constants.additionSignStr;
				break;
			case Constants.subtractionSignStr:
				subtractionQuizChoice.SetActive(true);
				quizNameText.text = Constants.subtractionSignStr;
				break;
			case Constants.multiplicationSignStr:
				multiplicationQuizChoice.SetActive(true);
				quizNameText.text = Constants.multiplicationSignStr;
				break;
			case Constants.divisionSignStr:
				divisionQuizChoice.SetActive(true);
				quizNameText.text = Constants.divisionSignStr;
				break;
			default:
				Debug.Log("Go Quiz Choice Menu method's selectQuiz string has error");
				break;
		}
	}

	public void ProfileButtonClick()
	{
		GoProfileMenu();
	}

	private void GoProfileMenu()
	{
		AllObjectDown();
		profileMenu.SetActive(true);
		sideMenu.SetActive(true);

		// First, profileMenu shows Total Menu
		profileTotalButton.interactable = false;
		profileTodayButton.interactable = true;
		ShowProfileContent(Constants.total);
	}

	private void InitializeQuiz()
	{
		stage = 1;
		num1Digit = 0;
		num2Digit = 0;
		operation = ' ';
		playingTime = 0;
		wrongCount = 0;

		switch (selectQuiz)
		{
			case Constants.oneDayChallengeSignStr:
				stageSlider.maxValue = Constants.maxOneDayChallengeQuizStage;
				break;
			default:
				stageSlider.maxValue = Constants.maxQuizStage;
				break;
		}
		ResetQuiz();
	}

	private void ResetQuiz()
	{
		answerObject.GetComponent<TextMeshProUGUI>().text = "";
		operationSign.GetComponent<TextMeshProUGUI>().text = "";
		correctText.SetActive(false);
		quiz.SetActive(true);
		stageSlider.value = stage;
		stageText.text = "Stage " + stage.ToString() + "/" + (selectQuiz == Constants.oneDayChallengeSignStr ? Constants.maxOneDayChallengeQuizStage : Constants.maxQuizStage);
		SetTimer(30);
		answer = 0;
		isCorrectMessageWorking = false;
		isWrongMessageWorking = false;
	}

	private void SetTimer(int timerValue)
	{
		timer = timerValue;
		timerSlider.value = timer;
		timerSlider.maxValue = timer;
		timerText.text = timer.ToString() + "s";
	}

	/// <summary>모든 오브젝트를 끈다.</summary>
	private void AllObjectDown()
	{
		mainMenu.SetActive(false);
		sideMenu.SetActive(false);
		operationChoiceMenu.SetActive(false);
		quizMenu.SetActive(false);
		quizChoiceMenu.SetActive(false);
		oneDayChallengeQuizChoice.SetActive(false);
		additionQuizChoice.SetActive(false);
		subtractionQuizChoice.SetActive(false);
		multiplicationQuizChoice.SetActive(false);
		divisionQuizChoice.SetActive(false);
		finishWindow.SetActive(false);
		quizName.SetActive(false);
		optionMenu.SetActive(false);
		coverImage.SetActive(false);
		correctText.SetActive(false);
		answerText.SetActive(false);

		checkExitMenu.SetActive(false);

		profileMenu.SetActive(false);

		copyrightContent.SetActive(false);
	}

	public void OperationChoiceButtonClick(string operationName)
	{
		ButtonClickAudioPlay();
		if (operationChoiceMenu.activeSelf == false)
		{
			Debug.Log("operation choice menu is not active");
			return;
		}

		switch (operationName)
		{
			case Constants.oneDayChallengeSignStr:
				selectQuiz = Constants.oneDayChallengeSignStr;
				break;
			case Constants.additionSignStr:
				selectQuiz = Constants.additionSignStr;
				break;
			case Constants.subtractionSignStr:
				selectQuiz = Constants.subtractionSignStr;
				break;
			case Constants.multiplicationSignStr:
				selectQuiz = Constants.multiplicationSignStr;
				break;
			case Constants.divisionSignStr:
				selectQuiz = Constants.divisionSignStr;
				break;
			default:
				Debug.Log("operation choice name is invalid");
				GoMainMenu();
				return;
		}
		GoQuizChoiceMenu();
	}

	public void QuizButtonClick()
	{
		// If chance is 0, ad play
		if (chance <= 0)
		{
			ADButtonClick();
			return;
		}
		ButtonClickAudioPlay();
		BackgroundAudioStop();
		AllObjectDown();
		sideMenu.SetActive(true);
		quizMenu.SetActive(true);

		switch (selectQuiz)
		{
			case Constants.oneDayChallengeSignStr:
				operation = ' ';
				break;
			case Constants.additionSignStr:
				operation = Constants.plusSign;
				break;
			case Constants.subtractionSignStr:
				operation = Constants.minusSign;
				break;
			case Constants.multiplicationSignStr:
				operation = Constants.multipleSign;
				break;
			case Constants.divisionSignStr:
				operation = Constants.divideSign;
				break;
		}
		SetChance(--chance);
		QuizPlay();
	}

	private void QuizPlay()
	{
		// create num1 & insert quiz Content text
		int num1 = 0;
		NumberGenerate(num1Digit, out num1);
		if (num1 <= 0)
		{
			Debug.Log($"num1Digit : {num1Digit} num1 generation has error");
			return;
		}
		quizContent.GetComponent<TextMeshProUGUI>().text = num1.ToString();
		answer = num1;

		QuizGenerate();
		SetTimer(30);
		// 15stage까지 Quiz를 진행한다.
		// 10stage부터는 숫자 3개를 계산한다.
		if (stage > Constants.maxQuizStage)
		{
			SetTimer(45);
			QuizGenerate();
		}
		// 15Stage부터는 숫자 4개를 계산한다.
		if (stage > Constants.maxOneDayChallengeQuizStage - 5)
		{
			SetTimer(60);
			QuizGenerate();
		}

		isPlaying = true;
		timerCoroutine = StartCoroutine(StartTimer());
	}

	private void QuizGenerate()
	{
		string quizStr = quizContent.GetComponent<TextMeshProUGUI>().text;
		string operationSignStr = operationSign.GetComponent<TextMeshProUGUI>().text;
		int num2 = 0;
		char realOperation = operation;

		NumberGenerate(num2Digit, out num2);
		if (num2 <= 0)
		{
			Debug.Log("Number generate has error in quiz Generate");
			return;
		}

		// If quiz is one day challenge, default operation is ' ';
		// so, generate random operation
		string operationRandomStr = "+-÷x";
		int operationIndex = 0;
		if (realOperation == ' ')
		{
			operationIndex = UnityEngine.Random.Range(0, 4);
			realOperation = operationRandomStr[operationIndex];
		}

		quizStr += "\n" + num2.ToString();
		operationSignStr += "\n" + realOperation;
		quizContent.GetComponent<TextMeshProUGUI>().text = quizStr;
		operationSign.GetComponent<TextMeshProUGUI>().text = operationSignStr;
		calculateAnswer(num2, realOperation);
	}

	private void calculateAnswer(int num, char operation)
	{
		switch (operation)
		{
			case Constants.plusSign:
				answer += num;
				break;
			case Constants.minusSign:
				answer -= num;
				break;
			case Constants.multipleSign:
				answer *= num;
				break;
			case Constants.divideSign:
				answer /= num;
				break;
		}
	}

	// num의 digit에 따라 num을 구하는 함수이다.
	// Digit의 값이 이상하면 num은 -1이 된다.
	private void NumberGenerate(int numDigit, out int num)
	{
		switch (numDigit)
		{
			case 1:
				num = UnityEngine.Random.Range(1, 9);
				break;
			case 2:
				num = UnityEngine.Random.Range(10, 99);
				break;
			case 3:
				num = UnityEngine.Random.Range(100, 999);
				break;
			case 4:
				num = UnityEngine.Random.Range(1000, 9999);
				break;
			default:
				num = -1;
				break;
		}
	}

	public void InitializeNumberOneDigit(int numDigit)
	{
		num1Digit = numDigit;
	}
	public void InitializeNumberTwoDigit(int numDigit)
	{
		num2Digit = numDigit;
	}

	public void NumberButtonClick(string input)
	{
		if (CheckCorrectMessageCoroutineWorking() == true || CheckWrongMessageCoroutineWorking() == true)
		{
			return;
		}

		string text = answerObject.GetComponent<TextMeshProUGUI>().text;
		NumberButtonClickAudioPlay();
		switch (input)
		{
			case "Minus":
				if (text != "" && text[0] == '-')
				{
					answerObject.GetComponent<TextMeshProUGUI>().text = text.Substring(1, text.Length - 1);
				}
				else if (text == "")
				{
					answerObject.GetComponent<TextMeshProUGUI>().text += '-';
				}
				else
				{
					answerObject.GetComponent<TextMeshProUGUI>().text = '-' + text;
				}
				break;
			case "Delete":
				answerObject.GetComponent<TextMeshProUGUI>().text = text.Substring(0, text.Length - 1);
				break;
			default:
				answerObject.GetComponent<TextMeshProUGUI>().text += input;
				break;
		}
	}

	public void EnterButtonClick()
	{
		string inputAnswerStr = answerObject.GetComponent<TextMeshProUGUI>().text;
		int inputAnswer = 0;
		try
		{
			inputAnswer = int.Parse(inputAnswerStr);
		}
		catch (System.Exception)
		{
			Debug.Log("input answer changes to int has error");
		}

		if (inputAnswer == answer)
		{
			if (CheckCorrectMessageCoroutineWorking() == false)
			{
				stage++;
				correctMsgCoroutine = StartCoroutine(CorrectMessage());
			}
		}
		// If answer is not correct, try again
		else
		{
			if (CheckWrongMessageCoroutineWorking() == false)
				wrongMsgCoroutine = StartCoroutine(WrongMessage());
		}
	}

	private void QuizFinish(string finalMentStr)
	{
		isCorrectMessageWorking = false;
		isWrongMessageWorking = false;
		quizMenu.SetActive(false);
		finishWindow.SetActive(true);
		ClockAudioStop();
		FinishAudioPlay();

		int score = stage - 1;
		int playingTimeInt = (int)playingTime;
		finalMentText.text = finalMentStr;
		scoreAndTimeText.text = "Your Score\n" + score.ToString() + "\n\nPlaying Time\n" + playingTimeInt.ToString() + "s" + "\n\nWrong Count\n" + wrongCount;

		SetProfileTotalContent();
		SetProfileTodayContent();
		isAlreadySetProfileTotalContent = true;
		isAlreadySetProfileTodayContent = true;
	}

	public void SetChallengeLevel(string level)
	{
		challengeLevel = level;
	}

	IEnumerator WrongMessage()
	{
		isWrongMessageWorking = true;
		answerObject.GetComponent<TextMeshProUGUI>().text = "Please try again";
		wrongCount++;
		WrongAudioPlay();
		yield return new WaitForSeconds(1);
		answerObject.GetComponent<TextMeshProUGUI>().text = "";
		isWrongMessageWorking = false;

		yield break;
	}

	private bool CheckWrongMessageCoroutineWorking()
	{
		return (isWrongMessageWorking == true ? true : false);
	}

	IEnumerator CorrectMessage()
	{
		isCorrectMessageWorking = true;
		ClockAudioStop();
		correctText.SetActive(true);
		quiz.SetActive(false);

		StopTimer();
		CorrectAudioPlay();
		yield return new WaitForSeconds(1);

		// If quiz is One day Challenge, max stage is 20
		// If quiz is normal Quiz, max stage is 10
		if (stage > Constants.maxQuizStage)
		{
			switch (selectQuiz)
			{
				case Constants.oneDayChallengeSignStr:
					if (stage > Constants.maxOneDayChallengeQuizStage)
					{
						QuizFinish("Clear!");
						totalClearNum++;
						todayClearNum++;
						PlayerPrefs.SetInt(Constants.totalClearNum, totalClearNum);
						PlayerPrefs.SetInt(Constants.todayClearNum, todayClearNum);
						PlayerPrefs.SetString(Constants.level, challengeLevel);
						GetPlayerPrefsLevel();
						yield break;
					}
					break;
				default:
					QuizFinish("Perfect!");
					yield break;
			}
		}

		ResetQuiz();
		QuizPlay();
		isCorrectMessageWorking = false;
		yield break;
	}

	private void StopCorrectMessage()
	{
		if (isCorrectMessageWorking == true)
		{
			isCorrectMessageWorking = false;
			StopCoroutine(correctMsgCoroutine);
		}
	}

	private bool CheckCorrectMessageCoroutineWorking()
	{
		return (isCorrectMessageWorking == true ? true : false);
	}

	IEnumerator StartTimer()
	{
		ClockAudioPlay();
		isTimerWorking = true;
		while (timer > 0 && isPlaying == true)
		{
			timer -= Time.deltaTime;
			timerSlider.value = timer;
			timerText.text = ((int)timer).ToString() + "s";
			playingTime += Time.deltaTime;
			yield return null;
		}
		isTimerWorking = false;
		CreateAnswerTextMessage();
		answerText.SetActive(true);
		quiz.SetActive(false);
		ClockAudioStop();
		FailAudioPlay();
		yield return new WaitForSeconds(3);
		answerText.SetActive(false);
		QuizFinish("Well Done!");
		yield break;
	}

	private void StopTimer()
	{
		if (isTimerWorking == true)
		{
			isTimerWorking = false;
			StopCoroutine(timerCoroutine);
		}
	}

	private void CreateAnswerTextMessage()
	{
		string answerTextMsg = "Answer\n" + answer.ToString();
		answerText.GetComponent<TextMeshProUGUI>().text = answerTextMsg;
	}

	public void QuitButtonClick()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
	}

	private void OptionButtonClick()
	{
		ButtonClickAudioPlay();
		coverImage.SetActive(true);
		optionMenu.SetActive(true);
	}

	private void CloseOptionButtonClick()
	{
		ButtonClickAudioPlay();
		coverImage.SetActive(false);
		optionMenu.SetActive(false);
	}

	private void CheckExitMenu()
	{
		ButtonClickAudioPlay();
		coverImage.SetActive(true);
		checkExitMenu.SetActive(true);
	}

	private void CheckExitYesButtonClick()
	{
		ButtonClickAudioPlay();
		coverImage.SetActive(false);
		checkExitMenu.SetActive(false);

		if (isAlreadySetProfileTodayContent == false)
		{
			SetProfileTodayContent();
		}
		if (isAlreadySetProfileTotalContent == false)
		{
			SetProfileTotalContent();
		}
		isAlreadySetProfileTodayContent = false;
		isAlreadySetProfileTotalContent = false;

		BackgroundAudioPlay();
		GoQuizChoiceMenu();
		StopTimer();
		ClockAudioStop();
	}

	private void CheckExitNoButtonClick()
	{
		ButtonClickAudioPlay();
		coverImage.SetActive(false);
		checkExitMenu.SetActive(false);
	}

	private void CopyrightButtonClick()
	{
		ButtonClickAudioPlay();
		AllObjectDown();
		copyrightContent.SetActive(true);
	}

	private void CloseCopyrightButtonClick()
	{
		ButtonClickAudioPlay();
		GoMainMenu();
	}

	private void BackgroundMusicVolumeSliderChange(float value)
	{
		backgroundSource.volume = value;
		PlayerPrefs.SetFloat(Constants.backgroundSoundVolume, value);
	}

	private void ClockSoundVolumeSliderChange(float value)
	{
		clockAudioSource.volume = value;
		PlayerPrefs.SetFloat(Constants.clockSoundVolume, value);
	}

	private void SoundEffectVolumeSliderChange(float value)
	{
		audioSource.volume = value;
		PlayerPrefs.SetFloat(Constants.soundEffectVolume, value);
	}

	/// <summary>버튼 클릭 사운드를 재생한다.</summary>
	void ButtonClickAudioPlay()
	{
		audioSource.PlayOneShot(buttonClickAudio);
	}

	/// <summary>넘버 버튼 클릭 사운드를 재생한다.</summary>
	void NumberButtonClickAudioPlay()
	{
		audioSource.PlayOneShot(numberButtonClickAudio);
	}

	/// <summary>문제를 맞추었을 때 사운드를 재생한다.</summary>
	void CorrectAudioPlay()
	{
		audioSource.PlayOneShot(correctAudio, 0.5f);
	}

	/// <summary>문제를 틀렸을 때 사운드를 재생한다.</summary>
	void WrongAudioPlay()
	{
		audioSource.PlayOneShot(wrongAudio);
	}

	/// <summary>타이머 사운드를 재생한다.</summary>
	void ClockAudioPlay()
	{
		clockAudioSource.Play();
	}

	/// <summary>타이머 사운드를 멈춘다.</summary>
	void ClockAudioStop()
	{
		clockAudioSource.Stop();
	}

	/// <summary>배경 음악을 재생한다.</summary>
	void BackgroundAudioPlay()
	{
		backgroundSource.Play();
	}

	/// <summary>배경 음악을 멈춘다.</summary>
	void BackgroundAudioStop()
	{
		backgroundSource.Stop();
	}

	/// <summary>스테이지가 끝났을 때 사운드를 재생한다.</summary>
	void FinishAudioPlay()
	{
		audioSource.PlayOneShot(finishAudio);
	}

	/// <summary>스테이지를 클리어하지 못했을 떄 사운드를 재생한다.</summary>
	void FailAudioPlay()
	{
		audioSource.PlayOneShot(failAudio);
	}

	/// <summary>PlayerPrefs에 저장되어 있는 음량 값을 Key를 통해 가져온 후 세팅한다.</summary>
	void GetPlayerPrefsVolume(string volumeKey, AudioSource audioSourceName, Slider volumeSlider)
	{
		if (PlayerPrefs.HasKey(volumeKey))
		{
			float volume = PlayerPrefs.GetFloat(volumeKey);
			audioSourceName.volume = volume;
			volumeSlider.value = volume;
		}
		else
		{
			audioSourceName.volume = 0.3f;
			volumeSlider.value = 0.3f;
		}
	}

	/// <summary>PlayerPrefs에 저장되어 있는 플레이어의 레벨을 가져와서 세팅한다.</summary>
	void GetPlayerPrefsLevel()
	{
		string level = "";
		if (PlayerPrefs.HasKey(Constants.level))
		{
			level = PlayerPrefs.GetString(Constants.level);
			switch (level)
			{
				case Constants.level0:
					currentLevel = Constants.level0;
					break;
				case Constants.level1:
					currentLevel = Constants.level1;
					break;
				case Constants.level2:
					currentLevel = Constants.level2;
					break;
				case Constants.level3:
					currentLevel = Constants.level3;
					break;
				case Constants.level4:
					currentLevel = Constants.level4;
					break;
				case Constants.level5:
					currentLevel = Constants.level5;
					break;
				case Constants.level6:
					currentLevel = Constants.level6;
					break;
				default:
					currentLevel = Constants.level0;
					break;
			}
		}
		else
		{
			currentLevel = Constants.level0;
		}
	}

	/// <summary>PlayerPrefs에 저장되어 있는 전체 점수를 가져온다. 프로필 창의 Total에서 확인할 수 있다.</summary>
	void GetProfileTotalContent()
	{
		if (PlayerPrefs.HasKey(Constants.totalClearNum))
		{
			totalClearNum = PlayerPrefs.GetInt(Constants.totalClearNum);
		}
		else
		{
			totalClearNum = 0;
		}

		if (PlayerPrefs.HasKey(Constants.totalCorrectNum))
		{
			totalCorrectNum = PlayerPrefs.GetInt(Constants.totalCorrectNum);
		}
		else
		{
			totalCorrectNum = 0;
		}

		if (PlayerPrefs.HasKey(Constants.totalWrongNum))
		{
			totalWrongNum = PlayerPrefs.GetInt(Constants.totalWrongNum);
		}
		else
		{
			totalWrongNum = 0;
		}

		if ((totalCorrectNum + totalWrongNum) != 0)
		{
			totalCorrectAnswerRatio = (float)totalCorrectNum * 100 / (totalCorrectNum + totalWrongNum);
		}
		else
		{
			totalCorrectAnswerRatio = 0;
		}
	}

	/// <summary>PlayerPrefs에 전체 점수 값을 저장한다.</summary>
	void SetProfileTotalContent()
	{
		totalCorrectNum += stage - 1;
		totalWrongNum += wrongCount;
		totalCorrectAnswerRatio = (float)totalCorrectNum * 100 / (totalCorrectNum + totalWrongNum);
		PlayerPrefs.SetInt(Constants.totalClearNum, totalClearNum);
		PlayerPrefs.SetInt(Constants.totalCorrectNum, totalCorrectNum);
		PlayerPrefs.SetInt(Constants.totalWrongNum, totalWrongNum);
	}

	/// <summary>PlayerPrefs에 저장되어 있는 오늘 점수 값을 가져온다.</summary>
	void GetProfileTodayContent()
	{
		if (PlayerPrefs.HasKey(Constants.todayClearNum))
		{
			todayClearNum = PlayerPrefs.GetInt(Constants.todayClearNum);
		}
		else
		{
			todayClearNum = 0;
		}

		if (PlayerPrefs.HasKey(Constants.todayCorrectNum))
		{
			todayCorrectNum = PlayerPrefs.GetInt(Constants.todayCorrectNum);
		}
		else
		{
			todayCorrectNum = 0;
		}

		if (PlayerPrefs.HasKey(Constants.todayWrongNum))
		{
			todayWrongNum = PlayerPrefs.GetInt(Constants.todayWrongNum);
		}
		else
		{
			todayWrongNum = 0;
		}

		if ((todayCorrectNum + todayWrongNum) != 0)
		{
			todayCorrectAnswerRatio = (float)todayCorrectNum * 100 / (todayCorrectNum + todayWrongNum);
		}
		else
		{
			todayCorrectAnswerRatio = 0;
		}
	}

	/// <summary>PlayerPrefs에 오늘 점수 값을 저장한다.</summary>
	void SetProfileTodayContent()
	{
		todayCorrectNum += stage - 1;
		todayWrongNum += wrongCount;
		todayCorrectAnswerRatio = (float)todayCorrectNum * 100 / (todayCorrectNum + todayWrongNum);
		PlayerPrefs.SetInt(Constants.todayClearNum, todayClearNum);
		PlayerPrefs.SetInt(Constants.todayCorrectNum, todayCorrectNum);
		PlayerPrefs.SetInt(Constants.todayWrongNum, todayWrongNum);
	}

	/// <summary>Profile을 보여준다.</summary>
	public void ShowProfileContent(string profileType)
	{
		string contentText = "";

		switch (profileType)
		{
			case Constants.today:
				profileTodayButton.interactable = false;
				profileTotalButton.interactable = true;
				contentText = "Clear\n" + todayClearNum + "\n\nCorrect\n" + todayCorrectNum + "\n\nWrong\n" + todayWrongNum + "\n\nCorrect Answer Ratio\n" + todayCorrectAnswerRatio + "%";
				break;
			case Constants.total:
				profileTodayButton.interactable = true;
				profileTotalButton.interactable = false;
				contentText = "Clear\n" + totalClearNum + "\n\nCorrect\n" + totalCorrectNum + "\n\nWrong\n" + totalWrongNum + "\n\nCorrect Answer Ratio\n" + totalCorrectAnswerRatio + "%";
				break;
			default:
				Debug.Log("Profile type has error");
				break;
		}

		profileContentText.text = contentText;

		levelText.text = "Today you are a \n" + currentLevel;
	}

	private void GetPlayerPrefsTodayDateDay()
	{
		// 저장된 날짜가 있다면 가져온다.
		if (PlayerPrefs.HasKey(Constants.todayDateDay))
		{
			playerPrefsDay = PlayerPrefs.GetInt(Constants.todayDateDay);
		}
		else
		{
			playerPrefsDay = 0;
		}
	}

	/// <summary>저장된 기회가 있다면 그대로 가져오고, 아니라면 기회를 15만큼 주고 PlayerPrefs에 저장한다.</summary>
	private void GetChance()
	{
		if (PlayerPrefs.HasKey(Constants.chance))
		{
			chance = PlayerPrefs.GetInt(Constants.chance);
			// chance = 1;
			if (chance < 3)
			{
				chance = 3;
			}
		}
		else
		{
			chance = 15;
		}
		Debug.Log($"Chance : {chance}");
	}

	/// <summary>인자로 전달된 값을 기회로 저장한다.</summary>
	private void SetChance(int remainChance)
	{
		Debug.Log($"Chance : {chance}");
		PlayerPrefs.SetInt(Constants.chance, remainChance);
	}

	/// <summary>광고 버튼을 눌렀을 때 문제를 풀 수 있는 기회를 추가한다.</summary>
	private void ADButtonClick()
	{
		chance += 15;
		SetChance(chance);

		foregroundAD.GetComponent<ForegroundAD>().LoadInterstitialAd();
		BackgroundAudioStop();
		foregroundAD.GetComponent<ForegroundAD>().interstitialAd.OnAdFullScreenContentClosed += () =>
		{
			BackgroundAudioPlay();
		};
	}


}
