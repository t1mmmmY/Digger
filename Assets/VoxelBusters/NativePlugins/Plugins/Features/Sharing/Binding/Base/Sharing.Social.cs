using UnityEngine;
using System.Collections;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class Sharing : MonoBehaviour 
	{
		#region Methods

		public virtual bool IsFBShareServiceAvailable ()
		{
			bool _isAvailable	= false;
			Console.Log(Constants.kDebugTag, "[Sharing:FB] Is service available=" + _isAvailable);
			
			return _isAvailable;
		}
		
		public virtual bool IsTwitterShareServiceAvailable ()
		{
			bool _isAvailable	= false;
			Console.Log(Constants.kDebugTag, "[Sharing:Twitter] Is service available=" + _isAvailable);
			
			return _isAvailable;
		}
		
		protected virtual void ShowFBShareComposer (FBShareComposer _composer)
		{
			if (!IsFBShareServiceAvailable())
			{
				FBShareFinished(FBShareFailedResponse());
				return;
			}
		}
		
		protected virtual void ShowTwitterShareComposer (TwitterShareComposer _composer)
		{
			if (!IsTwitterShareServiceAvailable())
			{
				TwitterShareFinished(TwitterShareFailedResponse());
				return;
			}
		}
		
		#endregion
	}
}