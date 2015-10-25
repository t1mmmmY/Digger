using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Data container for each contact detail.
	/// </summary>
	[System.Serializable]
	public class AddressBookContact
	{
		#region Static Fields

		private static Texture2D		defaultImage;

		#endregion

		#region Fields

		[SerializeField]
		private 		string 			m_firstName;
		[SerializeField]
		private 		string 			m_lastName;
		[SerializeField]
		private			Texture2D		m_image;
		private			string			m_imageDownloadError;
		[SerializeField]
		private 		string[]		m_phoneNumberList;
		[SerializeField]
		private 		string[]		m_emailIDList;

		#endregion

		#region Properties
		
		/// <summary>
		/// First Name of the contact.
		///	</summary>
		public string FirstName
		{
			get 
			{ 
				return m_firstName; 
			}

			protected set 
			{ 
				m_firstName = value; 
			}
		}

		/// <summary>
		/// Last Name of the contact.
		///	</summary>
		public string LastName
		{
			get 
			{ 
				return m_lastName; 
			}

			protected set 
			{ 
				m_lastName = value; 
			}
		}

		protected string ImagePath
		{
			get;
			set;
		}
	
		/// <summary>
		/// List of phone numbers in this contact.
		/// </summary>
		public string[] PhoneNumberList
		{
			get 
			{ 
				return m_phoneNumberList; 
			}

			protected set 
			{ 
				m_phoneNumberList = value; 
			}
		}

		/// <summary>
		/// List of Email ID's in this contact.
		/// </summary>
		public string[] EmailIDList
		{
			get 
			{ 
				return m_emailIDList; 
			}

			protected set 
			{ 
				m_emailIDList = value; 
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AddressBookContact"/> class.
		/// </summary>
		public AddressBookContact ()
		{
			this.FirstName				= null;
			this.LastName				= null;
			this.ImagePath				= null;
			this.m_image				= null;
			this.m_imageDownloadError	= null;
			this.PhoneNumberList		= new string[0];
			this.EmailIDList			= new string[0];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AddressBookContact"/> class by details from _source.
		/// </summary>
		/// <param name="_source">Source to copy from.</param>
		public AddressBookContact (AddressBookContact _source)
		{
			this.FirstName				= _source.FirstName;
			this.LastName				= _source.LastName;
			this.ImagePath				= _source.ImagePath;
			this.m_image				= _source.m_image;
			this.m_imageDownloadError	= _source.m_imageDownloadError;
			this.PhoneNumberList		= _source.PhoneNumberList;
			this.EmailIDList			= _source.EmailIDList;
		}

		#endregion

		#region Static Methods
		
		public static Texture2D GetDefaultImage ()
		{
			if (defaultImage == null)
				defaultImage	= Resources.Load(Constants.kDefaultContactImagePath) as Texture2D;

			return defaultImage;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Helper for getting Texture from image path.
		/// </summary>
		/// <param name="_onCompletion">Callback to be triggered after downloading the image.</param>
		public void GetImageAsync (DownloadTexture.Completion _onCompletion)
		{
			// Use cached information
			if (m_image != null || m_imageDownloadError != null)
			{
				_onCompletion(m_image, m_imageDownloadError);
				return;
			}
			else if (string.IsNullOrEmpty(ImagePath))
			{
				_onCompletion(GetDefaultImage(), null);
				return;
			}

			// Download image from given path
			URL 			_imagePathURL	= URL.FileURLWithPath(ImagePath);

			DownloadTexture _newDownload	= new DownloadTexture(_imagePathURL, true, true);
			_newDownload.OnCompletion		= (Texture2D _newTexture, string _error)=>{

				// Set properties
				m_image 				= _newTexture;
				m_imageDownloadError	= _error;

				// Send callback
				if (_onCompletion != null)
					_onCompletion(_newTexture, _error);
			};

			// Start download
			_newDownload.StartRequest();
		}

		/// <summary>
		/// String representation of <see cref="AddressBookContact"/>.
		/// </summary>
		public override string ToString ()
		{
			return string.Format("[AddressBookContact: FirstName={0}, LastName={1}]", FirstName, LastName);
		}

		#endregion
	}
}