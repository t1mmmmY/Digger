using UnityEngine;
using System.Collections;
using System;
using System.IO;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Sharing gives access to different forms of sharing. Can share text or image with Social network, Mail, SMS, WhatsApp.
	/// </summary>
	public partial class Sharing : MonoBehaviour 
	{
		#region Constants
		
		protected const string kSharingFeatureDeprecatedMethodInfo	= "This method is deprecated. Instead of this use ShowView.";

		#endregion

		#region Methods

		public void ShowView (IShareView _shareView, SharingCompletion _onCompletion)
		{
			StartCoroutine(ShowViewCoroutine(_shareView, _onCompletion));
		}

		private IEnumerator ShowViewCoroutine (IShareView _shareView, SharingCompletion _onCompletion)
		{
			while (!_shareView.IsReadyToShowView)
				yield return null;

			// Pause unity player
			this.PauseUnity();
			
			// Cache callback
			OnSharingFinished	= _onCompletion;

			if (_shareView is MailShareComposer)
			{
				ShowMailShareComposer((MailShareComposer)_shareView);
			}
			else if (_shareView is MessageShareComposer)
			{
				ShowMessageShareComposer((MessageShareComposer)_shareView);
			}
			else if (_shareView is WhatsAppShareComposer)
			{
				ShowWhatsAppShareComposer((WhatsAppShareComposer)_shareView);
			}
			else if (_shareView is FBShareComposer)
			{
				ShowFBShareComposer((FBShareComposer)_shareView);
			}
			else if (_shareView is TwitterShareComposer)
			{
				ShowTwitterShareComposer((TwitterShareComposer)_shareView);
			}
			else 
			{
				ShowShareSheet((ShareSheet)_shareView);
			}
		}

		protected virtual void ShowShareSheet (ShareSheet _shareSheet)
		{}

		#endregion
	
		#region Deprecated Social Network Methods

		private eShareOptions[] m_socialNetworkExcludedList	= new eShareOptions[] { eShareOptions.MESSAGE,
			eShareOptions.MAIL,
			eShareOptions.WHATSAPP
		};
		
		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareTextMessageOnSocialNetwork (string _message, SharingCompletion _onCompletion) 
		{
			ShareMessage(_message, m_socialNetworkExcludedList, _onCompletion);
		}
		
		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareURLOnSocialNetwork (string _message, string _URLString, SharingCompletion _onCompletion) 
		{
			ShareURL(_message, _URLString, m_socialNetworkExcludedList, _onCompletion);
		}
		
		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareScreenShotOnSocialNetwork (string _message, SharingCompletion _onCompletion)
		{
			ShareScreenShot(_message, m_socialNetworkExcludedList, _onCompletion);
		}
		
		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareImageOnSocialNetwork (string _message, Texture2D _texture, SharingCompletion _onCompletion)
		{
			ShareImage(_message, _texture, m_socialNetworkExcludedList, _onCompletion);
		}
		
		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareImageOnSocialNetwork (string _message, string _imagePath, SharingCompletion _onCompletion)
		{
			ShareImageAtPath(_message, _imagePath, m_socialNetworkExcludedList, _onCompletion);
		}
		
		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareImageOnSocialNetwork (string _message, byte[] _imageByteArray, SharingCompletion _onCompletion)
		{
			Share(_message, null, _imageByteArray, m_socialNetworkExcludedList, _onCompletion);
		}
		
		#endregion

		#region Deprecated Share Methods

		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareMessage (string _message, eShareOptions[] _excludedOptions, SharingCompletion _onCompletion)
		{
			Share(_message, null, null, _excludedOptions, _onCompletion);
		}

		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareURL (string _message, string _URLString, eShareOptions[] _excludedOptions, SharingCompletion _onCompletion)
		{
			if (string.IsNullOrEmpty(_URLString))
			{
				DebugPRO.Console.LogWarning(Constants.kDebugTag, "[Sharing] ShareURL, URL is null/empty");
			}
			
			Share(_message, _URLString, null, _excludedOptions, _onCompletion);
		}

		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareScreenShot (string _message, eShareOptions[] _excludedOptions, SharingCompletion _onCompletion)
		{
			// First capture screenshot
			StartCoroutine(TextureExtensions.TakeScreenshot((_texture)=>{
				
				// Share image
				ShareImage(_message, _texture, _excludedOptions, _onCompletion);
			}));
		}

		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareImage (string _message, Texture2D _texture, eShareOptions[] _excludedOptions, SharingCompletion _onCompletion)
		{
			byte[] _imageByteArray	= null;
			
			if (_texture != null)
			{
				_imageByteArray	= _texture.EncodeToPNG();
			}
			else
			{
				DebugPRO.Console.LogWarning(Constants.kDebugTag, "[Sharing] ShareImage, texure is null");
			}
			
			Share(_message, null, _imageByteArray, _excludedOptions, _onCompletion);
		}

		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareImageAtPath (string _message, string _imagePath, eShareOptions[] _excludedOptions, SharingCompletion _onCompletion)
		{
			if (!File.Exists(_imagePath))
			{
				DebugPRO.Console.LogWarning(Constants.kDebugTag, "[Sharing] ShareImageAtPath, path is invalid");
				ShareImage(_message, null, _excludedOptions, _onCompletion);
				return;
			}
			
			// Download image from given path
			URL _imagePathURL				= URL.FileURLWithPath(_imagePath);
			DownloadTexture _newDownload	= new DownloadTexture(_imagePathURL, true, false);
			_newDownload.OnCompletion		= (Texture2D _texture, string _error)=>{
				
				// Download went wrong
				if (!string.IsNullOrEmpty(_error))
				{
					DebugPRO.Console.LogWarning(Constants.kDebugTag, "[Sharing] ShareImageAtPath, failed to download texture. Error=" + _error);
				}
				
				ShareImage(_message, _texture, _excludedOptions, _onCompletion);
			};
			
			// Start download
			_newDownload.StartRequest();
		}

		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void Share (string _message, string _URLString, byte[] _imageByteArray, eShareOptions[] _excludedOptions, SharingCompletion _onCompletion)
		{
			string _excludedOptionsJsonString	= null;
			
			if (_excludedOptions != null)
				_excludedOptionsJsonString	= _excludedOptions.ToJSON();
			
			Share(_message, _URLString, _imageByteArray, _excludedOptionsJsonString, _onCompletion);
		}

		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		protected virtual void Share (string _message, string _URLString, byte[] _imageByteArray, string _excludedOptionsJsonString, SharingCompletion _onCompletion)
		{
			// Pause unity player
			this.PauseUnity();
			
			// Cache callback
			OnSharingFinished	= _onCompletion;
		}

		#endregion
	}
}