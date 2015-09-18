using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Notification Service Settings provides interface to configure properties related to notification.
	/// </summary>
	[System.Serializable]
	public class NotificationServiceSettings
	{
		#region iOS Settings

		/// <summary>
		/// Notification Service Settings specific to iOS platform.
		/// </summary>
		[System.Serializable]
		public class iOSSettings 
		{
			[SerializeField]
			[Tooltip ("Specify the key used to identify user data within notification payload")]//TODO
			private string 				m_userInfoKey;
			/// <summary>
			/// Gets or sets the key used to access user data within notification payload.
			/// </summary>
			/// <value>The user info key.</value>
			public string  				UserInfoKey
			{
				get 
				{ 
					return m_userInfoKey; 
				}
				
				set
				{
					m_userInfoKey = value;
				}
			}

			public iOSSettings ()
			{
				UserInfoKey	= "user_info";
			}
		}

		#endregion

		#region Android Settings

		/// <summary>
		/// Notification Service Settings specific to Android platform.
		/// </summary>
		[System.Serializable]
		public class AndroidSettings 
		{
			[SerializeField]
			[Tooltip ("List the sender IDs to register GCM with.")]
			private string[]	 		m_senderIDs;

			/// <summary>
			/// Gets or sets the sender identifier list.
			/// </summary>
			/// <value>	Contains list of sender ids.
			///	\note Sender ID list is required to receive Remote Notifications.
			///	Please check in Google Play Developer Console, select your application -> SERVICES & APIS -> GOOGLE CLOUD MESSAGING section.
			///</value>

			public string[] 			SenderIDList
			{
				get
				{
					return m_senderIDs;
				}
				
				set
				{
					m_senderIDs = value;
				}
			}
			
			private bool	 		m_needsBigStyle	= false; 
			internal bool 			NeedsBigStyle //Not Exposed currenltly. Becuase of issues on Lollipop
			{
				get
				{
					return m_needsBigStyle;
				}
				
				set
				{
					m_needsBigStyle = value;
				}
			}

			[SerializeField, ExecuteOnValueChange("OnSmallNotificationIconChanged")]
			[Tooltip ("Used for post Android L Devices. Changes because of material design adaptation.")]
			private Texture2D m_whiteSmallIcon;
			/// <summary>
			/// This texture will be used for showing small icon in notification.
			/// </summary>
			/// <value> Android L devices will use this as icon in notifications.</value>
			public Texture2D  				WhiteSmallIcon
			{
				get 
				{ 
					return m_whiteSmallIcon; 
				}
				
				set
				{
					m_whiteSmallIcon = value;
				}
			}
			
			[SerializeField, ExecuteOnValueChange("OnSmallNotificationIconChanged")]
			[Tooltip ("Coloured icon used for pre Android L Devices.")]
			private Texture2D m_colouredSmallIcon;
			/// <summary>
			/// This texture will be used for showing small icon in notification.
			/// </summary>
			/// <value> Android L devices will use this as icon in notifications.</value>
			public Texture2D  				ColouredSmallIcon
			{
				get 
				{ 
					return m_colouredSmallIcon; 
				}
				
				set
				{
					m_colouredSmallIcon = value;
				}
			}

			[Header("Remote Notification Keys")]
			[SerializeField]
			[Tooltip ("Custom ticker text key used in remote notifications.")]
			private string 				m_tickerTextKey = "ticker_text";
			/// <summary>
			/// Gets or sets the ticker text key.
			/// </summary>
			/// <value>Custom ticker text key used for remote notifications.</value>
			public string  				TickerTextKey
			{
				get 
				{ 
					return m_tickerTextKey; 
				}
				
				set
				{
					m_tickerTextKey = value;
				}
			}

			[SerializeField]
			[Tooltip ("Custom content title key used in remote notifications.")]
			private string 				m_contentTitleKey = "content_title";
			/// <summary>
			/// Gets or sets the content title key.
			/// </summary>
			/// <value>Custom content title key used for remote notifications.</value>
			public string  				ContentTitleKey
			{
				get 
				{ 
					return m_contentTitleKey; 
				}
				
				set
				{
					m_contentTitleKey = value;
				}
			}

			[SerializeField]
			[Tooltip ("Custom content text key used in remote notifications.")]
			private string 				m_contentTextKey = "content_text";

			/// <summary>
			/// Gets or sets the content text key.
			/// </summary>
			/// <value>Custom content text key used in remote notifications.</value>
			public string  				ContentTextKey
			{
				get 
				{ 
					return m_contentTextKey; 
				}
				
				set
				{
					m_contentTextKey = value;
				}
			}

			[SerializeField]
			[Tooltip ("User info key used in remote notifications.")]
			private string 				m_userInfoKey = "user_info";

			/// <summary>
			/// Gets or sets the user info key.
			/// </summary>
			/// <value>User info key where user can add custom data and retrieve back with UserInfo variable of CrossPlatformNotification.</value>
			public string  				UserInfoKey
			{
				get 
				{ 
					return m_userInfoKey; 
				}
				
				set
				{
					m_userInfoKey = value;
				}
			}

			[SerializeField]
			[Tooltip ("Tag key used in notifications for  overwriting existing notifications in notification bar if specified uniquely.")]
			private string 				m_tagKey = "tag";
			/// <summary>
			/// Gets or sets the tag key.
			/// </summary>
			/// <value>The Tag key used in remote notifcations. Specifying uniquely can avoid overwriting previous notification displayed.</value>
			public string  				TagKey
			{
				get 
				{ 
					return m_tagKey; 
				}
				
				set
				{
					m_tagKey = value;
				}
			}
		}

		#endregion

		#region Properties

		[SerializeField]
		private iOSSettings				m_iOS;
		/// <summary>
		/// Gets or sets the Notification Service Settings specific to iOS platform.
		/// </summary>
		/// <value>The i O.</value>
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
		/// Gets or sets the Notification Service Settings specific to Android platform.
		/// </summary>
		/// <value>The android.</value>
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

		#region Constructor

		public NotificationServiceSettings ()
		{
			iOS							= new iOSSettings();
			Android						= new AndroidSettings();
		}

		#endregion
	}
}