using UnityEngine;
using System.Collections;

public class SingleplayerGameManager : GameManager 
{

	public static System.Action OnStartSinglePlayerGame;

	protected override void Start()
	{
		Invoke("StartGame", 0.5f);
		base.Start();
	}

	protected override void Update ()
	{
		base.Update ();
	}
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
	}
	
	protected override void OnDisable ()
	{
		base.OnDisable ();
	}
	
	public override void StartGame ()
	{
		if (OnStartSinglePlayerGame != null)
		{
			OnStartSinglePlayerGame();
		}
		base.StartGame ();
	}
	
	public override void StopGame ()
	{
		base.StopGame ();
	}

	public override void WrongAnswer()
	{
		GameOver();

		base.WrongAnswer();
	}

	protected override void OnFinishClick()
	{
		RestartGame();

		base.OnFinishClick();
	}
	
	public override void GameOver ()
	{
		int bestLevel = PlayerPrefs.GetInt("BestLevel", 0);
		if (level > bestLevel)
		{
			PlayerPrefs.SetInt("BestLevel", level);
			MultiplayerController.Instance.SetBestSore(level);
		}
		
		if (OnGameOver != null)
		{
			OnGameOver();
		}

//		StartCoroutine("InvokeRestart", 1.0f);

		base.GameOver ();
	}
	
	public override void RestartGame ()
	{
		base.RestartGame ();
	}
	
	protected override void OnAnswer (bool isRight)
	{
		base.OnAnswer (isRight);
	}
	
	protected override void MoveCameraToStartPosition ()
	{
		base.MoveCameraToStartPosition ();
	}
	
	protected override void ShakeCamera (float time)
	{
		base.ShakeCamera (time);
	}
}
