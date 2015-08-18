using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	[System.Serializable]
	public class SocialNetworkSettings
	{
		#region Properties

		[SerializeField]
		private 				TwitterSettings			m_twitterSettings;
		public					TwitterSettings			TwitterSettings
		{
			get
			{
				return m_twitterSettings;
			}
		}

		#endregion

		#region Constructors

		public SocialNetworkSettings ()
		{
			m_twitterSettings	= new TwitterSettings();
		}

		#endregion
	}
}
