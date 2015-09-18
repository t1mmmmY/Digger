using UnityEngine;
using System.Collections;
using VoxelBusters.NativePlugins;

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

//	public override void SetBonusCharacter(BonusCharacter bonusCharacter)
//	{
//		base.SetBonusCharacter(bonusCharacter);
//	}
//
//	public override BonusCharacter GetBonusCharacter()
//	{
//		return bonusCharacter;
//	}
	
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
		if (canShare)
		{
			// Set popover to last touch position
			NPBinding.UI.SetPopoverPointAtLastTouchPosition();

//			canShare = false;
			eShareOptions[] options = new eShareOptions[] { eShareOptions.FB,
															eShareOptions.MAIL,
															eShareOptions.MESSAGE,
															eShareOptions.TWITTER,
															eShareOptions.UNDEFINED,
															eShareOptions.WHATSAPP 
														};
			NPBinding.Sharing.ShareScreenShot("My result", options, OnFinishSharing);
//			NPBinding.Sharing.ShareScreenShotOnSocialNetwork("My result", null);

			base.ShareGame();
		}
	}

	void OnFinishSharing(eShareResult _result)
	{
		Debug.Log("OnFinishSharing" + _result.ToString());
		canShare = true;
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
