using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Possible reasons when user finishes composing tweets.
	/// </summary>
	public enum eTwitterComposerResult
	{
		/// <summary>
		/// The composer is dismissed without sending the Tweet (i.e. the user selects Cancel, or the account is unavailable).
		/// </summary>
		CANCELLED,

		/// <summary>
		/// The composer is dismissed and the message is being sent in the background, after the user selects Done.
		/// </summary>
		DONE
	}
}