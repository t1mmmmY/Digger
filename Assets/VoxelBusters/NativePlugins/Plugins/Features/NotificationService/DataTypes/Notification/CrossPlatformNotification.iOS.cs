using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	public partial class CrossPlatformNotification 
	{
		/// <summary>
		/// Notification properties specifically used only in iOS platform.
		/// </summary>
		public class iOSSpecificProperties
		{
			#region Properties

			/// <summary>
			/// Gets or sets the title of the action button or slider.
			/// </summary>
			/// <value>The alert action.</value>
			public string 			AlertAction
			{
				get; 
				set;
			}

			/// <summary>
			/// Gets or sets a value indicating whether the alert action is visible or not.
			/// </summary>
			/// <value><c>true</c> if this instance has action; otherwise, <c>false</c>.</value>
			public bool 			HasAction
			{
				get; 
				set;
			}

			/// <summary>
			/// Gets or sets the number to display as the application's icon badge.
			/// </summary>
			/// <value>The badge count.</value>
			public int	 			BadgeCount
			{
				get; 
				set;
			}

			/// <summary>
			/// Gets or sets the name of the sound file to play when an alert is displayed.
			/// </summary>
			/// <value>The name of the sound.</value>
			public string 			SoundName
			{
				get; 
				set;
			}

			/// <summary>
			/// Gets or sets the image used as the launch image when the user taps the action button..
			/// </summary>
			/// <value>The launch image.</value>
			public string 			LaunchImage
			{
				get; 
				set;
			}
		
			#endregion

			#region Constants

			private const string 	kAlertActionKey		= "alert-action";
			private const string 	kHasActionKey		= "has-action";
			private const string 	kBadgeCountKey		= "badge-count";
			private const string 	kSoundNameKey		= "sound-name";
			private const string 	kLaunchImageKey		= "launch-image";

			#endregion

			#region Constructors

			public iOSSpecificProperties ()
			{
				AlertAction	= null;
				HasAction	= true;
				BadgeCount	= 0;
				SoundName	= null;
				LaunchImage	= null;
			}

			internal iOSSpecificProperties (IDictionary _jsonDict)
			{
				AlertAction	= _jsonDict.GetIfAvailable<string>(kAlertActionKey);
				HasAction	= _jsonDict.GetIfAvailable<bool>(kHasActionKey);
				BadgeCount	= _jsonDict.GetIfAvailable<int>(kBadgeCountKey);
				SoundName	= _jsonDict.GetIfAvailable<string>(kSoundNameKey);
				LaunchImage	= _jsonDict.GetIfAvailable<string>(kLaunchImageKey);
			}

			#endregion

			#region Methods

			internal IDictionary JSONObject ()
			{
				Dictionary<string, object> _jsonDict	= new Dictionary<string, object>();
				_jsonDict[kAlertActionKey]				= AlertAction;
				_jsonDict[kHasActionKey]				= HasAction;
				_jsonDict[kBadgeCountKey]				= BadgeCount;
				_jsonDict[kSoundNameKey]				= SoundName;
				_jsonDict[kLaunchImageKey]				= LaunchImage;

				return _jsonDict;
			}

			/// <summary>
			/// String representation of <see cref="CrossPlatformNotification+iOSSpecificProperties"/>.
			/// </summary>
			public override string ToString ()
			{
				return string.Format ("[iOSSpecificProperties: AlertAction={0}, HasAction={1}, BadgeCount={2}, SoundName={3}, LaunchImage={4}]", 
				                      AlertAction, HasAction, BadgeCount, SoundName, LaunchImage);
			}

			#endregion
		}
	}
}
