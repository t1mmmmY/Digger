using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Lists different sources available for picking an image.
	/// </summary>
	public enum eImageSource
	{
		/// <summary>Pick from gallery/album.</summary>
		ALBUM,
		/// <summary>Pick from camera.</summary>
		CAMERA,
		/// <summary>Pick from gallery/album and camera.</summary>
		BOTH
	}

	/// <summary>
	/// Lists different status for Pick image action.
	/// </summary>
	public enum ePickImageFinishReason
	{
		/// <summary>Selected image from specified <see cref="eImageSource"/> source.</summary>
		SELECTED,

		/// <summary>Cancelled picking image action.</summary>		
		CANCELLED,

		/// <summary>Failed picking image from specified <see cref="eImageSource"/> source.</summary>		
		FAILED
	}

	/// <summary>
	/// Lists different status for Pick video action.
	/// </summary>
	public enum ePickVideoFinishReason
	{
		/// <summary>Selected video from gallery.</summary>	
		SELECTED,

		/// <summary>Cancelled picking video from gallery.</summary>	
		CANCELLED,
	
		/// <summary>Picking video from gallery failed.</summary>	
		FAILED
	}

	/// <summary>
	///Lists different status for Play video action.
	/// </summary>
	public enum ePlayVideoFinishReason
	{
		/// <summary>Video playback ended. Played till end of the video.</summary>	
		PLAYBACK_ENDED,

		/// <summary>Error while playing video.</summary>
		PLAYBACK_ERROR,

		/// <summary>User exited without playing the complete video.</summary>
		USER_EXITED
	}
}