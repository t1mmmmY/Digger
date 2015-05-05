using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FormulaDrawer : MonoBehaviour 
{
	public Text textFormula;
	public Button[] buttons;
	public Text[] textVariants;
	public Text levelText;
	public Text bestLevelText;
	public Text opponentScore;
	public Text timer;

	public Color colorRightAnswer = Color.green;
	public Color colorWrongAnswer = Color.red;
	public int disableTime = 5;

	int level = 0;
	Formula formula;
	int rightAnswerNumber = -1;
	bool gameOver = true;
	Messages.AllFormulas allFormulas;
	bool isMultiplayer = false;
	int lastAnswer = -1;
	Color oldColor;

	public static System.Action<bool> OnAnswer;
	public static System.Action OnFinishClick;

	public void Answer(int buttonNumber)
	{
		if (gameOver)
		{
			return;
		}

		lastAnswer = buttonNumber;

		if (buttonNumber == rightAnswerNumber)
		{
			if (OnAnswer != null)
			{
				OnAnswer(true);
			}
		}
		else
		{

			if (OnAnswer != null)
			{
				OnAnswer(false);
			}
		}
	}

	void OnEnable()
	{
		
		oldColor = buttons[0].colors.disabledColor;

//		allFormulas = null;
		Digger.onDig += OnDig;
		SingleplayerGameManager.OnStartSinglePlayerGame += OnStartSinglePlayerGame;
		MultiplayerGameManager.OnStartMultiplayerGame += OnStartMultiplayerGame;
//		GameManager.OnStartGame += OnStartGame;
		GameManager.OnWrongAnswer += OnWrongAnswer;
		GameManager.OnGameOver += OnGameOver;

		ClearTexts();
		DisableButtons();
	}

	void OnDisable()
	{
		Digger.onDig -= OnDig;
		SingleplayerGameManager.OnStartSinglePlayerGame -= OnStartSinglePlayerGame;
		MultiplayerGameManager.OnStartMultiplayerGame -= OnStartMultiplayerGame;
		GameManager.OnWrongAnswer -= OnWrongAnswer;
		GameManager.OnGameOver -= OnGameOver;
	}

	void ClearTexts()
	{
		textFormula.text = "";
		foreach (Text t in textVariants)
		{
			t.text = "";
		}
		levelText.text = "";
		bestLevelText.text = "";
	}

	void Update()
	{
		if (gameOver)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (OnFinishClick != null)
				{
					OnFinishClick();
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			buttons[0].OnPointerClick(pointerEventData);
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			buttons[1].OnPointerClick(pointerEventData);
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			buttons[2].OnPointerClick(pointerEventData);
		}
	}

	void OnStartSinglePlayerGame()
	{
		allFormulas = null;
		bestLevelText.enabled = true;
		if (opponentScore != null)
		{
			opponentScore.enabled = false;
		}
		isMultiplayer = false;

		gameOver = false;
		rightAnswerNumber = -1;
		lastAnswer = -1;
		level = 0;
		levelText.text = level.ToString();
		bestLevelText.text = GameManager.GetBestScore().ToString();
		GenerateQuestion();

		EnableButtons();
	}

	void OnStartMultiplayerGame(Messages.AllFormulas allGeneratedFormulas)
	{
		Debug.Log("OnStartMultiplayerGame");
		allFormulas = allGeneratedFormulas;

		bestLevelText.enabled = false;
		if (opponentScore != null)
		{
			opponentScore.enabled = true;
		}
		isMultiplayer = true;

		gameOver = false;
		rightAnswerNumber = -1;
		lastAnswer = -1;
		level = 0;
		levelText.text = level.ToString();
		bestLevelText.text = GameManager.GetBestScore().ToString();
		GenerateQuestion();

		EnableButtons();

		MultiplayerGameManager.OnOpponentTurn += OnOpponentTurn;
		MultiplayerGameManager.OnTick += OnTick;
	}

	void OnOpponentTurn(Messages.OneTurn oneTurn)
	{
		if (opponentScore != null)
		{
			opponentScore.text = oneTurn.turnNumber.ToString();
		}
	}

	void OnWrongAnswer()
	{
		if (isMultiplayer)
		{
			StartCoroutine("FreezeButtons", disableTime);
		}
		else
		{
			DisableButtons();
		}
	}

	void DisableButtons()
	{
		ColorBlock colorBlock;
		if (lastAnswer != -1)
		{
			colorBlock = buttons[lastAnswer].colors;
			colorBlock.disabledColor = colorWrongAnswer;
			buttons[lastAnswer].colors = colorBlock;
		}
		if (!isMultiplayer && rightAnswerNumber != -1)
		{
			colorBlock = buttons[rightAnswerNumber].colors;
			colorBlock.disabledColor = colorRightAnswer;
			buttons[rightAnswerNumber].colors = colorBlock;
		}
		foreach (Button button in buttons)
		{
			button.interactable = false;
		}
	}

	void EnableButtons()
	{ 
		ColorBlock colorBlock;
		if (lastAnswer != -1)
		{
			colorBlock = buttons[lastAnswer].colors;
			colorBlock.disabledColor = oldColor;
			buttons[lastAnswer].colors = colorBlock;
		}
		if (!isMultiplayer && rightAnswerNumber != -1)
		{
			colorBlock = buttons[rightAnswerNumber].colors;
			colorBlock.disabledColor = oldColor;
			buttons[rightAnswerNumber].colors = colorBlock;
		}

		foreach (Button button in buttons)
		{
			button.interactable = true;
		}
	}

	IEnumerator FreezeButtons(float time)
	{
		DisableButtons();

		yield return new WaitForSeconds(time);

		EnableButtons();
	}

	void OnGameOver()
	{
		gameOver = true;
		MultiplayerGameManager.OnOpponentTurn -= OnOpponentTurn;
		MultiplayerGameManager.OnTick -= OnTick;

		DisableButtons();
	}


	void OnDig()
	{
//		Debug.Log("OnDig");
		level++;
		levelText.text = level.ToString();
		GenerateQuestion();
	}

	void OnTick(int tick)
	{
		if (timer != null)
		{
			timer.text = tick.ToString();
		}
	}

	Formula GetFormula(int level)
	{
		lastAnswer = -1;
		if (isMultiplayer && allFormulas != null/* && allFormulas.formulas.Count > level + 1*/)
		{
			Debug.Log("Get Formula");
			return allFormulas.formulas[level];
		}
		else
		{
			Debug.Log("Generate Formula");
			return Mathematician.GetFormula(level);
		}
	}

	void GenerateQuestion()
	{
		formula = GetFormula(level);
		int sector = Mathematician.GetSector(Mathematician.GetDifficult(level));

		int randValue = Random.Range(0, sector);

		string rightAnswerText = "";
		int rightAnswer = 0;

		switch (randValue)
		{
		case 0: //0 - result
			rightAnswerText = formula.result.ToString();
			rightAnswer = formula.result;
			textFormula.text = string.Format("{0} {1} {2} = ?", formula.x, formula.GetSignString(), formula.y);
			break;
		case 1: //1 - y
			rightAnswerText = formula.y.ToString();
			rightAnswer = formula.y;
			textFormula.text = string.Format("{0} {1} ? = {2}", formula.x, formula.GetSignString(), formula.result);
			break;
		case 2: //2 - x
			rightAnswerText = formula.x.ToString();
			rightAnswer = formula.x;
			textFormula.text = string.Format("? {0} {1} = {2}", formula.GetSignString(), formula.y, formula.result);
			break;
		case 3: //3 - sign
			rightAnswerText = formula.GetSignString();
			textFormula.text = string.Format("{0} ? {1} = {2}", formula.x, formula.y, formula.result);
			break;
		default: //else - result
			rightAnswerText = formula.result.ToString();
			rightAnswer = formula.result;
			textFormula.text = string.Format("{0} {1} {2} = ?", formula.x, formula.GetSignString(), formula.y);
			break;
		}

		//Set right button
		rightAnswerNumber = Random.Range(0, textVariants.Length);
		List<string> answers = new List<string>();

		//Heighlight right answer
//		SetButtonsColor(rightAnswerNumber);


		for (int i = 0; i < textVariants.Length; i++)
		{
			if (i == rightAnswerNumber)
			{
				textVariants[rightAnswerNumber].text = rightAnswerText;
				answers.Add(textVariants[rightAnswerNumber].text);
			}
			else
			{
				if (randValue != 3) //Not a sign
				{
					do
					{
						textVariants[i].text = Random.Range(rightAnswer - 5, rightAnswer + 5).ToString();
//						if (rightAnswer >= 4)
//						{
//							textVariants[i].text = Random.Range(-(rightAnswer / 2), (rightAnswer / 2)).ToString();
//						}
//						else
//						{
//							textVariants[i].text = Random.Range(-4, 4).ToString();
//						}
					} while (textVariants[i].text == rightAnswerText || answers.Contains(textVariants[i].text));

					answers.Add(textVariants[i].text);
				}
				else
				{
					do
					{
						textVariants[i].text = Mathematician.GetSignString(Mathematician.signs[Random.Range(0, Mathematician.signs.Length)]);
					} while (textVariants[i].text == rightAnswerText || answers.Contains(textVariants[i].text));
					
					answers.Add(textVariants[i].text);
				}
			}
		}


	}

	void SetButtonsColor(int rightAnswer)
	{
		for (int i = 0; i < buttons.Length; i++)
		{
			ColorBlock colorBlock = buttons[i].colors;
			colorBlock.pressedColor = i == rightAnswer ? colorRightAnswer : colorWrongAnswer;
			buttons[i].colors = colorBlock;
		}
	}

}
