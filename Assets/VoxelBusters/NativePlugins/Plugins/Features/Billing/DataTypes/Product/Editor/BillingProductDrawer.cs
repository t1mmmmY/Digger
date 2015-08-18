using UnityEngine;
using UnityEditor;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	[CustomPropertyDrawer(typeof(BillingProduct))]
	public class BillingProductDrawer : PropertyDrawer
	{
		#region Properties

		private float  m_tagPropertyHeight					= 20f;
		private float  m_tagPropertyHeightWithOffset		= 25f;
		private float  m_singleLinePropertyHeight			= EditorGUIUtility.singleLineHeight;
		private float  m_singleLinePropertyHeightWithOffset	= EditorGUIUtility.singleLineHeight + 3f;

		#endregion

		#region Drawer Methods

		public override void OnGUI (Rect _position, SerializedProperty _property, GUIContent _label)
		{
			// Serialized properties
			SerializedProperty _nameProperty				= _property.FindPropertyRelative("m_name");
			SerializedProperty _descriptionProperty			= _property.FindPropertyRelative("m_description");
			SerializedProperty _isConsumableProperty		= _property.FindPropertyRelative("m_isConsumable");
			SerializedProperty _iosProductIdProperty		= _property.FindPropertyRelative("m_iosProductId");
			SerializedProperty _androidProductIdProperty	= _property.FindPropertyRelative("m_androidProductId");

			// GUI Styles
			GUIStyle _tagStyle								= new GUIStyle("GUIEditor.BreadcrumbLeft");
			_tagStyle.fontStyle								= FontStyle.Bold;

			// Precalculate rects	
			float _positionY								= _position.y;
			Rect _tagLabelRect 								= new Rect(_position.x, _positionY, 200f, m_tagPropertyHeight);				
			_positionY += m_tagPropertyHeightWithOffset;
			Rect _namePropertyRect 							= new Rect(_position.x, _positionY, _position.width, m_singleLinePropertyHeight);	
			_positionY += m_singleLinePropertyHeightWithOffset;
			Rect _descPropertyRect 							= new Rect(_position.x, _positionY, _position.width, m_singleLinePropertyHeight);	
			_positionY += m_singleLinePropertyHeightWithOffset;
			Rect _isConsumableProductRect 					= new Rect(_position.x, _positionY, _position.width, m_singleLinePropertyHeight);	
			_positionY += m_singleLinePropertyHeightWithOffset;
			Rect _storeLabelRect							= new Rect(_position.x, _positionY, _position.width, m_singleLinePropertyHeight);	
			_positionY += m_singleLinePropertyHeightWithOffset;
			Rect _iosProductPropertyRect					= new Rect(_position.x, _positionY, _position.width, m_singleLinePropertyHeight);	
			_positionY += m_singleLinePropertyHeightWithOffset;
			Rect _androidProductPropertyRect				= new Rect(_position.x, _positionY, _position.width, m_singleLinePropertyHeight);	
			_positionY += m_singleLinePropertyHeightWithOffset;
				
			// Mark begin _property
			EditorGUI.BeginProperty(_position, _label, _property);
			EditorGUI.LabelField(_tagLabelRect, _label, _tagStyle);

			////////////////////////////////
	        //////// PRODUCT SECTION //////
	        //////////////////////////////
			EditorGUI.PropertyField(_namePropertyRect,
			                        _nameProperty, 
			                        new GUIContent("Name"));

			EditorGUI.PropertyField(_descPropertyRect,
			                        _descriptionProperty, 
			                        new GUIContent("Description"));
			
			EditorGUI.PropertyField(_isConsumableProductRect,
			                        _isConsumableProperty, 
			                        new GUIContent("Is Consumable"));

			////////////////////////////////
			//////// STORE SECTION ////////
			//////////////////////////////
			EditorGUI.LabelField(_storeLabelRect, 
			                     "Product Identifier(s)");

			// We will use next level of indentation
			EditorGUI.indentLevel++;

			// Show store
			EditorGUI.PropertyField(_iosProductPropertyRect,
			                        _iosProductIdProperty, 
			                        new GUIContent("iOS"),
			                        true);

			EditorGUI.PropertyField(_androidProductPropertyRect,
			                        _androidProductIdProperty, 
			                        new GUIContent("Android"),
			                        true);
			
			// Resetting indentation
			EditorGUI.indentLevel--;

			// End _property
			EditorGUI.EndProperty();
		}
		
		public override float GetPropertyHeight (SerializedProperty _property, GUIContent _label)
		{
			return m_tagPropertyHeightWithOffset + (m_singleLinePropertyHeightWithOffset * 6);
		}

		#endregion
	}
}