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
		#region Fields

		[SerializeField]
		private 	string		m_name;
		[SerializeField]
		private 	string		m_description;
		[SerializeField]
		private 	bool		m_isConsumable;
		[SerializeField]
		private 	float		m_price;
		[SerializeField]
		private 	string		m_iosProductId;
		[SerializeField]
		private 	string 		m_androidProductId;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of the product.
		/// </summary>
		/// <value>Name of the billing product. This is used only for reference or for display purpose only.</value>
		public string Name 
		{ 
			get	
			{ 
				return  m_name; 
			}

			protected set	
			{ 
				m_name	= value; 
			}
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		/// <value>Description of the billing product </value>
		public string Description 
		{ 
			get	
			{ 
				return  m_description; 
			} 

			protected set	
			{ 
				m_description	= value; 
			} 
		}

		/// <summary>
		/// Gets a value indicating whether this billing product is consumable.
		/// </summary>
		/// <value><c>true</c> If this billing product is consumable; otherwise, <c>false</c>.</value>
		public bool IsConsumable
		{ 
			get	
			{ 
				return  m_isConsumable; 
			}

			protected set 
			{ 
				m_isConsumable	= value; 
			} 
		}

		/// <summary>
		/// Gets the price.
		/// </summary>
		/// <value>Amount/value of this billing product in local currency.</value>
		public float Price 
		{ 
			get	
			{ 
				return  m_price; 
			}
			
			protected set 
			{ 
				m_price	= value; 
			}
		}

		/// <summary>
		/// Gets the localized price.
		/// </summary>
		/// <value>Contains localized price of this product with local currency information.</value>
		public string LocalizedPrice 
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the currency code.
		/// </summary>
		/// <value>The three-letter ISO 4217 currency code.</value>
		public string CurrencyCode
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the currency symbol.
		/// </summary>
		/// <value>The local currency symbol.</value>
		public string CurrencySymbol
		{
			get;
			protected set;
		}

		protected string IOSProductID
		{
			get
			{
				return m_iosProductId;
			}

			set
			{
				m_iosProductId = value;
			}
		}

		protected string AndroidProductID
		{
			get
			{
				return m_androidProductId;
			}

			set
			{
				m_androidProductId = value;
			}
		}

		/// <summary>
		/// Gets the product identifier based on active platform.
		/// </summary>
		/// <value>The string that identifies the product to the store.</value>
		public string ProductIdentifier
		{
			get 
			{
#if UNITY_ANDROID
				return m_androidProductId;
#else
				return m_iosProductId;
#endif
			}
		}

		#endregion

		#region Constructors

		protected BillingProduct ()
		{}

		protected BillingProduct (BillingProduct _product)
		{
			this.Name				= _product.Name;
			this.Description		= _product.Description;
			this.IsConsumable		= _product.IsConsumable;
			this.Price				= _product.Price;
			this.LocalizedPrice		= _product.LocalizedPrice;
			this.CurrencyCode		= _product.CurrencyCode;
			this.CurrencySymbol		= _product.CurrencySymbol;
			this.IOSProductID		= _product.IOSProductID;
			this.AndroidProductID	= _product.AndroidProductID;
		}

		#endregion

		#region Static Methods

		public static BillingProduct Create (string _name, bool _isConsumable, params PlatformID[] _platformIDs)
		{
			BillingProduct	_newProduct	= new BillingProduct();
			_newProduct.Name			= _name;
			_newProduct.IsConsumable	= _isConsumable;

			// Set product identifiers
			if (_platformIDs != null)
			{
				foreach (PlatformID _curID in _platformIDs)
				{
					if (_curID == null)
						continue;

					if (_curID.Platform == PlatformID.ePlatform.IOS)
						_newProduct.IOSProductID		= _curID.Value;
					else if (_curID.Platform == PlatformID.ePlatform.ANDROID)
						_newProduct.AndroidProductID	= _curID.Value;
				}
			}

			return _newProduct;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="BillingProduct"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="BillingProduct"/>.</returns>
		public override string ToString ()
		{
			return string.Format ("[BillingProduct: Name={0}, ProductIdentifier={1}, IsConsumable={2}, LocalizedPrice={3}]", 
			                      Name, ProductIdentifier, IsConsumable, LocalizedPrice);
		}

		#endregion

		#region Deprecated Methods

		[System.Obsolete("Instead use Create methods.")]
		public BillingProduct Copy ()
		{
			return null;
		}

		#endregion
	}
}