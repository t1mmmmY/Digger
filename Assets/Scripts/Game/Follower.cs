using UnityEngine;
using System.Collections;

public class Follower : MonoBehaviour 
{
	public Transform target;
	public float speed = 1.0f;
	[SerializeField] Vector3 offset = new Vector3(0, 0, -10);

	void Update () 
	{
		FollowTarget();
	}

	void FollowTarget()
	{
		if (target != null)
		{
			transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * speed);
		}
	}

}
