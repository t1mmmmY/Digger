using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour 
{
	[SerializeField] Transform player;
	[SerializeField] float speed = 1.0f;
	[SerializeField] Vector3 offset = new Vector3(0, 0, -10);

	void Update () 
	{
		FollowTarget(player);
	}

	void FollowTarget(Transform target)
	{
		transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
	}

}
