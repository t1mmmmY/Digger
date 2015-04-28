using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SimpleTile : MonoBehaviour 
{
	public bool fitToGrid = true;
	public float tileSize = 1;

	protected Vector3 checkPosition;

	void Awake() 
	{
		checkPosition = transform.position;
	}

	void Update() 
	{
		Vector3 displacement = transform.position - checkPosition;
		if (fitToGrid && (displacement.x != 0 || displacement.y != 0)) 
		{
			transform.position = new Vector3(Mathf.Round(transform.position.x / tileSize) * tileSize, 
			                                 Mathf.Round(transform.position.y / tileSize) * tileSize, 
			                                 transform.position.z);
		}
	}
}
