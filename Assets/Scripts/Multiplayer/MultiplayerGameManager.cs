using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Messages;

public class MultiplayerGameManager : GameManager 
{
	public int maximumScore = 240;
	public int gameSessionTime = 60;

	public iTweenEvent winEvent;
	public iTweenEvent looseEvent;
	public iTweenEvent pairEvent;
	
	int opponentLevel = 0;
	AllFormulas allFormulas = null;
	bool gameStarted = false;

	public static System.Action<OneTurn> OnOpponentTurn;
	public static System.Action<int> OnTick;
	public static System.Action<AllFormulas> OnStartMultiplayerGame;


	protected override void Start()
	{
//		Debug.LogWarning("Start");
		//Invoke("StartGame", 0.5f);
		base.Start();
	}

	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		object obj;

		obj	= MultiplayerController.Deserialize(data, typeof(AllFormulas));
		try
		{
			AllFormulas tempFormulas = (AllFormulas)obj;
//			foreach (Formula formula in tempFormulas.formulas)
//			{
////				Debug.Log(formula.ToString());
//			}
			
//			Debug.LogWarning("Receive formulas");

			allFormulas = new AllFormulas(new List<Formula>());
			allFormulas.Copy(tempFormulas);
			StartGame();
			return;
		}
		catch (System.Exception ex)
		{
			Debug.Log("Not an AllFormulas object");
		}

		obj	= MultiplayerController.Deserialize(data, typeof(OneTurn));
		try
		{
			OneTurn opponentTurn = (OneTurn)obj;
			if (OnOpponentTurn != null)
			{
				OnOpponentTurn(opponentTurn);
			}
			opponentLevel = opponentTurn.turnNumber;
			return;
		}
		catch (System.Exception ex2)
		{
			Debug.Log("Not an OneTurn object");
		}

	}

	AllFormulas GenerateStartFormulas()
	{
		List<Formula> formulas = new List<Formula>();

		for (int i = 0; i < maximumScore; i++)
		{
			formulas.Add(Mathematician.GetFormula(i));
		}
		return new AllFormulas(formulas);
	}

	protected override void Update ()
	{
		base.Update();
	}

	protected override void OnEnable ()
	{
		MultiplayerController.onRealTimeMessageReceived += OnRealTimeMessageReceived;

		if (MultiplayerController.Instance.IsFirstPlayer())
		{
//			Debug.LogWarning("Generate formulas");
			allFormulas = GenerateStartFormulas();
			foreach (Formula formula in allFormulas.formulas)
			{
//				Debug.Log(formula.ToString());
			}

			MultiplayerController.Instance.SendRealTimeMessage(allFormulas);
			Invoke("StartGame", 0.5f);
		}

		base.OnEnable();
	}

	protected override void OnDisable ()
	{
		MultiplayerController.onRealTimeMessageReceived -= OnRealTimeMessageReceived;
		MultiplayerController.Instance.Disconnect();
		base.OnDisable();
	}

	public override void StartGame ()
	{
//		Debug.LogWarning("Start Game");
		if (gameStarted)
		{
//			Debug.LogWarning("Game already started");
			return;
		}


		gameStarted = true;

//#if UNITY_EDITOR
//		LevelLoader.Instance.LoadLevel(2);
//#endif

		if (OnStartMultiplayerGame != null)
		{
			OnStartMultiplayerGame(allFormulas);
		}

		StartTimer();

		base.StartGame();
	}

	public override void StopGame ()
	{
		base.StopGame();
	}

	public override void WrongAnswer()
	{
		base.WrongAnswer();
	}

	protected override void OnFinishClick()
	{
		LevelLoader.Instance.LoadLevel(Scene.Lobby);
		base.OnFinishClick();
	}

	public override void GameOver ()
	{
		if (OnGameOver != null)
		{
			OnGameOver();
		}

		//Set player score
		if (level > opponentLevel) //Win
		{
			MultiplayerController.Instance.ChangeRang(1);
			if (winEvent != null)
			{
				winEvent.gameObject.SetActive(true);
				winEvent.Play();
			}
		}
		else if (level < opponentLevel) //Loose
		{
			MultiplayerController.Instance.ChangeRang(-1);
			if (looseEvent != null)
			{
				looseEvent.gameObject.SetActive(true);
				looseEvent.Play();
			}
		}
		else //Pair
		{
			MultiplayerController.Instance.ChangeRang(0);
			if (pairEvent != null)
			{
				pairEvent.gameObject.SetActive(true);
				pairEvent.Play();
			}
		}

//		StartCoroutine("InvokeToLobby", 2.0f);
		
		base.GameOver ();
	}

	IEnumerator InvokeToLobby(float time)
	{
		yield return new WaitForSeconds(time);
		
		LevelLoader.Instance.LoadLevel(Scene.Lobby);
	}

	public override void RestartGame ()
	{
		gameStarted = false;
		base.RestartGame();
	}

	protected override void OnAnswer (bool isRight)
	{
		MoveCameraToStartPosition();

		int currentLevel = isRight ? level + 1 : level;
		OneTurn oneTurn = new OneTurn(isRight, currentLevel);
		MultiplayerController.Instance.SendRealTimeMessage(oneTurn, true);
		base.OnAnswer(isRight);
	}

	protected override void MoveCameraToStartPosition ()
	{
		base.MoveCameraToStartPosition();
	}

	protected override void ShakeCamera (float time)
	{
		forceShake = (Mathf.Exp(time + 5 - gameSessionTime) - 1) / 1000.0f;
		
		iTween.ShakePosition(camera.gameObject, new Vector3(forceShake, forceShake), 1.0f);

		if (OnShake != null)
		{
			OnShake(forceShake, level);
		}
	}

	void StartTimer()
	{
		Debug.LogWarning("StartTimer");
		StartCoroutine("MultiplayerGameLoop", gameSessionTime);
	}

	void StopTimer()
	{
	}

	IEnumerator MultiplayerGameLoop(float allTime)
	{
		float elapsedTime = 0;

		if (OnTick != null)
		{
			OnTick((int)(allTime-elapsedTime));
		}
		
		MoveCameraToStartPosition();

		do
		{
			yield return new WaitForSeconds(1.0f);
			elapsedTime += 1.0f;

			if (OnTick != null)
			{
				OnTick((int)(allTime-elapsedTime));
			}
//			MoveCameraToStartPosition();
			
			//			Debug.Log(elapsedTime);
			if (elapsedTime >= allTime - 5)
			{
				ShakeCamera(elapsedTime);
			}
			
		} while (elapsedTime < allTime);
		
		yield return new WaitForSeconds(1.0f);

		GameOver();
	}
}
