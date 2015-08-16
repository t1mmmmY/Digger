using UnityEngine;
using System.Collections;
using System.Reflection;
using VoxelBusters.DesignPatterns;
using VoxelBusters.NativePlugins;
using VoxelBusters.NativePlugins.Internal;
using System.Collections.Generic;
using System;

[RequireComponent (typeof (PlatformBindingHelper))]
public class NPBinding : SingletonPattern <NPBinding>
{
	#region Properties

	/// <summary>
	/// Provides unique interface to access Address Book for all platforms.
	/// </summary>
	/// <value>The address book.</value>
	private AddressBook					m_addressBook;
	public static AddressBook 			AddressBook 	
	{ 
		get
		{
			if(!NPSettings.Application.SupportedFeatures.UsesAddressBook)
			{
				Debug.LogError("[NPBinding] Enable AddressBook in Supported Features under NPSettings/Application editor window.");
			}

			return Instance.m_addressBook;
		}

		private set
		{
			Instance.m_addressBook = value;
		} 
	}

	private	NetworkConnectivity			m_networkConnectivity;
	public static NetworkConnectivity 	NetworkConnectivity 	
	{ 
		get
		{
			if(!NPSettings.Application.SupportedFeatures.UsesNetworkConnectivity)
			{
				Debug.LogError("[NPBinding] Enable Network Connectivity in Supported Features under NPSettings/Application Settings editor window.");
			}
			return Instance.m_networkConnectivity;
		}
		
		private set
		{
			Instance.m_networkConnectivity = value;
		} 
	}

	private Sharing 					m_sharing; 		
	public static Sharing 				Sharing 		
	{ 
		get
		{
			if(!NPSettings.Application.SupportedFeatures.UsesSharing)
			{
				Debug.LogError("[NPBinding] Enable Sharing in Supported Features under NPSettings/Application Settings editor window.");
			}
			return Instance.m_sharing;
		}
		
		private set
		{
			Instance.m_sharing = value;
		} 
	}
	
	private	UI 							m_UI;
	public static UI					UI 		
	{ 
		get
		{
			return Instance.m_UI;
		}
		
		private set
		{
			Instance.m_UI = value;
		} 
	}
	
	private	Utility 					m_utility;
	public static Utility 				Utility 		
	{ 
		get
		{
			return Instance.m_utility;
		}
		
		private set
		{
			Instance.m_utility = value;
		}  
	}

#if !NATIVE_PLUGINS_LITE_VERSION
	private Billing 					m_billing;
	public static Billing 				Billing 			
	{ 
		get
		{
			if(!NPSettings.Application.SupportedFeatures.UsesBilling)
			{
				Debug.LogError("[NPBinding] Enable Billing in Supported Features under NPSettings/Application Settings editor window.");
			}
			return Instance.m_billing;
		}
		
		private set
		{
			Instance.m_billing = value;
		} 
	}
	private MediaLibrary 				m_mediaLibrary;
	public static MediaLibrary 			MediaLibrary 	
	{ 
		get
		{
			if(!NPSettings.Application.SupportedFeatures.UsesMediaLibrary)
			{
				Debug.LogError("[NPBinding] Enable MediaLibrary in Supported Features under NPSettings/Application Settings editor window.");
			}
			return Instance.m_mediaLibrary;
		}
		
		private set
		{
			Instance.m_mediaLibrary = value;
		} 
	}

	private Twitter 					m_twitter;
	public static Twitter 				Twitter 		
	{ 
		get
		{
			if(!NPSettings.Application.SupportedFeatures.UsesTwitter)
			{
				Debug.LogError("[NPBinding] Enable Twitter in Supported Features under NPSettings/Application Settings editor window.");
			}
			return Instance.m_twitter;
		}
		
		private set
		{
			Instance.m_twitter = value;
		} 
	}

	private NotificationService 		m_notificationService; 	
	public static NotificationService 	NotificationService 	
	{ 
		get
		{
			if(!NPSettings.Application.SupportedFeatures.UsesNotificationService)
			{
				Debug.LogError("[NPBinding] Enable Notification Service in Supported Features under NPSettings/Application Settings editor window.");
			}
			return Instance.m_notificationService;
		}
		
		private set
		{
			Instance.m_notificationService = value;
		}  
	}

	private WebViewNative 				m_webView;
	public static WebViewNative 		WebView 			
	{ 
		get
		{
			return Instance.m_webView;
		}
		
		private set
		{
			Instance.m_webView = value;
		}  
	}

#endif
	
	#endregion
	
	#region Overriden Methods
	
	protected override void Awake ()
	{
		base.Awake ();
		
		// Not interested in non singleton instance
		if (instance != this)
			return;

		// Create instances 
		m_addressBook				= AddComponentBasedOnPlatform<AddressBook>();
		m_networkConnectivity		= AddComponentBasedOnPlatform<NetworkConnectivity>();
		m_sharing					= AddComponentBasedOnPlatform<Sharing>();
		m_UI						= AddComponentBasedOnPlatform<UI>();
		m_utility					= AddComponentBasedOnPlatform<Utility>();

#if !NATIVE_PLUGINS_LITE_VERSION
		m_billing					= AddComponentBasedOnPlatform<Billing>();
		m_mediaLibrary				= AddComponentBasedOnPlatform<MediaLibrary>();
		m_notificationService		= AddComponentBasedOnPlatform<NotificationService>();
		m_twitter					= AddComponentBasedOnPlatform<Twitter>();
		m_webView					= AddComponentBasedOnPlatform<WebViewNative>();
#endif
	}
	
	#endregion
	
	#region Create Component Methods
	
	private T AddComponentBasedOnPlatform <T> () where T : MonoBehaviour
	{
		System.Type 	_basicType					= typeof(T);
		string 			_baseTypeName				= _basicType.ToString();
		
		// Check if we have free version specific class
		string 			_platformSpecificTypeName	= null;
		
#if UNITY_EDITOR
		_platformSpecificTypeName					= _baseTypeName + "Editor";	
#elif UNITY_IOS 
		_platformSpecificTypeName					= _baseTypeName + "IOS";	
#elif UNITY_ANDROID
		_platformSpecificTypeName					= _baseTypeName + "Android";
#endif
		if (!string.IsNullOrEmpty(_platformSpecificTypeName))
		{
			System.Type _platformSpecificClassType	= _basicType.Assembly.GetType(_platformSpecificTypeName, false);

			return CachedGameObject.AddComponent(_platformSpecificClassType) as T;
		}

		return CachedGameObject.AddComponent<T>();
	}

	#endregion
}