using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Possible reasons when user finishes composing tweets.
	/// </summary>
	public enum eToastMessageLength
	{
		/// <summary>
		/// Show the toast message for a short period of time.
		/// </summary>
		SHORT,
		
		/// <summary>
		/// Show the toast message for a long period of time.
		/// </summary>
		LONG
	}
}