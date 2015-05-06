using UnityEngine;
using System.Collections;

public class FallenRocks : MonoBehaviour 
{
	[SerializeField] ParticleSystem[] rockParticels;
	[SerializeField] float defaultSize = 0.3f;
	int playerLevel = 0;

	void OnEnable()
	{
		DisabeParticles();
		GameManager.OnShake += OnShake;
		GameManager.OnGameOver += OnGameOver;
	}

	void OnDisable()
	{
		GameManager.OnShake -= OnShake;
		GameManager.OnGameOver -= OnGameOver;
	}

	void OnShake(float force, int level)
	{
		playerLevel = level;
		//multiply by 10 for normal size
//		Debug.Log(force);
		if (level > 3)
		{
			Vector3 newPos = transform.position;
			newPos.y = -level + 4;
			transform.position = newPos;
		}
		else
		{
			Vector3 newPos = transform.position;
			newPos.y = 0;
			transform.position = newPos;
		}

		BurstParticles(force * 8.0f);
	}

	void OnGameOver()
	{
		if (playerLevel > 0)
		{
			BurstParticles(1.5f);
		}
	}

//	void Update()
//	{
//		if (Input.GetKeyDown(KeyCode.Space))
//		{
//			BurstParticles();
//		}
//	}

	void DisabeParticles()
	{
		foreach(ParticleSystem particle in rockParticels)
		{
			particle.Stop();
		}
	}

	void BurstParticles(float size)
	{
		foreach(ParticleSystem particle in rockParticels)
		{
			particle.startSize = size;
			particle.Play();
		}
	}

}
