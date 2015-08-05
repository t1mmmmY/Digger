using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollArea : MonoBehaviour 
{
	[SerializeField] Camera camera;
	[SerializeField] Transform cameraTransform;
	[SerializeField] List<Bounds> cameraPositions; //550
	[SerializeField] Bounds scrollArea;
	[SerializeField] float scale = 10;
	[SerializeField] float minShift = 1;
	[SerializeField] int currentPosition = 0;
//    [SerializeField] float momentum = 0.5f;
    [SerializeField] float momentumStrong = 0.01f;
	[SerializeField] float minMomentumVelocity = 0.05f;
	int oldPositionNumber = 0;

	[Range(0, 500)]
	[SerializeField] float maxShift = 100;

	Vector2 oldPosition;
	Vector2 deltaPosition;

	bool beginScroll = false;
	bool isFinishingMoving = false;

	enum ScrollState
	{
		Begin,
		Scroll,
		Finishing,
		End,
		Animate,
		Nothing
	}

	ScrollState state = ScrollState.Nothing;

	public static System.Action onStartMoving;
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
		if (state != ScrollState.Animate)
		{
			if (Input.GetMouseButtonDown(0))
			{
				BeginScroll();
				state = ScrollState.Begin;
			}
			if (Input.GetMouseButton(0))
			{
				if (state == ScrollState.Nothing)
				{
					BeginScroll();
				}
				Scroll(Input.mousePosition);
				state = ScrollState.Scroll;
			}
			if (Input.GetMouseButtonUp(0) || (!Input.GetMouseButton(0) && (state == ScrollState.Scroll || state == ScrollState.Begin)))
			{
				if (momentumStrong > 0)
				{
					if (!isFinishingMoving)
					{
						Momentum();
					}
					state = ScrollState.Finishing;
				}
				else
				{
//					EndScroll();
					state = ScrollState.End;
				}
			}
			if (state == ScrollState.End)
			{
				EndScroll();
			}
			
//			switch (state)
//			{
//			case ScrollState.Begin:
//				
//				BeginScroll();
//				break;
//			case ScrollState.Scroll:
//				Scroll(Input.mousePosition);
//				break;
//			case ScrollState.Finishing:
//				if (!isFinishingMoving)
//				{
//					Momentum();
//				}
//				break;
//			case ScrollState.End:
//				EndScroll();
//				break;
//			}


//			if (Input.GetMouseButtonDown(0))
//			{
//				state = ScrollState.Begin;
//			}
//			else if (Input.GetMouseButton(0))
//			{
//				if (state == ScrollState.Nothing)
//				{
//					BeginScroll();
//				}
//				state = ScrollState.Scroll;
//			}
//			else if (Input.GetMouseButtonUp(0) || (!Input.GetMouseButton(0) && (state == ScrollState.Scroll || state == ScrollState.Begin)))
//			{
//				if (momentum > 0)
//				{
//					state = ScrollState.Finishing;
//				}
//				else
//				{
//					state = ScrollState.End;
//				}
//			}
//
//			switch (state)
//			{
//			case ScrollState.Begin:
//
//				BeginScroll();
//				break;
//			case ScrollState.Scroll:
//				Scroll(Input.mousePosition);
//				break;
//			case ScrollState.Finishing:
//				if (!isFinishingMoving)
//				{
//					Momentum();
//				}
//				break;
//			case ScrollState.End:
//                EndScroll();
//				break;
//			}


		}
	}

	void BeginScroll()
	{
		oldPosition = Input.mousePosition;
		deltaPosition = Vector2.zero;

		beginScroll = true;
	}

	void Scroll(Vector3 pointPosition)
	{
		deltaPosition = oldPosition - (Vector2)pointPosition;

		deltaPosition.x = (deltaPosition.x / Screen.width) * 100 * scale;

		if (beginScroll && deltaPosition.magnitude > 0)
		{
			if (onStartMoving != null)
			{
				onStartMoving();
			}
			beginScroll = false;
		}

		oldPosition = pointPosition;

		Vector3 newPosition = cameraTransform.localPosition;
		newPosition.x += deltaPosition.x;

		cameraTransform.localPosition = newPosition;


		if (!scrollArea.Contains(cameraTransform.position))
		{
			cameraTransform.position = scrollArea.ClosestPoint(cameraTransform.position);
		}
	}

    void Momentum()
    {
		isFinishingMoving = true;
        StartCoroutine("MomentumCoroutine");
    }

    IEnumerator MomentumCoroutine()
    {
        Vector3 cameraVelocity = camera.velocity;
		Vector3 pointPosition = oldPosition;
//		Vector3 newPosition = (Vector3)oldPosition + cameraVelocity * momentumStrong;
//        Vector3 distance = cameraVelocity;
       // float elapsedTime = 0;
        do
        {
			pointPosition = (Vector3)oldPosition - camera.velocity * momentumStrong;
			//pointPosition = Vector3.Lerp(oldPosition, newPosition, elapsedTime);
//            distance = Vector3.Lerp(cameraVelocity, Vector3.zero, elapsedTime) * momentumStrong;
			Scroll(pointPosition);
//            cameraTransform.Translate(distance);
            if (!scrollArea.Contains(cameraTransform.position))
            {
                cameraTransform.position = scrollArea.ClosestPoint(cameraTransform.position);
            }
            yield return null;
//
           // elapsedTime += Time.deltaTime / momentum;

		} while (/*elapsedTime < 1.0f && */Mathf.Abs(camera.velocity.magnitude) > minMomentumVelocity);

		isFinishingMoving = false;
		state = ScrollState.End;
    }

	void EndScroll()
	{
		MoveToPosition(FindNearetPosition(cameraTransform.localPosition));

		if (onChangePosition != null)
		{
			onChangePosition(currentPosition);
		}
		beginScroll = false;
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

		state = ScrollState.Animate;


		Hashtable hash = new Hashtable();
		hash.Add("position", cameraPositions[positionNumber].center);
		hash.Add("isLocal", false);
		hash.Add("time", 0.6f);
		hash.Add("oncomplete", "OnFinishMove");
		hash.Add("oncompletetarget", this.gameObject);

		iTween.MoveTo(cameraTransform.gameObject, hash);

		oldPositionNumber = currentPosition;
		currentPosition = positionNumber;

		if (onEndMoving != null)
		{
			//Debug.Log("OnEndMoving");
			onEndMoving();
		}
	}

	void OnFinishMove()
	{
        state = ScrollState.Nothing;
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
