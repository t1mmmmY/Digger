using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;

namespace VoxelBusters.Utility
{
	[CustomPropertyDrawer(typeof(ExecuteOnValueChangeAttribute))]
	public class ExecuteOnValueChangeDrawer : PropertyDrawer 
	{
		#region Properties

		private ExecuteOnValueChangeAttribute ExecuteOnValueChange 
		{ 
			get { return ((ExecuteOnValueChangeAttribute)attribute); } 
		}

		#endregion

		#region Drawer Methods

		public override float GetPropertyHeight (SerializedProperty _property, GUIContent _label) 
		{
			return base.GetPropertyHeight(_property, _label);
		}

		public override void OnGUI (Rect _position, SerializedProperty _property, GUIContent _label)
		{
			EditorGUI.BeginProperty(_position, _label, _property);

			// Start checking if property was changed
			EditorGUI.BeginChangeCheck();

			// Call base class to draw property
			EditorGUI.PropertyField(_position, _property, _label, true);

			// Finish checking and invoke method if value is changed
			if (EditorGUI.EndChangeCheck())
			{
				// Apply value change
				_property.serializedObject.ApplyModifiedProperties();

				// Trigger callback
				object 			_instance		= _property.serializedObject.targetObject;
				BindingFlags 	_bindingAttr	= BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.OptionalParamBinding;
				MethodInfo 		_methodInfo		= _instance.GetType().GetMethod(ExecuteOnValueChange.Function, _bindingAttr);
				
				if (_methodInfo != null)
					_methodInfo.Invoke(_instance, null);
			}

			EditorGUI.EndProperty();
		}

		#endregion
	}
}
