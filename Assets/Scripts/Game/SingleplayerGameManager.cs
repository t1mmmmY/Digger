using UnityEngine;
using System.Collections;

public class SingleplayerGameManager : GameManager 
{
	bool canShare = true;

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
		base.GameOver();

		if (OnGameOver != null)
		{
			OnGameOver();
		}

		int bestLevel = PlayerPrefs.GetInt("BestLevel", 0);
		if (level > bestLevel)
		{
			PlayerPrefs.SetInt("BestLevel", level);
			MultiplayerController.Instance.SetBestSore(level);
		}

//		StartCoroutine("InvokeRestart", 1.0f);

	}
	
	public override void RestartGame ()
	{
		base.RestartGame ();
	}

	public override void ShareGame()
	{
//		if (canShare)
		{
			canShare = false;
			NPBinding.Sharing.ShareScreenShotOnSocialNetwork("My result", null);
	//		VoxelBusters.NativePlugins.Sharing sharing = new VoxelBusters.NativePlugins.Sharing();
	//		sharing.ShareScreenShotOnSocialNetwork();

			base.ShareGame();
		}
	}

//	void OnFinishSharing(VoxelBusters.NativePlugins.Sharing.SharingCompletion _result)
//	{
//		canShare = true;
//	}
	
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
