using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace VoxelBusters.Utility
{
	public partial class ShaderUtility : AdvancedScriptableObject <ShaderUtility>
	{
		[Serializable]
		public class AdvancedShader 
		{
			#region Properties

			[SerializeField]
			private			string						m_shaderName;
			public 			string						ShaderName
			{
				get
				{
					return m_shaderName;
				}
				
				private set
				{
					m_shaderName	= value;
				}
			}

			[SerializeField]
			private			List<ShaderProperty>		m_shaderPropertyList;
			public 			List<ShaderProperty>		ShaderPropertyList
			{
				get
				{
					return m_shaderPropertyList;
				}
				
				private set
				{
					m_shaderPropertyList	= value;
				}
			}

			#endregion

			#region Constructors

			private AdvancedShader ()
			{}

#if UNITY_EDITOR
			public AdvancedShader (Shader _shader)
			{
				if (_shader == null)
					throw new Exception("[ShaderUtility] Couldnt find shader with name");

				// Initialize
				ShaderName				= _shader.name;
				ShaderPropertyList		= new List<ShaderProperty>();
			
				// Iterate through properties
				int 	_propertyCount	= ShaderUtil.GetPropertyCount(_shader);

				for (int _iter = 0; _iter < _propertyCount; _iter++)
				{
					ShaderProperty	_property	= new ShaderProperty(ShaderUtil.GetPropertyName(_shader, _iter), ShaderUtil.GetPropertyType(_shader, _iter));

					// Add it to list
					ShaderPropertyList.Add(_property);
				}
			}
#endif

			#endregion

			#region Methods

			public Shader GetShader ()
			{
				return Shader.Find(ShaderName);
			}

			#endregion
		}
	}
}