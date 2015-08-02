using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VoxelBusters.Utility
{
	public class EnumMaskFieldAttribute : PropertyAttribute 
	{
		#region Properties

		private System.Type		TargetType 
		{ 
			get; 
			set; 
		}

		#endregion

		#region Constructors

		private EnumMaskFieldAttribute ()
		{}

		public EnumMaskFieldAttribute (System.Type _targetType)
		{
			TargetType	= _targetType;
		}

		#endregion

		#region Methods

		public bool IsEnum ()
		{
			return TargetType.IsEnum;
		}

#if UNITY_EDITOR

		public System.Enum GetEnumValue (SerializedProperty _property)
		{
			return (System.Enum)System.Enum.ToObject(TargetType, _property.intValue);
		}

#endif
		#endregion
	}
}