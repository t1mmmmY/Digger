using UnityEngine;
using System.Collections;

public class AutoTileSetManager : MonoBehaviour {
	[Header("Grid Options")]
	public Vector2 gridSize=Vector2.one;
	public Vector3 offset;
	public bool displayGrid=true;
	public Color gridColor;
	[Header("Draw Options")]
	public GameObject currentTile;

	public static AutoTileSetManager instance;

	void Awake()
	{
		instance = this;
	}

	void OnDrawGizmos() {
		if (displayGrid) { 
			Gizmos.color=gridColor*0.5f;
			DrawGrid();
		}
	}
	
	void OnDrawGizmosSelected() {
		if (displayGrid) { 
			Gizmos.color=gridColor;
			DrawGrid();
		}
	}
	
	void DrawGrid() {
		Vector3 pos = Camera.current.transform.position-Vector3.one;
		
		for (float y = pos.y - 800.0f; y < pos.y + 800.0f; y+= gridSize.y) {
			Gizmos.DrawLine(new Vector3(-1000000.0f, Mathf.Floor(y/gridSize.y) * gridSize.y+offset.y, offset.z),
			                new Vector3(1000000.0f,  Mathf.Floor(y/gridSize.y) * gridSize.y+offset.y, offset.z));
		}
		
		for (float x = pos.x - 1200.0f; x < pos.x + 1200.0f; x+= gridSize.x) {
			Gizmos.DrawLine (new Vector3 (Mathf.Floor (x / gridSize.x) * gridSize.x + offset.x, -1000000.0f, offset.z),
			                 new Vector3 (Mathf.Floor (x / gridSize.x) * gridSize.x + offset.x, 1000000.0f, offset.z));
				}
	}


	public GameObject CreateTile(Vector2 position)
	{
		GameObject newObject = currentTile;
		//GameObject newObject=(GameObject)serializedObject.FindProperty("currentTile").objectReferenceValue;
		try {
			newObject=(GameObject)GameObject.Instantiate(newObject);
			newObject.layer = this.gameObject.layer;
//			newObject=(GameObject)PrefabUtility.InstantiatePrefab(newObject);
			newObject.transform.position = position;
//			newObject.transform.position = Camera.main.ScreenPointToRay(position).origin;
//			newObject.transform.position=HandleUtility.GUIPointToWorldRay(position).origin;
			newObject.transform.rotation=Quaternion.identity;
			newObject.transform.position=new Vector3(newObject.transform.position.x, newObject.transform.position.y, 0);
			newObject.transform.parent= this.transform;
			newObject.name=newObject.name.Replace("(Clone)", "");
//			newObject.transform.parent=((Component)serializedObject.targetObject).gameObject.transform;
			return newObject;
//			Undo.RegisterCreatedObjectUndo(newObject, "Created new prefab tile");
		} catch {
			newObject=(GameObject)GameObject.Instantiate(newObject, position, Quaternion.identity);
			newObject.layer = this.gameObject.layer;
//			newObject=(GameObject)GameObject.Instantiate(newObject, Camera.main.ScreenPointToRay(position).origin, Quaternion.identity);
//			newObject=(GameObject)Instantiate(newObject, HandleUtility.GUIPointToWorldRay(position).origin, Quaternion.identity);
			newObject.transform.position=new Vector3(newObject.transform.position.x, newObject.transform.position.y, 0);
			newObject.transform.parent= this.transform;
//			newObject.transform.parent=((Component)serializedObject.targetObject).gameObject.transform;
			newObject.name=newObject.name.Replace("(Clone)", "");
			return newObject;
//			Undo.RegisterCreatedObjectUndo(newObject, "Created new tile");
		}
	}

}
