using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	[CustomEditor(typeof(EditorGameCenter))]
	public class EditorGameCenterInspector : Editor 
	{
		#region Methods

		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector();
			
			if (GUILayout.Button("Sign Out"))
				(target as EditorGameCenter).SignOut();
			
			if (GUILayout.Button("Reset Achievements"))
				(target as EditorGameCenter).ResetAllAchievements(null);
			
			// Apply modified values
			if (GUI.changed)		
				serializedObject.ApplyModifiedProperties();
		}

		#endregion
	}
}
