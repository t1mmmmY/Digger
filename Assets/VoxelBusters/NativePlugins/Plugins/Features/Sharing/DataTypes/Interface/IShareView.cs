using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public interface IShareView 
	{
		#region Fields

		bool IsReadyToShowView
		{
			get;
		}

		#endregion
	}
}
