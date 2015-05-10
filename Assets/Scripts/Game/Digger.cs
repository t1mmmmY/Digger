using UnityEngine;
using System.Collections;

public class Digger : MonoBehaviour 
{
	public Transform player;
	[SerializeField] Animator playerAnimator;
	[SerializeField] AudioSource audioPlayer;
	[SerializeField] AudioClip[] emptyDigSound;
	[SerializeField] AudioClip[] mineralDigSound;

	public static System.Action onDig;
	int level = 0;

	void OnEnable()
	{
		level = 0;
		FormulaDrawer.OnAnswer += OnAnswer;
		GameManager.OnGameOver += OnGameOver;
	}

	void OnDisable()
	{
		FormulaDrawer.OnAnswer -= OnAnswer;
		GameManager.OnGameOver -= OnGameOver;
	}

	void OnAnswer(bool isRightAnswer)
	{
		if (isRightAnswer)
		{
			level++;
			Dig();
		}
		else
		{
			Hit();
		}
	}

	void Dig()
	{
		RaycastHit hit; 
		if (Physics.Raycast(player.transform.position, -Vector2.up, out hit, 100, 1 << 8))
		{
			SimpleTile tile = hit.transform.GetComponent<SimpleTile>();
			bool haveMineral = tile.DigMe();

			if (haveMineral)
			{
				audioPlayer.clip = GetRandomClip(mineralDigSound);
			}
			else
			{
				audioPlayer.clip = GetRandomClip(emptyDigSound);
			}

			audioPlayer.Play();

//			Destroy(hit.transform.gameObject);
		}

		if (onDig != null)
		{
			onDig();
		}
	}

	AudioClip GetRandomClip(AudioClip[] arrayOfClips)
	{
		return arrayOfClips[Random.Range(0, arrayOfClips.Length)];
	}

	void OnGameOver()
	{
		if (level > 0)
		{
			if (playerAnimator != null)
			{
				playerAnimator.SetTrigger("Die");
			}
			else
			{
				Debug.LogError("Animator not found");
			}
			

			Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
			playerRigidbody.isKinematic = true;
			Collider[] playerColliders = player.GetComponents<Collider>();
			foreach (Collider col in playerColliders)
			{
				col.enabled = false;
			}
		}
	}

	//On wrong answer
	void Hit()
	{
	}

}
