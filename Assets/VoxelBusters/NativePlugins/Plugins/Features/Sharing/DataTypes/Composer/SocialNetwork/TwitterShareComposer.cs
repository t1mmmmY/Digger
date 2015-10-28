using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public class TwitterShareComposer : SocialShareComposerBase 
	{
		#region Constructors

		public TwitterShareComposer () : base (eSocialServiceType.TWITTER)
		{}

		#endregion
	}
}