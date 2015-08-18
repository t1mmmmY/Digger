using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Billing Settings provides interface to configure properties related to native billing.
	/// </summary>
	[System.Serializable]
	public class BillingSettings 
	{
		#region Android Settings

		/// <summary>
		/// Billing Settings specific to Android platform.
		/// </summary>
		[System.Serializable]
		public class AndroidSettings
		{
			/// <summary>
			/// Public license key string value provided by Google Play for in-app billing. 
			///	Please check in Google Play Developer Console, select your application -> SERVICES & APIS -> LICENSING & IN-APP BILLING section.
			/// </summary>
			[SerializeField]
			private string				m_publicKey	= null;
			public string 				PublicKey
			{
				get 
				{ 
					return m_publicKey; 
				}

				set
				{
					m_publicKey	= value;
				}
			}
		}

		#endregion

		#region iOS Settings

		/// <summary>
		/// Billing Settings specific to iOS platform.
		/// </summary>
		[System.Serializable]
		public class iOSSettings
		{
			[SerializeField]
			private bool				m_supportsReceiptValidation	= true;
			/// <summary>
			/// Gets or sets a value indicating whether <see cref="Billing"/> supports receipt validation.
			/// </summary>
			/// <value><c>true</c> if supports receipt validation; otherwise, <c>false</c>.</value>
			public bool 				SupportsReceiptValidation
			{
				get 
				{ 
					return m_supportsReceiptValidation; 
				}

				set
				{
					m_supportsReceiptValidation	= value;
				}
			}
			
			[SerializeField]
			private bool				m_validateUsingAppleServer	= true;
			/// <summary>
			/// Gets or sets a value indicating whether <see cref="BillingTransaction"/> is validated using apple server or custom server.
			/// </summary>
			/// <value><c>true</c> if validate using apple server; otherwise, <c>false</c>.</value>
			public bool 				ValidateUsingAppleServer
			{
				get 
				{ 
					return m_validateUsingAppleServer; 
				}
				
				set
				{
					m_validateUsingAppleServer	= value;
				}
			}
			
			[SerializeField]
			private string				m_validateUsingServerURL;
			/// <summary>
			/// Gets or sets the server URL used for <see cref="BillingTransaction"/> receipt validation.
			/// </summary>
			/// <value>The validate  <see cref="BillingTransaction"/> receipt using server URL.</value>
			/// <description>
			/// Incase if you dont want to use Apple itunes server for receipt validation, then specify the URL that will used for validating transaction receipts.
			/// </description>
			public string				ValidateUsingServerURL
			{
				get 
				{ 
					return m_validateUsingServerURL; 
				}
				
				set
				{
					m_validateUsingServerURL	= value;
				}
			}
		}

		#endregion
		
		#region Properties

		[SerializeField] 
		private List<BillingProduct>	m_products;
		/// <summary>
		/// Gets or sets the billing products.
		/// </summary>
		/// <value>The billing products.</value>
		public List<BillingProduct> 	Products
		{
			get 
			{ 
				return m_products; 
			}

			set 
			{ 
				m_products	= value; 
			}
		}

		[SerializeField]
		private iOSSettings				m_iOS;
		/// <summary>
		/// Gets or sets the Billing Settings specific to iOS platform.
		/// </summary>
		/// <value>The Billing Settings specific to iOS platform.</value>
		public	iOSSettings				iOS
		{
			get 
			{ 
				return m_iOS; 
			}

			set 
			{ 
				m_iOS = value; 
			}
		}

		[SerializeField]
		private AndroidSettings			m_android;
		/// <summary>
		/// Gets or sets the Billing Settings specific to Android platform.
		/// </summary>
		/// <value>The Billing Settings specific to Android platform.</value>
		public	AndroidSettings			Android
		{
			get 
			{ 
				return m_android; 
			}

		 	set 
			{ 
				m_android = value; 
			}
		}

		#endregion

		#region Constructors

		public BillingSettings ()
		{
			Products	= new List<BillingProduct>();
			iOS			= new BillingSettings.iOSSettings();
			Android		= new BillingSettings.AndroidSettings();
		}

		#endregion
	}
}