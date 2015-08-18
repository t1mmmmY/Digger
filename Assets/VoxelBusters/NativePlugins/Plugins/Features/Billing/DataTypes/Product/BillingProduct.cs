using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// This provides unique access to a product which is purchasable. This encloses details for accessing different properties of a in-app product.
	/// </summary>
	[System.Serializable]
	public class BillingProduct
	{
		#region Properties

		[SerializeField]
		private string				m_name;

		/// <summary>
		/// Gets or sets the name of the product.
		/// </summary>
		/// <value>Name of the billing product. This is used only for reference or for display.</value>
		public string 				Name 
		{ 
			get	
			{ 
				return  m_name; 
			}

			set	
			{ 
				m_name	= value; 
			}
		}

		[SerializeField]
		private string				m_description;

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>Description of the billing product </value>
		public string 				Description 
		{ 
			get	
			{ 
				return  m_description; 
			} 

			set	
			{ 
				m_description	= value; 
			} 
		}

		[SerializeField]
		private bool				m_isConsumable;

		/// <summary>
		/// Gets or sets a value indicating whether this billing product is consumable.
		/// </summary>
		/// <value><c>true</c> if this billing product is consumable; otherwise, <c>false</c>.</value>
		public bool 				IsConsumable
		{ 
			get	
			{ 
				return  m_isConsumable; 
			}

			set 
			{ 
				m_isConsumable	= value; 
			} 
		}

		[SerializeField]
		private float				m_price;

		/// <summary>
		/// Gets or sets the price.
		/// </summary>
		/// <value>Amount/value of this billing product in local currency.</value>
		public float 				Price 
		{ 
			get	
			{ 
				return  m_price; 
			}
			
			set 
			{ 
				m_price	= value; 
			}
		}

		/// <summary>
		/// Gets or sets the localized price.
		/// </summary>
		/// <value>Contains localized price of this product with local currency information.</value>
		public string 				LocalizedPrice 
		{ 
			get; 
			internal set; 
		}

		// Related to store details
		#pragma warning disable
		[SerializeField]
		private string				m_iosProductId;
		[SerializeField]
		private string 				m_androidProductId;
		#pragma warning restore

		/// <summary>
		/// Gets or sets the product identifier.
		/// </summary>
		/// <value>Hold identifier to uniquely identity this billing product.</value>
		public string				ProductIdentifier
		{
			get 
			{
#if UNITY_ANDROID
				return m_androidProductId;
#else
				return m_iosProductId;
#endif
			}

			protected set 
			{
#if UNITY_ANDROID
				m_androidProductId	= value;
#else
				m_iosProductId		= value;
#endif
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Copy this instance.
		/// </summary>
		public BillingProduct Copy ()
		{
			BillingProduct _productClone		= new BillingProduct();
			_productClone.Name					= this.Name;
			_productClone.Description			= this.Description;
			_productClone.IsConsumable			= this.IsConsumable;
			_productClone.Price					= this.Price;
			_productClone.LocalizedPrice		= this.LocalizedPrice;
			_productClone.m_iosProductId		= this.m_iosProductId;
			_productClone.m_androidProductId	= this.m_androidProductId;

			return _productClone;
		}
		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="BillingProduct"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="BillingProduct"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[BillingProduct: Name={0}, Description={1}, IsConsumable={2}, Price={3}, LocalizedPrice={4}, ProductIdentifier={5}]", 
			                      Name, Description, IsConsumable, Price, LocalizedPrice, ProductIdentifier);
		}

		#endregion
	}
}