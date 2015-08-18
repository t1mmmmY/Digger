using UnityEngine;
using System.Collections;

namespace VoxelBusters.Utility
{
	public class ExecuteOnValueChangeAttribute : PropertyAttribute 
	{
		#region Properties

		public string	Function { get; private set; }

		#endregion

		#region Constructor

		public ExecuteOnValueChangeAttribute (string _function)
		{
			Function	= _function;
		}

		#endregion
	}
}
