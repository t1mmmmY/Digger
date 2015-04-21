using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
	[SerializeField] float timeForOneTurn = 5.0f;
	[SerializeField] Camera camera;

	float forceShake = 0;

	public static System.Action OnStartGame;
	public static System.Action OnGameOver;

	void Start()
	{
		StartGame();
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
		if (OnGameOver != null)
		{
			OnGameOver();
		}

		StartCoroutine("InvokeRestart", 1.0f);
	}

	public void RestartGame()
	{
		Application.LoadLevel(0);
	}

	void OnAnswer(bool isRight)
	{
		if (isRight)
		{
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

		GameOver();
	}

	void ShakeCamera(float time)
	{
		forceShake = (Mathf.Exp(time + 5 - timeForOneTurn) - 1) / 1000.0f;
		
		iTween.ShakePosition(camera.gameObject, new Vector3(forceShake, forceShake), 1.0f);
	}

	void ShakeCamera()
	{
	}

}
