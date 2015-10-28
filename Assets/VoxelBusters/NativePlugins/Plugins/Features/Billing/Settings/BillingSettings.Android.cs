using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public partial class BillingSettings 
	{
		/// <summary>
		/// Billing Settings specific to Android platform.
		/// </summary>
		[System.Serializable]
		public class AndroidSettings
		{
			#region Fields

			[SerializeField]
			private 	string		m_publicKey	= null;

			#endregion

			#region Properties

			/// <summary>
			/// Public license key string value provided by Google Play for in-app billing. 
			///	Please check in Google Play Developer Console, select your application -> SERVICES & APIS -> LICENSING & IN-APP BILLING section.
			/// </summary>
			public string PublicKey
			{
				get 
				{ 
					return m_publicKey; 
				}

				private set
				{
					m_publicKey	= value;
				}
			}

			#endregion
		}
	}
}