using UnityEngine;
using System.Collections;
using VoxelBusters.NativePlugins;
using System.IO;

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
#if UNITY_ANDROID
			StartCoroutine("ShareScreenshot");

#elif UNITY_IOS
			canShare = false;
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

#endif
			base.ShareGame();
		}
	}

	public IEnumerator ShareScreenshot()
	{
		canShare = false;

		// wait for graphics to render
		yield return new WaitForEndOfFrame();
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO
		// create the texture
		Texture2D screenTexture = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);

		// put buffer into texture
		screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);

		// apply
		screenTexture.Apply();
		//----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO

		byte[] dataToSave = screenTexture.EncodeToPNG();

		string destination = Path.Combine(Application.persistentDataPath,System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");

		File.WriteAllBytes(destination, dataToSave);

		if(!Application.isEditor)
		{
			// block to open the file and share it ------------START
			AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
			AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
			intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
			AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
			AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://" + destination);
			intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
			//intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "testo");
			//intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");
			intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
			AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

			// option one WITHOUT chooser:
			currentActivity.Call("startActivity", intentObject);

			// option two WITH chooser:
			//AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "YO BRO! WANNA SHARE?");
			//currentActivity.Call("startActivity", jChooser);

			// block to open the file and share it ------------END

		}
		canShare = true;
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
