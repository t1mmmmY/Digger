using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Messages;

public class MultiplayerGameManager : GameManager 
{
	public int maximumScore = 240;

	AllFormulas allFormulas;

	public static System.Action<OneTurn> OnOpponentTurn;
	public static System.Action OnStartMultiplayerGame;


	protected override void Start()
	{
		if (allFormulas == null)
		{
			allFormulas = GenerateStartFormulas();
			MultiplayerController.Instance.SendRealTimeMessage(allFormulas);
		}
		//Invoke("StartGame", 0.5f);
		base.Start();
	}

	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		object obj;

		//If initial message - AllFormulas
		obj	= MultiplayerController.Deserialize(data, typeof(AllFormulas));
		if (obj != null)
		{
			allFormulas = (AllFormulas)obj;
			StartGame();
//			foreach (Formula formula in allFormulas.formulas)
//			{
//				Debug.Log(formula.ToString());
//			}
		}
		else
		{
			//If OneTurn message
			obj = MultiplayerController.Deserialize(data, typeof(OneTurn));
			if (obj != null)
			{
				OneTurn opponentTurn = (OneTurn)obj;
				if (OnOpponentTurn != null)
				{
					OnOpponentTurn(opponentTurn);
				}
			}
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
		base.OnEnable();
	}

	protected override void OnDisable ()
	{
		MultiplayerController.onRealTimeMessageReceived -= OnRealTimeMessageReceived;
		base.OnDisable();
	}

	public override void StartGame ()
	{
		if (OnStartMultiplayerGame != null)
		{
			OnStartMultiplayerGame();
		}
		base.StartGame();
	}

	public override void StopGame ()
	{
		base.StopGame();
	}

	public override void GameOver ()
	{
		if (OnGameOver != null)
		{
			OnGameOver();
		}

		StartCoroutine("InvokeToLobby", 1.0f);
		base.GameOver();
	}

	IEnumerator InvokeToLobby(float time)
	{
		yield return new WaitForSeconds(time);
		
		LevelLoader.Instance.LoadLevel(0);
	}

	public override void RestartGame ()
	{
		base.RestartGame();
	}

	protected override void OnAnswer (bool isRight)
	{
		OneTurn oneTurn = new OneTurn(isRight, level);
		MultiplayerController.Instance.SendRealTimeMessage(oneTurn, true);
		base.OnAnswer(isRight);
	}

	protected override void MoveCameraToStartPosition ()
	{
		base.MoveCameraToStartPosition();
	}

	protected override void ShakeCamera (float time)
	{
		base.ShakeCamera(time);
	}
}
