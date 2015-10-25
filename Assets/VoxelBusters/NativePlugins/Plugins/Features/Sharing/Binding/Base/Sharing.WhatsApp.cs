using UnityEngine;
using System.Collections;
using System.IO;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class Sharing : MonoBehaviour 
	{
		#region Methods

		/// <summary>
		/// Determines whether whatsApp service is available.
		/// </summary>
		/// <returns><c>true</c> if whatsApp service is available; otherwise, <c>false</c>.</returns>
		public virtual bool IsWhatsAppServiceAvailable ()
		{
			bool _isAvailable	= false;
			Console.Log(Constants.kDebugTag, "[Sharing:WhatsApp] Is service available=" + _isAvailable);
			
			return _isAvailable;
		}

		protected virtual void ShowWhatsAppShareComposer (WhatsAppShareComposer _composer)
		{
			if (!IsWhatsAppServiceAvailable())
			{
				WhatsAppShareFinished(WhatsAppShareFailedResponse());
				return;
			}
		}

		#endregion

		#region Deprecated Methods

		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public virtual void ShareTextMessageOnWhatsApp (string _message, SharingCompletion _onCompletion)
		{
			// Pause unity player
			this.PauseUnity();
			
			// Cache callback
			OnSharingFinished	= _onCompletion;
			
			// Sharing on whatsapp isnt supported
			if (string.IsNullOrEmpty(_message) || !IsWhatsAppServiceAvailable())
			{
				Console.Log(Constants.kDebugTag, "[Sharing:WhatsApp] Failed to share text");
				WhatsAppShareFinished(WhatsAppShareFailedResponse());
				return;
			}
		}
		
		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareScreenshotOnWhatsApp (SharingCompletion _onCompletion)
		{
			// First capture frame
			StartCoroutine(TextureExtensions.TakeScreenshot((_texture)=>{
				// Convert texture into byte array
				byte[] _imageByteArray	= _texture.EncodeToPNG();
				
				// Share
				ShareImageOnWhatsApp(_imageByteArray, _onCompletion);
			}));
		}
		
		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareImageOnWhatsApp (string _imagePath, SharingCompletion _onCompletion)
		{
			if (!File.Exists(_imagePath))
			{
				Console.LogError(Constants.kDebugTag, "[Sharing:WhatsApp] File doesnt exist. Path="  + _imagePath);
				ShareImageOnWhatsApp((byte[])null, _onCompletion);
				return;
			}
			
			// Get file data
			byte[] _imageByteArray	= FileOperations.ReadAllBytes(_imagePath);
			
			// Share
			ShareImageOnWhatsApp(_imageByteArray, _onCompletion);
		}
		
		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public void ShareImageOnWhatsApp (Texture2D _texture, SharingCompletion _onCompletion)
		{
			if (_texture == null)
			{
				Console.LogError(Constants.kDebugTag, "[Sharing:WhatsApp] Texture is null");
				ShareImageOnWhatsApp((byte[])null, _onCompletion);
				return;
			}
			
			// Convert texture into byte array
			byte[] _imageByteArray	= _texture.EncodeToPNG();
			
			// Share
			ShareImageOnWhatsApp(_imageByteArray, _onCompletion);
		}

		[System.Obsolete(kSharingFeatureDeprecatedMethodInfo)]
		public virtual void ShareImageOnWhatsApp (byte[] _imageByteArray, SharingCompletion _onCompletion)
		{
			// Pause unity player
			this.PauseUnity();
			
			// Cache callback
			OnSharingFinished	= _onCompletion;
			
			// Sharing on whatsapp isnt supported
			if (_imageByteArray == null || !IsWhatsAppServiceAvailable())
			{
				Console.LogError(Constants.kDebugTag, "[Sharing:WhatsApp] Failed to share image");
				WhatsAppShareFinished(WhatsAppShareFailedResponse());
				return;
			}		
		}
		
		#endregion
	}
}