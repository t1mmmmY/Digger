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

	public Color colorRightAnswer = Color.green;
	public Color colorWrongAnswer = Color.red;

	int level = 0;
	Formula formula;
	int rightAnswerNumber = 0;
	bool gameOver = true;

	public static System.Action<bool> OnAnswer;

	public void Answer(int buttonNumber)
	{
		if (gameOver)
		{
			return;
		}

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
		Digger.onDig += OnDig;
		SingleplayerGameManager.OnStartSinglePlayerGame += OnStartSinglePlayerGame;
		MultiplayerGameManager.OnStartMultiplayerGame += OnStartMultiplayerGame;
//		GameManager.OnStartGame += OnStartGame;
		GameManager.OnGameOver += OnGameOver;

		ClearTexts();
	}

	void OnDisable()
	{
		Digger.onDig -= OnDig;
		SingleplayerGameManager.OnStartSinglePlayerGame -= OnStartSinglePlayerGame;
		MultiplayerGameManager.OnStartMultiplayerGame -= OnStartMultiplayerGame;
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
		bestLevelText.enabled = true;
		opponentScore.enabled = false;

		gameOver = false;
		level = 0;
		levelText.text = level.ToString();
		bestLevelText.text = GameManager.GetBestScore().ToString();
		GenerateQuestion();
	}

	void OnStartMultiplayerGame()
	{
		bestLevelText.enabled = false;
		opponentScore.enabled = true;

		gameOver = false;
		level = 0;
		levelText.text = level.ToString();
		bestLevelText.text = GameManager.GetBestScore().ToString();
		GenerateQuestion();

		MultiplayerGameManager.OnOpponentTurn += OnOpponentTurn;
	}

	void OnOpponentTurn(Messages.OneTurn oneTurn)
	{
		opponentScore.text = oneTurn.turnNumber.ToString();
	}

	void OnGameOver()
	{
		gameOver = true;
		MultiplayerGameManager.OnOpponentTurn -= OnOpponentTurn;
	}


	void OnDig()
	{
		level++;
		levelText.text = level.ToString();
		GenerateQuestion();
	}

	Formula GetFormula(int level)
	{
		return Mathematician.GetFormula(level);
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
