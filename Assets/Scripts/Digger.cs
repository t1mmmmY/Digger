using UnityEngine;
using System.Collections;

public class Digger : MonoBehaviour 
{
	public Transform player;

	public static System.Action onDig;


	void OnEnable()
	{
		FormulaDrawer.OnAnswer += OnAnswer;
	}

	void OnDisable()
	{
		FormulaDrawer.OnAnswer -= OnAnswer;
	}

	void OnAnswer(bool isRightAnswer)
	{
		if (isRightAnswer)
		{
			Dig();
		}
		else
		{
			Hit();
		}
	}

	void Dig()
	{
		RaycastHit2D hit = Physics2D.Raycast(player.transform.position, -Vector2.up, 100, 1 << 8);
		Destroy(hit.transform.gameObject);

		if (onDig != null)
		{
			onDig();
		}
	}

	//On wrong answer
	void Hit()
	{
		if (onDig != null)
		{
			onDig();
		}
	}

}
