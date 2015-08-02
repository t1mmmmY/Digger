using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VoxelBusters.Utility
{
	[CustomEditor(typeof(ShaderUtility))]
	public class ShaderUtilityInspector : Editor 
	{
		#region Properties

		private				ShaderUtility			m_instance;
		private				SerializedProperty		m_shaderListProperty;

		#endregion

		#region Methods

		public void OnEnable ()
		{
			m_instance				= (target as ShaderUtility);
			m_shaderListProperty	= serializedObject.FindProperty("m_shaderList");
		}

		public override void OnInspectorGUI ()
		{
			if (GUILayout.Button("Reload Shader Utility"))
				m_instance.ReloadShaderUtility();

			// Draw default inspector
			EditorGUILayout.PropertyField(m_shaderListProperty, true);
		}

		#endregion
	}
}