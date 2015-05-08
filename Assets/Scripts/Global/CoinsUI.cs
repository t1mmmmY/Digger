using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinsUI : MonoBehaviour 
{
	[SerializeField] Text coinsLabel;
	[SerializeField] Animator coinsAnimator;
	[SerializeField] Image goldCoinPrefab;
	[SerializeField] Rect goldCoinsStartPos;
	[SerializeField] Vector2 goldCoinSize;
	[SerializeField] Vector2 goldCoinSpeed;
	[SerializeField] Transform goldFollowerPrefab;
	[SerializeField] Transform goldFollowerStartPoint;

	private int showCoinsHash = Animator.StringToHash("ShowLabel");
	private int hideCoinsHash = Animator.StringToHash("HideLabel");

	private int playerCoins = 0;
	private float delay = 1.5f;
	private float coinsAddAnimationTime = 1.0f;

	private int followerCounter = 0;

	void OnEnable()
	{
		BankController.OnChangeCoins += OnChangeCoins;
		GeneralGameController.onLoadLobby += OnLoadLobby;
	}

	void OnDisable()
	{
		BankController.OnChangeCoins -= OnChangeCoins;
		GeneralGameController.onLoadLobby -= OnLoadLobby;
	}

	void OnChangeCoins(int coins)
	{
		int oldPlayerCoins = playerCoins;
		playerCoins = coins;
		StartCoroutine(AddCoins(coins - oldPlayerCoins, oldPlayerCoins));
	}

	void OnLoadLobby()
	{
		coinsAnimator.SetTrigger(showCoinsHash);
	}

	IEnumerator AddCoins(int coins, int oldPlayerCoins)
	{
		if (CONST.InGame())
		{
			CreateAndAnimateGoldCoins(coins);
			yield return new WaitForSeconds(delay);
		}

		float coinsAddDelay = coinsAddAnimationTime / coins;
		float elapsedTime = 0;

		do
		{
			yield return null;
			elapsedTime += Time.deltaTime;

			if (elapsedTime > coinsAddAnimationTime)
			{
				elapsedTime = 1;
			}

			coinsLabel.text = (Mathf.RoundToInt(oldPlayerCoins + coins * elapsedTime)).ToString();

		} while (elapsedTime < coinsAddAnimationTime);

//		playerCoins += coins;

		coinsLabel.text = playerCoins.ToString();

	}


	void CreateAndAnimateGoldCoins(int count)
	{
		GameObject follower = (GameObject)GameObject.Instantiate(goldFollowerPrefab.gameObject);
		follower.transform.SetParent(goldFollowerStartPoint.transform);
		follower.transform.localPosition = Vector3.zero;


		for (int i = 0; i < count; i++)
		{
			GameObject go = (GameObject)GameObject.Instantiate(goldCoinPrefab.gameObject);
			go.transform.SetParent(this.transform);

			Image coinImage = go.GetComponent<Image>();
			coinImage.rectTransform.localPosition = new Vector3(Random.Range(goldCoinsStartPos.xMin, goldCoinsStartPos.xMax),
			                                                    Random.Range(goldCoinsStartPos.yMin, goldCoinsStartPos.yMax),
			                                                    0);

			float size = Random.Range(goldCoinSize.x, goldCoinSize.y);
			coinImage.rectTransform.sizeDelta = new Vector2(size, size);
			coinImage.rectTransform.localScale = Vector3.one;

			coinImage.rectTransform.Rotate(0, 0, Random.Range(0, 360));

			go.GetComponent<Follower>().target = follower.transform;
			go.GetComponent<Follower>().speed = Random.Range(goldCoinSpeed.x, goldCoinSpeed.y);

		}


	}

}
