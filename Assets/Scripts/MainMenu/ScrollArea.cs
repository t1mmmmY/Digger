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
	int oldPositionNumber = 0;

	[Range(0, 500)]
	[SerializeField] float maxShift = 100;

	Vector2 oldPosition;
	Vector2 deltaPosition;

	bool beginScroll = false;

	enum ScrollState
	{
		Begin,
		Scroll,
		End,
		Animate,
		Nothing
	}

	ScrollState state = ScrollState.Nothing;

	public static System.Action onStartMoving;
	public static System.Action<int> onChangePosition;
	public static System.Action onEndMoving;

//	void Awake()
//	{
//		cameraPositions = new List<Bounds>();
//	}

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
				state = ScrollState.Begin;
			}
			else if (Input.GetMouseButton(0))
			{
				if (state == ScrollState.Nothing)
				{
					BeginScroll();
				}
				state = ScrollState.Scroll;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				state = ScrollState.End;
			}

			switch (state)
			{
			case ScrollState.Begin:

				BeginScroll();
				break;
			case ScrollState.Scroll:
				Scroll();
				break;
			case ScrollState.End:

				EndScroll();
				break;
			}


		}
	}

	void BeginScroll()
	{
		oldPosition = Input.mousePosition;
		deltaPosition = Vector2.zero;

		beginScroll = true;
	}

	void Scroll()
	{
		deltaPosition = oldPosition - (Vector2)Input.mousePosition;

		deltaPosition.x = (deltaPosition.x / Screen.width) * 100 * scale;

		if (beginScroll && deltaPosition.magnitude > 0)
		{
			if (onStartMoving != null)
			{
				onStartMoving();
			}
			beginScroll = false;
		}

		oldPosition = Input.mousePosition;

		Vector3 newPosition = cameraTransform.localPosition;
		newPosition.x += deltaPosition.x;

		cameraTransform.localPosition = newPosition;


		if (!scrollArea.Contains(cameraTransform.position))
		{
			cameraTransform.position = scrollArea.ClosestPoint(cameraTransform.position);
		}
	}

	void EndScroll()
	{
		MoveToPosition(FindNearetPosition(cameraTransform.localPosition));

//		if (oldPositionNumber != currentPosition)
//		{
		if (onChangePosition != null)
		{
			onChangePosition(currentPosition);
		}
//		}
		beginScroll = false;
	}

	int FindNearetPosition(Vector3 position)
	{
		if (cameraPositions.Count == 0)
		{
			return 0;
		}

//		Bounds cameraBounds = new Bounds(cameraTransform.localPosition, new Vector3(720, 1280, 1000));
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

	public void MoveToPosition(int positionNumber)
	{
		if (positionNumber >= cameraPositions.Count)
		{
			Debug.LogWarning("Out of range positions!");
			return;
		}

		state = ScrollState.Animate;


		Hashtable hash = new Hashtable();
//		hash.Add("position", cameraPositions[positionNumber].center);
//		hash.Add("isLocal", true);
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
			onEndMoving();
		}
	}

	void OnFinishMove()
	{
		state = ScrollState.Nothing;

//		if (onChangePosition != null)
//		{
//			onChangePosition(currentPosition);
//		}
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
