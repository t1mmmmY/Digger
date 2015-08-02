using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class GameManager : BaseSingleton<GameManager> 
{
	[SerializeField] float timeForOneTurn = 5.0f;
	[SerializeField] protected Camera camera;
	[SerializeField] Transform characterStartPosition;
	[SerializeField] Follower follower;
	[SerializeField] EndGamePanel endGamePanel;

	protected float forceShake = 0;
	protected int level = 0;
	protected int startCoinsCount = 0;

	public int GetLevel()
	{
		return level;
	}

//	public static System.Action OnStartGame;
	public static System.Action OnGameOver;
	public static System.Action OnWrongAnswer;
	public static System.Action<float, int> OnShake;


	protected virtual void Start()
	{
		LoadCharacter();
	}

	protected virtual void Update()
	{
	}

	protected virtual void OnEnable()
	{
		FormulaDrawer.OnAnswer += OnAnswer;
		FormulaDrawer.OnFinishClick += OnFinishClick;
		BankController.OnChangeCoins += OnChangeCoins;
	}

	protected virtual void OnDisable()
	{
		FormulaDrawer.OnAnswer -= OnAnswer;
		FormulaDrawer.OnFinishClick -= OnFinishClick;
		BankController.OnChangeCoins -= OnChangeCoins;
//		StopGame();
	}

	public virtual void StartGame()
	{
		startCoinsCount = BankController.coins;
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

	protected virtual void OnChangeCoins(int coinsCount)
	{

	}

	public virtual void GameOver()
	{
        Debug.LogWarning("Game over");
		bool isBestScore = level > GetBestScore();
		int coinsCount = BankController.coins - startCoinsCount;

		endGamePanel.ShowEndGamePanel(isBestScore, coinsCount, level);
	}

	public static int GetBestScore()
	{
		return PlayerPrefs.GetInt("BestLevel", 0);
	}


	public virtual void RestartGame()
	{
		LevelLoader.Instance.LoadLevel(Scene.SinglePlayer);
	}

	public virtual void ShareGame()
	{
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
            ShakeCamera(timeForOneTurn);
            StopCoroutine("GameLoop");
//			GameOver();
		}
	}

	private void LoadCharacter()
	{
		int characterNumber = 0;
		if (GeneralGameController.Instance != null)
		{
			characterNumber = GeneralGameController.Instance.characterNumber;
		}

		Object obj = Resources.Load(string.Format("{0}{1}", CONST.PLAYABLE_PLAYERS_PATH, CONST.PLAYER_NAMES[characterNumber]));
		if (obj != null)
		{
			GameObject characterGO = GameObject.Instantiate<GameObject>((GameObject)obj);
			characterGO.transform.parent = characterStartPosition;
			characterGO.transform.localPosition = Vector3.zero;

			follower.SetTarget(characterGO.transform);
		}
		else
		{
			Debug.LogError("Cannot load character!");
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
