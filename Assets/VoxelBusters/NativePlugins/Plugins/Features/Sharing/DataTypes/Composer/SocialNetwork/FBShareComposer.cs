using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	using Internal;
	
	public class FBShareComposer : SocialShareComposerBase 
	{
		#region Constructors
		
		public FBShareComposer () : base (eSocialServiceType.FB)
		{}
		
		#endregion
	}
}