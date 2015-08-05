using UnityEngine;
using System.Collections;

public class TavernCharacter : MonoBehaviour 
{
	[SerializeField] Character character;

	Vector3 oldPosition = Vector3.zero;
	float maxDistance = 0.1f;

	void OnMouseDown()
	{
        

		oldPosition = Input.mousePosition;
	}

	void OnMouseUp()
	{
		if (Vector3.Distance(oldPosition, Input.mousePosition) <= maxDistance)
		{
			if (TavernManager.Instance != null)
			{
	            if (TavernManager.Instance.currentCharacter == character.number)
	            {
	                TavernManager.Instance.SelectCharacter(character);
	            }
			}
		}
	}
}
