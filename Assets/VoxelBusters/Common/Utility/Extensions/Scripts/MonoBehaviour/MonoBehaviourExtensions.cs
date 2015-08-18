using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

namespace VoxelBusters.Utility
{
	public static class MonoBehaviourExtensions
	{
		#region Properties

		private static bool		isPaused	= false;
		private static float	timeScale	= 1f;

		#endregion

		#region Pause Resume Methods

		public static void PauseUnity (this MonoBehaviour _monoTarget)
		{
			// Pause only if its not in paused state
			if (!isPaused)
			{
				Debug.LogWarning("[MonoBehaviourExtensions] Paused");
				isPaused		= true;

				// Cache timescale, later used for resetting
				timeScale		= Time.timeScale;
				Time.timeScale	= 0f;
			}
		}

		public static void ResumeUnity (this MonoBehaviour _monoTarget)
		{
			// Resume only if paused
			if (isPaused)
			{
				Debug.LogWarning("[MonoBehaviourExtensions] Resumed");
				isPaused		= false;
				
				// Reset timescale
				Time.timeScale	= timeScale;
			}
		}

		#endregion

		#region Send Message

		public static void InvokeMethod (this MonoBehaviour _monoTarget, string _method, object _value = null)
		{
			// Get value type and arg type
			object[] _valueList	= null;
			Type[] _argTypeList	= null;

			if (_value != null)
			{
				_valueList		= new object[1] { _value };
				_argTypeList	= new Type[1] { _value.GetType() };
			}

			InvokeMethod(_monoTarget.GetType(), _monoTarget, _method, _argTypeList, _valueList);
		}

		private static void InvokeMethod (Type _monoType,  MonoBehaviour _monoTarget, string _method, Type[] _argTypeList, object[] _valueList)
		{
			BindingFlags _bindingAttr	= BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.OptionalParamBinding;
			MethodInfo _methodInfo		= null;

			if (_valueList != null)
				_methodInfo = _monoType.GetMethod(_method, _bindingAttr, null, _argTypeList, null);
			else
				_methodInfo	= _monoType.GetMethod(_method, _bindingAttr);

			// Found a matching method
			if (_methodInfo != null)
			{
				_methodInfo.Invoke(_monoTarget, _valueList);
			}
			// Failed to find a matching method, so search for it in base class
			else if (_monoType.BaseType != null)
			{
				InvokeMethod(_monoType.BaseType, _monoTarget, _method, _argTypeList, _valueList);
			}
			else
			{
				throw new MissingMethodException();
			}
		}

		#endregion
	}
}