using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Twitter Settings provides interface to configure TwitterKit.
	/// </summary>
	/// <description>
	/// To configure TwitterKit, you will need a consumer key and a consumer secret (collectively, a “Twitter app”). 
	/// Visit Twitter’s developer site (https://apps.twitter.com/) to create or review an application settings.
	/// </description>
	[System.Serializable]
	public class TwitterSettings
	{
		#region Properties

		[SerializeField]
		private string 		m_consumerKey;
		/// <summary>
		/// Gets or sets the consumer key.
		/// </summary>
		/// <value>The consumer key.</value>
		public string		ConsumerKey
		{
			get 
			{ 
				return m_consumerKey; 
			}

			set 
			{ 
				m_consumerKey	= value; 
			}
		}

		[SerializeField]
		private string 		m_consumerSecret;
		/// <summary>
		/// Gets or sets the consumer secret.
		/// </summary>
		/// <value>The consumer secret.</value>
		public string		ConsumerSecret
		{
			get 
			{ 
				return m_consumerSecret; 
			}

			set 
			{ 
				m_consumerSecret	= value; 
			}
		}

		#endregion

		#region Constructor

		public TwitterSettings ()
		{
			ConsumerKey		= string.Empty;
			ConsumerSecret	= string.Empty;
		}

		#endregion
	}
}
