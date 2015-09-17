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
//	[SerializeField] float minShift = 1;
	[SerializeField] int currentPosition = 0;
//	[SerializeField] float momentumStrong = 0.01f;
//	[SerializeField] float minMomentumVelocity = 0.05f;
	[SerializeField] float minScrollVelocity = 0.0f;
	int oldPositionNumber = 0;

	Touch touch;
	
	Vector2 oldPosition;
	Vector2 initPosition;
//	Vector2 deltaPosition;
	Vector2 speed = Vector2.zero;
	Vector3 oldPointPosition = Vector3.zero;
	bool scroll = false;
	
//	bool beginScroll = false;
//	bool isFinishingMoving = false;
//	bool tick = false;
	
//	enum ScrollState
//	{
//		Begin,
//		Scroll,
//		Finishing,
//		End,
//		Animate,
//		Nothing
//	}
	
//	ScrollState state = ScrollState.Nothing;
	
//	public static System.Action onStartMoving;
	public static System.Action<int> onChangePosition;
	public static System.Action onEndMoving;
	
	
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
//#if UNITY_EDITOR

//#else
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
//#endif
	}
	
//	void Update()
//	{
//		
//		if (state != ScrollState.Animate && !isFinishingMoving)
//		{
//			bool endMoving = false;
//			tick = false;
//			
//			if (Input.GetMouseButtonDown(0))
//			{
//				BeginScroll();
//				state = ScrollState.Begin;
//			}
//			if (Input.GetMouseButton(0) && state != ScrollState.Finishing && state != ScrollState.End)
//			{
//				if (state == ScrollState.Nothing)
//				{
//					BeginScroll();
//				}
//				if ((Vector2)Input.mousePosition == oldPosition && state == ScrollState.Begin)
//				{
//					
//				}
//				else
//				{
//					Scroll(Input.mousePosition);
//					state = ScrollState.Scroll;
//				}
//				
//			}
//			if (Input.touches.Length > 0 && state == ScrollState.Scroll)
//			{
//				tick = true;
//			}
//			
//			#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
//			if ((state == ScrollState.Scroll || state == ScrollState.Begin) && Input.touches.Length == 0)
//			{
//				isFinishingMoving = true;
//			}
//			#endif
//			
//			if (!tick && state == ScrollState.Scroll)
//			{
//				isFinishingMoving = true;
//			}
//			
//			
//			if (isFinishingMoving)
//			{
//				if (momentumStrong > 0)
//				{
//					Momentum();
//					state = ScrollState.Finishing;
//				}
//				else
//				{
//					state = ScrollState.End;
//				}
//			}
//			
//			if (state == ScrollState.End)
//			{
//				EndScroll();
//			}
//			
//		}
//	}
	
//	void BeginScroll()
//	{
//		oldPosition = Input.mousePosition;
//		deltaPosition = Vector2.zero;
//		initPosition = oldPosition;
//		
//		beginScroll = true;
//		
//	}
//	
//	IEnumerator LookForMouseButtonUp()
//	{
//		bool isUp = false;
//		do
//		{
//			if (Input.GetMouseButtonUp(0))
//			{
//				Debug.Log("Register Up");
//				isUp = true;
//				isFinishingMoving = true;
//			}
//			yield return null;
//		} while (!isUp);
//	}

	
	bool Scroll(Vector3 deltaPosition)
	{
		
//		if (Vector3.Distance(pointPosition, oldPointPosition) <= minScrollVelocity && (Vector2)pointPosition != initPosition)
//		{
//			Debug.Log(Vector3.Distance(pointPosition, oldPointPosition));
//			return false;
//		}
//		else
//		{
//			
//		}
		
//		tick = true;
		
//		Vector2 oldDeltaPosition = Vector2.zero;
//		oldDeltaPosition = deltaPosition;
//		deltaPosition = oldPosition - (Vector2)pointPosition;
//		
//		deltaPosition.x = (deltaPosition.x / Screen.width) * 100 * scale;
		
//		if (beginScroll && deltaPosition.magnitude > 0)
//		{
//			if (onStartMoving != null)
//			{
//				onStartMoving();
//			}
//			beginScroll = false;
//		}

//		oldPosition = cameraTransform.localPosition;

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
			//			Debug.Log(speed);
		}
		
//		oldPointPosition = pointPosition;
		
		
		return false;
	}
	
//	void Momentum()
//	{
//		StartCoroutine("MomentumCoroutine");
//	}
	
//	IEnumerator MomentumCoroutine()
//	{
//		isFinishingMoving = false;
//		Vector3 pointPosition = oldPosition;
//		do
//		{
//			pointPosition = (Vector3)oldPosition - (Vector3)speed * momentumStrong;
//			Scroll(pointPosition);
//			
//			
//			if (!scrollArea.Contains(cameraTransform.position))
//			{
//				cameraTransform.position = scrollArea.ClosestPoint(cameraTransform.position);
//			}
//			yield return null;
//			
//		} while (Mathf.Abs(camera.velocity.magnitude) > minMomentumVelocity);
//		
//		state = ScrollState.End;
//	}
	
	void EndScroll()
	{
		if (scroll)
		{
			scroll = false;
			MoveToPosition(FindNearetPosition(cameraTransform.localPosition));
		}
//		isFinishingMoving = false;
//		MoveToPosition(FindNearetPosition(cameraTransform.localPosition));
		
		if (onChangePosition != null)
		{
			onChangePosition(currentPosition);
		}
//		beginScroll = false;
		
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
		
//		state = ScrollState.Animate;
		
		
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
//		state = ScrollState.Nothing;
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




//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//
//public class ScrollArea : MonoBehaviour 
//{
//	[SerializeField] Camera camera;
//	[SerializeField] Transform cameraTransform;
//	[SerializeField] List<Bounds> cameraPositions; //550
//	[SerializeField] Bounds scrollArea;
//	[SerializeField] float scale = 10;
//	[SerializeField] float minShift = 1;
//	[SerializeField] int currentPosition = 0;
//    [SerializeField] float momentumStrong = 0.01f;
//	[SerializeField] float minMomentumVelocity = 0.05f;
//	[SerializeField] float minScrollVelocity = 0.0f;
//	int oldPositionNumber = 0;
//
//
//	Vector2 oldPosition;
//	Vector2 initPosition;
//	Vector2 deltaPosition;
//	Vector2 speed = Vector2.zero;
//
//	bool beginScroll = false;
//	bool isFinishingMoving = false;
//	bool tick = false;
//
//	enum ScrollState
//	{
//		Begin,
//		Scroll,
//		Finishing,
//		End,
//		Animate,
//		Nothing
//	}
//
//	ScrollState state = ScrollState.Nothing;
//
//	public static System.Action onStartMoving;
//	public static System.Action<int> onChangePosition;
//	public static System.Action onEndMoving;
//
//
//	void Start()
//	{
//		MoveToPosition(0);
//	}
//
//	public Bounds GetScrollArea()
//	{
//		return scrollArea;
//	}
//
//	public void AddPoint(Bounds bound)
//	{
//		if (cameraPositions == null)
//		{
//			cameraPositions = new List<Bounds>();
//		}
//		cameraPositions.Add(bound);
//	}
//
//	public void ChangeScrollArea(Bounds bound)
//	{
//		scrollArea = bound;
//	}
//
//
//	void Update()
//	{
//
//		if (state != ScrollState.Animate && !isFinishingMoving)
//		{
//			bool endMoving = false;
//			tick = false;
//
//			if (Input.GetMouseButtonDown(0))
//			{
//				BeginScroll();
//				state = ScrollState.Begin;
//			}
//			if (Input.GetMouseButton(0) && state != ScrollState.Finishing && state != ScrollState.End)
//			{
//				if (state == ScrollState.Nothing)
//				{
//					BeginScroll();
//				}
//				if ((Vector2)Input.mousePosition == oldPosition && state == ScrollState.Begin)
//				{
//
//				}
//				else
//				{
//					Scroll(Input.mousePosition);
//					state = ScrollState.Scroll;
//				}
//
//			}
//			if (Input.touches.Length > 0 && state == ScrollState.Scroll)
//			{
//				tick = true;
//			}
//
//#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
//			if ((state == ScrollState.Scroll || state == ScrollState.Begin) && Input.touches.Length == 0)
//			{
//				isFinishingMoving = true;
//			}
//#endif
//
//			if (!tick && state == ScrollState.Scroll)
//			{
//				isFinishingMoving = true;
//			}
//
//
//			if (isFinishingMoving)
//			{
//				if (momentumStrong > 0)
//				{
//					Momentum();
//					state = ScrollState.Finishing;
//				}
//				else
//				{
//					state = ScrollState.End;
//				}
//			}
//
//			if (state == ScrollState.End)
//			{
//				EndScroll();
//			}
//
//		}
//	}
//
//	void BeginScroll()
//	{
//		oldPosition = Input.mousePosition;
//		deltaPosition = Vector2.zero;
//		initPosition = oldPosition;
//
//		beginScroll = true;
//
//	}
//
//	IEnumerator LookForMouseButtonUp()
//	{
//		bool isUp = false;
//		do
//		{
//			if (Input.GetMouseButtonUp(0))
//			{
//				Debug.Log("Register Up");
//				isUp = true;
//				isFinishingMoving = true;
//			}
//			yield return null;
//		} while (!isUp);
//	}
//
//	Vector3 oldPointPosition = Vector3.zero;
//
//	bool Scroll(Vector3 pointPosition)
//	{
//
//		if (Vector3.Distance(pointPosition, oldPointPosition) <= minScrollVelocity && (Vector2)pointPosition != initPosition)
//		{
//			Debug.Log(Vector3.Distance(pointPosition, oldPointPosition));
//			return false;
//		}
//		else
//		{
//
//		}
//
//		tick = true;
//
//		Vector2 oldDeltaPosition = Vector2.zero;
//		oldDeltaPosition = deltaPosition;
//		deltaPosition = oldPosition - (Vector2)pointPosition;
//
//		deltaPosition.x = (deltaPosition.x / Screen.width) * 100 * scale;
//
//		if (beginScroll && deltaPosition.magnitude > 0)
//		{
//			if (onStartMoving != null)
//			{
//				onStartMoving();
//			}
//			beginScroll = false;
//		}
//
//		Vector3 newPosition = cameraTransform.localPosition;
//		newPosition.x += deltaPosition.x;
//
//		cameraTransform.localPosition = newPosition;
//
//
//			
//		oldPosition = pointPosition;
//
//		if (!scrollArea.Contains(cameraTransform.position))
//		{
//			cameraTransform.position = scrollArea.ClosestPoint(cameraTransform.position);
//		}
//
//		if (camera.velocity.magnitude > minScrollVelocity)
//		{
//
//			speed = camera.velocity;
////			Debug.Log(speed);
//		}
//
//		oldPointPosition = pointPosition;
//
//
//		return false;
//	}
//
//    void Momentum()
//    {
//        StartCoroutine("MomentumCoroutine");
//    }
//
//    IEnumerator MomentumCoroutine()
//    {
//		isFinishingMoving = false;
//		Vector3 pointPosition = oldPosition;
//        do
//        {
//			pointPosition = (Vector3)oldPosition - (Vector3)speed * momentumStrong;
//			Scroll(pointPosition);
//
//
//            if (!scrollArea.Contains(cameraTransform.position))
//            {
//                cameraTransform.position = scrollArea.ClosestPoint(cameraTransform.position);
//            }
//            yield return null;
//
//		} while (Mathf.Abs(camera.velocity.magnitude) > minMomentumVelocity);
//
//		state = ScrollState.End;
//    }
//
//	void EndScroll()
//	{
//		isFinishingMoving = false;
//		MoveToPosition(FindNearetPosition(cameraTransform.localPosition));
//
//		if (onChangePosition != null)
//		{
//			onChangePosition(currentPosition);
//		}
//		beginScroll = false;
//
//	}
//
//	int FindNearetPosition(Vector3 position)
//	{
//		if (cameraPositions.Count == 0)
//		{
//			return 0;
//		}
//
//		Bounds cameraBounds = new Bounds(cameraTransform.position, new Vector3(1, 1, 1000));
//
//		float minDistance = float.MaxValue;
//		int number = currentPosition;
//
//		for (int i = 0; i < cameraPositions.Count; i++)
//		{
//			if (cameraPositions[i].Intersects(cameraBounds) && i != currentPosition)
//			{
//				number = i;
//			}
//		}
//
//		return number;
//	}
//
//    public bool MoveLeft()
//    {
//        if (currentPosition - 1 < 0)
//        {
//            return false;
//        }
//        else
//        {
//            MoveToPosition(currentPosition - 1);
//
//            return true;
//        }
//    }
//
//    public bool MoveRight()
//    {
//        if (currentPosition + 1 >= cameraPositions.Count)
//        {
//            return false;
//        }
//        else
//        {
//            MoveToPosition(currentPosition + 1);
//
//            return true;
//        }
//    }
//
//	public void MoveToPosition(int positionNumber)
//	{
//		if (positionNumber >= cameraPositions.Count)
//		{
//			Debug.LogWarning("Out of range positions!");
//			return;
//		}
//
//		state = ScrollState.Animate;
//
//
//		Hashtable hash = new Hashtable();
//		hash.Add("position", cameraPositions[positionNumber].center);
//		hash.Add("isLocal", false);
//		hash.Add("time", 1.0f);
//		hash.Add("oncomplete", "OnFinishMove");
//		hash.Add("oncompletetarget", this.gameObject);
//
//		iTween.MoveTo(cameraTransform.gameObject, hash);
//
//		oldPositionNumber = currentPosition;
//		currentPosition = positionNumber;
//
//		if (onEndMoving != null)
//		{
//			onEndMoving();
//		}
//	}
//
//	void OnFinishMove()
//	{
//        state = ScrollState.Nothing;
//	}
//
//	void OnDrawGizmos()
//	{
//		Gizmos.color = Color.green;
//
//		Gizmos.DrawLine(new Vector2(scrollArea.min.x, scrollArea.min.y), new Vector2(scrollArea.max.x, scrollArea.min.y));
//		Gizmos.DrawLine(new Vector2(scrollArea.min.x, scrollArea.min.y), new Vector2(scrollArea.min.x, scrollArea.max.y));
//		Gizmos.DrawLine(new Vector2(scrollArea.min.x, scrollArea.max.y), new Vector2(scrollArea.max.x, scrollArea.max.y));
//		Gizmos.DrawLine(new Vector2(scrollArea.max.x, scrollArea.min.y), new Vector2(scrollArea.max.x, scrollArea.max.y));
//	}
//
//}
