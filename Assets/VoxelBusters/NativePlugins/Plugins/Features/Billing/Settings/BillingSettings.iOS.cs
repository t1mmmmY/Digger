using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public partial class BillingSettings 
	{
		/// <summary>
		/// Billing Settings specific to iOS platform.
		/// </summary>
		[System.Serializable]
		public class iOSSettings
		{
			#region Fields

			[SerializeField]
			private 		bool		m_supportsReceiptValidation		= true;
			[SerializeField]
			private 		string		m_validateUsingServerURL;

			#endregion

			#region Properties

			/// <summary>
			/// Gets or sets a value indicating whether <see cref="Billing"/> supports receipt validation.
			/// </summary>
			/// <value><c>true</c> if supports receipt validation; otherwise, <c>false</c>.</value>
			public bool SupportsReceiptValidation
			{
				get 
				{ 
					return m_supportsReceiptValidation; 
				}

				private set
				{
					m_supportsReceiptValidation	= value;
				}
			}
			
			/// <summary>
			/// Gets or sets the server URL used for <see cref="BillingTransaction"/> receipt validation.
			/// </summary>
			/// <value>The validate  <see cref="BillingTransaction"/> receipt using server URL.</value>
			/// <description>
			/// Incase if you dont want to use Apple itunes server for receipt validation, then specify the URL that will used for validating transaction receipts.
			/// </description>
			public string ValidateUsingServerURL
			{
				get 
				{ 
					return m_validateUsingServerURL; 
				}
				
				private set
				{
					m_validateUsingServerURL	= value;
				}
			}

			#endregion
		}
	}
}