#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AutoTileSetManager))]
public class AutoTileSetManagerEditor : Editor {

	System.Action TileTool;
	Ray ray;
	Collider2D pointHit, radiusHit;

	void OnSceneGUI() { 
		Event current = Event.current;
		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		HandleUtility.AddDefaultControl(controlID);
		EventType currentEventType=current.GetTypeForControl(controlID);
		bool skip=false;
		int saveControl=GUIUtility.hotControl;
		
		Ray ray=HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		pointHit =Physics2D.OverlapPoint (ray.origin);
		radiusHit=Physics2D.OverlapCircle(ray.origin, 0.333f);
		if (!pointHit && radiusHit) {
			AutoTile currentTile=radiusHit.gameObject.GetComponent<AutoTile>();
			if (currentTile != null)
			{
				if (currentTile.autoTileMode==AutoTileMode.Slope) {
					pointHit=radiusHit;
				}
			}
		}
		
		if (currentEventType==EventType.Layout)      {skip=true;}
		if (currentEventType==EventType.ScrollWheel) {skip=true;}
		if (current.button!=0)                       {skip=true;}
		
		if (style==null) {
			style = new GUIStyle();
			style.normal.textColor = Color.red;
			style.normal.background=new Texture2D(1,1);
			style.normal.background.SetPixel(0,0,Color.white);
			style.normal.background.Apply();
		}
		
		if (SceneView.currentDrawingSceneView.in2DMode) {
			style.normal.textColor = Color.black;
			Handles.Label(HandleUtility.GUIPointToWorldRay(Vector3.zero).origin, "LMB: Draw/Erase , RMB: Toggle corners", style);
			if (current.button==0 && !skip) {
				GUIUtility.hotControl=controlID;
				
				switch (current.type) {
				    case EventType.KeyDown: if (current.keyCode==KeyCode.Escape) {Selection.activeGameObject=null; Event.current.Use(); } break;
				    case EventType.MouseDown: GetTool();  break;
				    case EventType.MouseDrag: TileTool(); break;
				    case EventType.MouseUp:	  TileTool=null; break;
				}
			}
		} else {
			style.normal.textColor = Color.red;
			Handles.Label(HandleUtility.GUIPointToWorldRay(Vector3.zero).origin, "Tile editing requires 2D mode", style);
		}
		
		if (current.button==1 && current.type==EventType.MouseDown) {
			ToggleTool();
		}
		
		GUIUtility.hotControl=saveControl;
		if (GUI.changed) {
			EditorUtility.SetDirty(target);
		}
	}
	GUIStyle style;
		
	void GetTool() {		
		if (pointHit) {
			TileTool=EraseTool;
		} else {
			TileTool=DrawTool;
		}
		TileTool();
	}
	
	void ColorPickTile() {
		if (pointHit) {
			GameObject newPrefab=pointHit.gameObject;
			((AutoTileSetManager)serializedObject.targetObject).currentTile=(GameObject)PrefabUtility.GetPrefabParent(newPrefab);
		}
	}

	void DrawTool() {
		if (!pointHit) {
			CreateTile(Event.current.mousePosition);
		}
	}

	void CreateTile(Vector2 position)
	{
		GameObject newObject=(GameObject)serializedObject.FindProperty("currentTile").objectReferenceValue;
		try {
			newObject=(GameObject)PrefabUtility.InstantiatePrefab(newObject);
			newObject.transform.position=HandleUtility.GUIPointToWorldRay(position).origin;
			newObject.transform.rotation=Quaternion.identity;
			newObject.transform.position=new Vector3(newObject.transform.position.x, newObject.transform.position.y, 0);
			newObject.transform.parent=((Component)serializedObject.targetObject).gameObject.transform;
			newObject.layer = newObject.transform.parent.gameObject.layer;
			Undo.RegisterCreatedObjectUndo(newObject, "Created new prefab tile");
		} catch {
			newObject=(GameObject)Instantiate(newObject, HandleUtility.GUIPointToWorldRay(position).origin, Quaternion.identity);
			newObject.transform.position=new Vector3(newObject.transform.position.x, newObject.transform.position.y, 0);
			newObject.transform.parent=((Component)serializedObject.targetObject).gameObject.transform;
			newObject.layer = newObject.transform.parent.gameObject.layer;
			newObject.name=newObject.name.Replace("(Clone)", "");
			Undo.RegisterCreatedObjectUndo(newObject, "Created new tile");
		}
	}
	
	void EraseTool() {
		if (pointHit && pointHit.gameObject.GetComponent<AutoTile>()!=null) {
			DestroyImmediate(pointHit.gameObject);
		}
	}
	
	void ToggleTool() {
		if (pointHit && pointHit.gameObject.GetComponent<AutoTile>()!=null) {
			AutoTile currentTile=pointHit.gameObject.GetComponent<AutoTile>();
			AutoTileMode newValue;
			switch (currentTile.autoTileMode) {
			case AutoTileMode.Corner:     newValue=AutoTileMode.Slope; break;
			case AutoTileMode.Slope:      newValue=AutoTileMode.Corner; break;
			default:                      newValue=AutoTileMode.Corner; break;
			}
			SerializedObject target=new SerializedObject(currentTile);
			target.FindProperty("autoTileMode").enumValueIndex=(int)newValue;
			target.ApplyModifiedProperties();
			currentTile.UpdateTile();
		}
	}
	
	Tool savedTool;
	void OnEnable() {
		savedTool=Tools.current;
		Tools.current=Tool.None;
	}
	
	void OnDisable() {
		Tools.current=savedTool;
	}
}

#endif
