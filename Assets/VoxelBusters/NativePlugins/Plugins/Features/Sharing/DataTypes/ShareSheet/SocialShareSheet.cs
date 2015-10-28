using UnityEngine;
using System.Collections;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;
	
	public class SocialShareSheet : ShareSheet
	{
		#region Properties

		public override eShareOptions[] ExcludedShareOptions
		{
			get
			{
				return base.ExcludedShareOptions;
			}

			set
			{
				Console.LogWarning(Constants.kDebugTag, "[Sharing] Not allowed.");
			}
		}

		#endregion

		#region Constructors
		
		public SocialShareSheet () 
		{
			base.ExcludedShareOptions	= new eShareOptions[]{
				eShareOptions.WHATSAPP,
				eShareOptions.MAIL,
				eShareOptions.MESSAGE
			};
		}
		
		#endregion
	}
}