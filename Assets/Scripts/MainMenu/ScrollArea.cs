using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollArea : MonoBehaviour 
{
	[SerializeField] Camera camera;
	[SerializeField] Transform cameraTransform;
	[SerializeField] List<Bounds> cameraPositions; //550
	[SerializeField] Bounds scrollArea;
	[SerializeField] float scale = 7.5f;
	[SerializeField] int currentPosition = 0;
	[SerializeField] float minScrollVelocity = 0.0f;
	int oldPositionNumber = 0;

	Touch touch;
	
	Vector2 oldPosition;
	Vector2 initPosition;
	Vector2 speed = Vector2.zero;
	Vector3 oldPointPosition = Vector3.zero;
	bool scroll = false;

	
	public static System.Action<int> onChangePosition;
	public static System.Action onEndMoving;

	private static bool _canSwipe = true;
	public static bool canSwipe
	{
		get { return _canSwipe; }
		set { _canSwipe = value; }
	}
	
	void Start()
	{
		MoveToPosition(0);
	}
	
	public Bounds GetScrollArea()
	{
		return scrollArea;
	}
	
	public void AddPoint(Bounds bound)
	{
		if (cameraPositions == null)
		{
			cameraPositions = new List<Bounds>();
		}
		cameraPositions.Add(bound);
	}
	
	public void ChangeScrollArea(Bounds bound)
	{
		scrollArea = bound;
	}

	void Update()
	{
		if (Input.touchCount > 0)
		{
			touch = Input.touches[0];
			switch (touch.phase) 
			{
				case TouchPhase.Began:
					
					break;
				case TouchPhase.Canceled:
					break;
				case TouchPhase.Ended:
					EndScroll();
					break;
				case TouchPhase.Moved:
					Scroll(touch.deltaPosition);
					break;
				case TouchPhase.Stationary:
					break;
			}
		}
	}

	
	bool Scroll(Vector3 deltaPosition)
	{
		if (!_canSwipe)
		{
			return false;
		}

		if (!scroll)
		{
			iTween.Stop(cameraTransform.gameObject);
		}

		scroll = true;

		Vector3 newPosition = cameraTransform.localPosition;
		newPosition.x -= deltaPosition.x;
		
		cameraTransform.localPosition = newPosition;
		
		
		if (!scrollArea.Contains(cameraTransform.position))
		{
			cameraTransform.position = scrollArea.ClosestPoint(cameraTransform.position);
		}
		
		if (camera.velocity.magnitude > minScrollVelocity)
		{
			
			speed = camera.velocity;
		}
		
		return false;
	}

	
	void EndScroll()
	{
		if (scroll)
		{
			scroll = false;
			MoveToPosition(FindNearetPosition(cameraTransform.localPosition));
		}
		
		if (onChangePosition != null)
		{
			onChangePosition(currentPosition);
		}
		
	}
	
	int FindNearetPosition(Vector3 position)
	{
		if (cameraPositions.Count == 0)
		{
			return 0;
		}
		
		Bounds cameraBounds = new Bounds(cameraTransform.position, new Vector3(1, 1, 1000));
		
		float minDistance = float.MaxValue;
		int number = currentPosition;
		
		for (int i = 0; i < cameraPositions.Count; i++)
		{
			if (cameraPositions[i].Intersects(cameraBounds) && i != currentPosition)
			{
				number = i;
			}
		}
		
		return number;
	}
	
	public bool MoveLeft()
	{
		if (currentPosition - 1 < 0)
		{
			return false;
		}
		else
		{
			MoveToPosition(currentPosition - 1);
			
			return true;
		}
	}
	
	public bool MoveRight()
	{
		if (currentPosition + 1 >= cameraPositions.Count)
		{
			return false;
		}
		else
		{
			MoveToPosition(currentPosition + 1);
			
			return true;
		}
	}
	
	public void MoveToPosition(int positionNumber)
	{
		if (positionNumber >= cameraPositions.Count)
		{
			Debug.LogWarning("Out of range positions!");
			return;
		}

		
		Hashtable hash = new Hashtable();
		hash.Add("position", cameraPositions[positionNumber].center);
		hash.Add("isLocal", false);
		hash.Add("time", 1.0f);
		hash.Add("oncomplete", "OnFinishMove");
		hash.Add("oncompletetarget", this.gameObject);

		iTween.MoveTo(cameraTransform.gameObject, hash);
		
		oldPositionNumber = currentPosition;
		currentPosition = positionNumber;
		
		if (onEndMoving != null)
		{
			onEndMoving();
		}
	}
	
	void OnFinishMove()
	{
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		
		Gizmos.DrawLine(new Vector2(scrollArea.min.x, scrollArea.min.y), new Vector2(scrollArea.max.x, scrollArea.min.y));
		Gizmos.DrawLine(new Vector2(scrollArea.min.x, scrollArea.min.y), new Vector2(scrollArea.min.x, scrollArea.max.y));
		Gizmos.DrawLine(new Vector2(scrollArea.min.x, scrollArea.max.y), new Vector2(scrollArea.max.x, scrollArea.max.y));
		Gizmos.DrawLine(new Vector2(scrollArea.max.x, scrollArea.min.y), new Vector2(scrollArea.max.x, scrollArea.max.y));
	}
	
}