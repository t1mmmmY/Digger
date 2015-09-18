using UnityEngine;
using System.Collections;

public class BonusCharacter : MonoBehaviour 
{
	[SerializeField] float _goldMultiplier = 2.0f;
	public float goldMultiplier
	{
		get { return _goldMultiplier; }
	}

//	void Awake()
//	{
//		if (SingleplayerGameManager.Instance != null)
//		{
//			SingleplayerGameManager.Instance.SetBonusCharacter(this);
//		}
//	}
}
