using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace VoxelBusters.Utility
{
	public partial class ShaderUtility : AdvancedScriptableObject <ShaderUtility>
	{
		[Serializable]
		public class ShaderProperty
		{
			#region Properties
			
			[SerializeField]
			private			string						m_propertyName;
			public 			string						PropertyName
			{
				get
				{
					return m_propertyName;
				}
				
				private set
				{
					m_propertyName	= value;
				}
			}
			
			[SerializeField]
			private			eShaderPropertyType			m_propertyType;
			public			eShaderPropertyType 		PropertyType
			{
				get
				{
					return m_propertyType;
				}
				
				private set
				{
					m_propertyType	= value;
				}
			}
			
			#endregion
			
			#region Constructor
			
#if UNITY_EDITOR
			public ShaderProperty (string _name, ShaderUtil.ShaderPropertyType _propertyType)
			{
				PropertyName	= _name;
				
				switch (_propertyType)
				{
				case ShaderUtil.ShaderPropertyType.Color:
					PropertyType	= eShaderPropertyType.COLOR;
					break;
					
				case ShaderUtil.ShaderPropertyType.Vector:
					PropertyType	= eShaderPropertyType.VECTOR;
					break;
					
				case ShaderUtil.ShaderPropertyType.Float:
					PropertyType	= eShaderPropertyType.FLOAT;
					break;
					
				case ShaderUtil.ShaderPropertyType.Range:
					PropertyType	= eShaderPropertyType.RANGE;
					break;
					
				case ShaderUtil.ShaderPropertyType.TexEnv:
					PropertyType	= eShaderPropertyType.TEXTURE;
					break;
					
				default:
					throw new Exception("[ShaderUtility] Unknown type.");
				}
			}
#endif
			
			#endregion
		}
	}
}