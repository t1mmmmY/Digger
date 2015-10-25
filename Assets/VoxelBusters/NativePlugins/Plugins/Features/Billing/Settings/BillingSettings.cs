using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Billing Settings provides interface to configure properties related to native billing.
	/// </summary>
	[System.Serializable]
	public partial class BillingSettings 
	{
		#region Fields

		[SerializeField] 
		private 		List<BillingProduct>	m_products;
		[SerializeField]
		private 		iOSSettings				m_iOS;
		[SerializeField]
		private 		AndroidSettings			m_android;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the billing products.
		/// </summary>
		/// <value>The billing products.</value>
		public BillingProduct[] Products
		{
			get 
			{ 
				return m_products.ToArray(); 
			}

			private set 
			{ 
				m_products	= new List<BillingProduct>(value); 
			}
		}

		/// <summary>
		/// Gets or sets the Billing Settings specific to iOS platform.
		/// </summary>
		/// <value>The Billing Settings specific to iOS platform.</value>
		public iOSSettings iOS
		{
			get 
			{ 
				return m_iOS; 
			}

			private set 
			{ 
				m_iOS = value; 
			}
		}

		/// <summary>
		/// Gets or sets the Billing Settings specific to Android platform.
		/// </summary>
		/// <value>The Billing Settings specific to Android platform.</value>
		public AndroidSettings Android
		{
			get 
			{ 
				return m_android; 
			}

			private set 
			{ 
				m_android = value; 
			}
		}

		#endregion

		#region Constructors

		public BillingSettings ()
		{
			Products	= new BillingProduct[0];
			iOS			= new BillingSettings.iOSSettings();
			Android		= new BillingSettings.AndroidSettings();
		}

		#endregion
	}
}