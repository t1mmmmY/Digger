using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

namespace VoxelBusters.Utility
{
#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public partial class ShaderUtility : AdvancedScriptableObject <ShaderUtility>
	{
		#region Properties

		[SerializeField]
		private				List<AdvancedShader>		m_shaderList;
		public				List<AdvancedShader>		ShaderList
		{
			get
			{
				return m_shaderList;
			}

			private set
			{
				m_shaderList	= value;
			}
		}

		#endregion

		#region Constructors

#if AUTO_RELOAD_SHADER_UTILITY
		static ShaderUtility ()
		{
			EditorInvoke.Invoke(()=>{
#pragma warning disable
			ShaderUtility	_instance	= ShaderUtility.Instance;
#pragma warning restore 
			}, 1f);
		}
#endif

		#endregion

		#region Methods

#if AUTO_RELOAD_SHADER_UTILITY
		protected override void OnEnable ()
		{
			base.OnEnable ();
			
			if (m_shaderList == null)
				m_shaderList	= new List<AdvancedShader>();

			// Reload shader information
			ReloadShaderUtility();
		}
#endif

		public AdvancedShader GetShader (Material _material)
		{
			return GetShader(_material.shader.name);
		}

		public AdvancedShader GetShader (string _shaderName)
		{
			for (int _iter = 0; _iter < m_shaderList.Count; _iter++)
			{
				AdvancedShader	_curShader	= m_shaderList[_iter];

				if (_shaderName.Equals(_curShader.ShaderName))
					return _curShader;
			}

			return null;
		}
		
#if !UNITY_EDITOR
		public void ReloadShaderUtility ()
		{}
#else
		public void ReloadShaderUtility ()
		{
			if (Application.isPlaying)
				return;
			
			// Clear existing entries
			m_shaderList.Clear();
			
			// Find inbuilt shaders 
			FindAllBuiltInShaders(ref m_shaderList);
			
			// Find custom shaders
			FindAllCustomShaders(ref m_shaderList);

			// Forcing unity to serialize
			EditorUtility.SetDirty(this);
		}

		private void  FindAllBuiltInShaders (ref List<AdvancedShader> _shaderList)
		{
			FindAllBuiltInShaders("Resources/unity_builtin_extra", 		ref _shaderList);
			FindAllBuiltInShaders("Library/unity default resources", 	ref _shaderList);
		}

		private void  FindAllBuiltInShaders (string _builtInFolderPath, ref List<AdvancedShader> _shaderList)
		{
			UnityEngine.Object[] 	_assetList  	= AssetDatabase.LoadAllAssetsAtPath(_builtInFolderPath) as UnityEngine.Object[];
			int						_assetCount		= _assetList.Length;

			for (int _iter = 0; _iter < _assetCount; _iter++)
			{
				UnityEngine.Object	_curAsset		= _assetList[_iter];

				if (_curAsset.GetType() == typeof(Shader))
					_shaderList.Add(new AdvancedShader(_curAsset as Shader));
			}
		}

		private void  FindAllCustomShaders (ref List<AdvancedShader> _shaderList)
		{
			string[] 		_assetPaths		= AssetDatabase.GetAllAssetPaths();
			int				_pathCount		= _assetPaths.Length;
				
			for (int _iter = 0; _iter < _pathCount; _iter++)
			{
				string		_curAssetPath	= _assetPaths[_iter];
				string		_fileExtension	= Path.GetExtension(_curAssetPath);

				if (!_fileExtension.Equals(".shader"))
					continue;

				Shader		_shader			= AssetDatabase.LoadAssetAtPath(_curAssetPath, typeof(Shader)) as Shader;

				if (_shader == null)
					continue;

				// Add this shader
				_shaderList.Add(new AdvancedShader(_shader));
			}
		}
#endif

		#endregion
	}
}