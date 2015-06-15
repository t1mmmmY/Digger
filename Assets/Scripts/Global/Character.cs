using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour 
{
	[SerializeField] int _number = 0;

	public int number
	{
		get { return _number; }
	}
}
