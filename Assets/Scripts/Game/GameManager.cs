using UnityEngine;
using System.Collections;

public abstract class GameManager : MonoBehaviour 
{
	[SerializeField] float timeForOneTurn = 5.0f;
	[SerializeField] protected Camera camera;

	protected float forceShake = 0;
	protected int level = 0;

//	public static System.Action OnStartGame;
	public static System.Action OnGameOver;
	public static System.Action OnWrongAnswer;
	public static System.Action<float, int> OnShake;

	protected virtual void Start()
	{
//		Invoke("StartGame", 0.5f);
	}

	protected virtual void Update()
	{
//		if (Input.GetKeyDown(KeyCode.Escape))
//		{
//			LevelLoader.Instance.LoadLevel(Scene.Lobby);
//		}
	}

	protected virtual void OnEnable()
	{
		FormulaDrawer.OnAnswer += OnAnswer;
		FormulaDrawer.OnFinishClick += OnFinishClick;
	}

	protected virtual void OnDisable()
	{
		FormulaDrawer.OnAnswer -= OnAnswer;
		FormulaDrawer.OnFinishClick -= OnFinishClick;
//		StopGame();
	}

	public virtual void StartGame()
	{
//		StartCoroutine("GameLoop");
	}

	public virtual void StopGame()
	{
		StopCoroutine("GameLoop");
		LevelLoader.Instance.LoadLevel(Scene.Lobby);
	}

	public virtual void WrongAnswer()
	{
		if (OnWrongAnswer != null)
		{
			OnWrongAnswer();
		}
	}

	protected virtual void OnFinishClick()
	{
	}

	public virtual void GameOver()
	{
//		int bestLevel = PlayerPrefs.GetInt("BestLevel", 0);
//		if (level > bestLevel)
//		{
//			PlayerPrefs.SetInt("BestLevel", level);
//			MultiplayerController.Instance.SetBestSore(level);
//		}
//
//		if (OnGameOver != null)
//		{
//			OnGameOver();
//		}

//		StartCoroutine("InvokeRestart", 1.0f);
	}

	public static int GetBestScore()
	{
		return PlayerPrefs.GetInt("BestLevel", 0);
	}


	public virtual void RestartGame()
	{
		LevelLoader.Instance.LoadLevel(Scene.SinglePlayer);
	}

	protected virtual void OnAnswer(bool isRight)
	{
		if (isRight)
		{
			level++;
			StopCoroutine("GameLoop");
			StartCoroutine("GameLoop");
		}
		else
		{
			WrongAnswer();
//			GameOver();
		}
	}

	IEnumerator InvokeRestart(float timeToRestart)
	{
		yield return new WaitForSeconds(timeToRestart);

		RestartGame();
	}

	IEnumerator GameLoop()
	{
		float elapsedTime = 0.0f;

		MoveCameraToStartPosition();

		do
		{
			yield return new WaitForSeconds(1.0f);
			elapsedTime += 1.0f;

//			Debug.Log(elapsedTime);
			if (elapsedTime >= timeForOneTurn - 2)
			{
				ShakeCamera(elapsedTime);
			}

		} while (elapsedTime < timeForOneTurn);

		yield return new WaitForSeconds(1.0f);

		WrongAnswer();
//		GameOver();
	}

	protected virtual void MoveCameraToStartPosition()
	{
		Hashtable hash = new Hashtable();
		hash.Add("position", Vector3.zero);
		hash.Add("isLocal", true);
		hash.Add("time", 1.0f);
		iTween.MoveTo(camera.gameObject, hash);
	}

	protected virtual void ShakeCamera(float time)
	{
		forceShake = (Mathf.Exp(time + 5 - timeForOneTurn) - 1) / 1000.0f;
		
		iTween.ShakePosition(camera.gameObject, new Vector3(forceShake, forceShake), 1.0f);

		if (OnShake != null)
		{
			OnShake(forceShake, level);
		}
	}

}
