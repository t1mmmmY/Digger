using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	[SerializeField] float timeForOneTurn = 5.0f;
	[SerializeField] Camera camera;

	float forceShake = 0;
	int level = 0;

	public static System.Action OnStartGame;
	public static System.Action OnGameOver;

	void Start()
	{
		Invoke("StartGame", 0.5f);
		MultiplayerController.Instance.TrySilentSignIn();
//		StartGame();
	}

	void OnEnable()
	{
		FormulaDrawer.OnAnswer += OnAnswer;
	}

	void OnDisable()
	{
		FormulaDrawer.OnAnswer -= OnAnswer;
		StopGame();
	}

	public void StartGame()
	{
		if (OnStartGame != null)
		{
			OnStartGame();
		}

		StartCoroutine("GameLoop");
	}

	public void StopGame()
	{
		StopCoroutine("GameLoop");
	}

	public void GameOver()
	{
		int bestLevel = PlayerPrefs.GetInt("BestLevel", 0);
		if (level > bestLevel)
		{
			PlayerPrefs.SetInt("BestLevel", level);
		}

		if (OnGameOver != null)
		{
			OnGameOver();
		}

		StartCoroutine("InvokeRestart", 1.0f);
	}

	public static int GetBestScore()
	{
		return PlayerPrefs.GetInt("BestLevel", 0);
	}

	public void RestartGame()
	{
		Application.LoadLevel(0);
	}

	void OnAnswer(bool isRight)
	{
		if (isRight)
		{
			level++;
			StopCoroutine("GameLoop");
			StartCoroutine("GameLoop");
		}
		else
		{
			GameOver();
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

		GameOver();
	}

	void MoveCameraToStartPosition()
	{
		Hashtable hash = new Hashtable();
		hash.Add("position", Vector3.zero);
		hash.Add("isLocal", true);
		hash.Add("time", 1.0f);
		iTween.MoveTo(camera.gameObject, hash);
	}

	void ShakeCamera(float time)
	{
		forceShake = (Mathf.Exp(time + 5 - timeForOneTurn) - 1) / 1000.0f;
		
		iTween.ShakePosition(camera.gameObject, new Vector3(forceShake, forceShake), 1.0f);
	}

}
