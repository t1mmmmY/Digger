using UnityEngine;
using System.Collections;

public class ScrollArea : MonoBehaviour 
{
	[SerializeField] Camera camera;
	[SerializeField] Transform cameraTransform;
	[SerializeField] Bounds[] cameraPositions;
	[SerializeField] Bounds scrollArea;
	[SerializeField] float scale = 10;
	[SerializeField] float minShift = 1;
	[SerializeField] int currentPosition = 0;

	Vector2 oldPosition;
	Vector2 deltaPosition;

	enum ScrollState
	{
		Begin,
		Scroll,
		End,
		Animate,
		Nothing
	}

	ScrollState state = ScrollState.Nothing;

//	[SerializeField] bool canDrag = true;


	void Start()
	{
		MoveToPosition(0);
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
//		Debug.Log("Begin");
		oldPosition = Input.mousePosition;
		deltaPosition = Vector2.zero;

//		state = ScrollState.Scroll;
	}

	void Scroll()
	{
//		Debug.Log("Scroll");
		deltaPosition = oldPosition - (Vector2)Input.mousePosition;

//		if (deltaPosition.magnitude < minShift)
//		{
//			return;
//		}

		deltaPosition.x = (deltaPosition.x / Screen.width) * 100 * scale;
//		deltaPosition.y = (deltaPosition.y / Screen.height) * 100 * scale;
		oldPosition = Input.mousePosition;

//		Debug.Log(deltaPosition);
		Vector3 newPosition = cameraTransform.localPosition;
		newPosition.x += deltaPosition.x;
//		newPosition.y += deltaPosition.y;

		cameraTransform.localPosition = newPosition;

		if (!scrollArea.Contains(cameraTransform.position))
		{
//			cameraTransform.position = Vector3.Lerp(cameraTransform.position, scrollArea.ClosestPoint(cameraTransform.position), Time.deltaTime * 10);
			cameraTransform.position = scrollArea.ClosestPoint(cameraTransform.position);
		}
	}

	void EndScroll()
	{
//		Debug.Log("End");
//		oldPosition = Vector2.zero;
//		deltaPosition = Vector2.zero;

		MoveToPosition(FindNearetPosition(cameraTransform.localPosition));
	}

	int FindNearetPosition(Vector3 position)
	{
		if (cameraPositions.Length == 0)
		{
			return 0;
		}

//		Rect cameraRect = camera.pixelRect;
//		Bounds cameraBounds = new Bounds(cameraTransform.localPosition, new Vector3(cameraRect.width, cameraRect.height, 1000));

		Bounds cameraBounds = new Bounds(cameraTransform.localPosition, new Vector3(720, 1280, 1000));
//		Debug.Log(cameraBounds);

		float minDistance = float.MaxValue;
		int number = currentPosition;

		for (int i = 0; i < cameraPositions.Length; i++)
		{
			if (cameraPositions[i].Intersects(cameraBounds) && i != currentPosition)
			{
				number = i;
			}
//			float distance = Vector3.Distance(position, cameraPositions[i].center);
//			if (distance < minDistance && i != currentPosition)
//			{
//				minDistance = distance;
//				number = i;
//			}
		}

		return number;
	}

	public void MoveToPosition(int positionNumber)
	{
		if (positionNumber >= cameraPositions.Length)
		{
			Debug.LogWarning("Out of range positions!");
			return;
		}

		state = ScrollState.Animate;

//		float distance = Vector3.Distance(cameraTransform.localPosition, cameraPositions[positionNumber].center);
//		float time = distance / 1000.0f;
//		Debug.Log(time);
//		canDrag = false;

		Hashtable hash = new Hashtable();
		hash.Add("position", cameraPositions[positionNumber].center);
		hash.Add("isLocal", true);
		hash.Add("time", 0.6f);
		hash.Add("oncomplete", "OnFinishMove");
		hash.Add("oncompletetarget", this.gameObject);

		iTween.MoveTo(cameraTransform.gameObject, hash);

		currentPosition = positionNumber;
	}

	void OnFinishMove()
	{
		state = ScrollState.Nothing;
//		canDrag = true;
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
