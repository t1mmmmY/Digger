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
			// Create share sheet
			SocialShareSheet _shareSheet 	= new SocialShareSheet();	
			_shareSheet.Text				= "My result";
			
//			 Add below line if you want to share URL
//			_shareSheet.URL					= m_shareURL;
			
			// Add below line if you want to share a screenshot
			_shareSheet.AttachScreenShot();

			// Add below line if you want to share an image from a specified path.
//			_shareSheet.AttachImageAtPath(IMAGE_PATH);
			
			// Show composer
			NPBinding.UI.SetPopoverPointAtLastTouchPosition(); // To show popover at last touch point on iOS. On Android, its ignored.
			NPBinding.Sharing.ShowView(_shareSheet, OnFinishSharing);

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
