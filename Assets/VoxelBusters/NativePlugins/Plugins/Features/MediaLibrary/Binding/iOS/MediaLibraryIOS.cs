using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

#if UNITY_IOS
namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class MediaLibraryIOS : MediaLibrary
	{
		#region Native Methods

		[DllImport("__Internal")]
		private static extern bool isCameraSupported ();
		
		[DllImport("__Internal")]
		private static extern void pickImage (int _accessPhotoInt, float _scaleFactor);
		
		[DllImport("__Internal")]
		private static extern void saveImageToGallery (byte[] _imgByteArray, int _imgByteArrayLength);

		[DllImport("__Internal")]
		private static extern void playVideoUsingWebView (string _embedHTMLString);

		[DllImport("__Internal")]
		private static extern void playVideoFromURL (string _URLString);

		[DllImport("__Internal")]
		private static extern void playVideoFromGallery ();

		#endregion

		#region Image

		public override bool IsCameraSupported ()
		{
			bool _isSupported	= isCameraSupported();
			Console.Log(Constants.kDebugTag, "[MediaLibrary] IsCameraSupported=" + _isSupported);

			return _isSupported;
		}
		
		public override void PickImage (eImageSource _source, float _scaleFactor, PickImageCompletion _onCompletion)
		{
			base.PickImage(_source, _scaleFactor, _onCompletion);

			if (_scaleFactor > 0f)
			{
				// Opens image picker
				pickImage((int)_source, _scaleFactor);
			}
		}
		
		public override void SaveImageToGallery (byte[] _imageByteArray, SaveImageToGalleryCompletion _onCompletion)
		{
			base.SaveImageToGallery(_imageByteArray, _onCompletion);

			if (_imageByteArray != null)
				saveImageToGallery(_imageByteArray, _imageByteArray.Length);
		}

		#endregion

		#region Video

		public override void PlayEmbeddedVideo (string _embedHTMLString, PlayVideoCompletion _onCompletion)
		{
			base.PlayEmbeddedVideo(_embedHTMLString, _onCompletion);
			
			if (!string.IsNullOrEmpty(_embedHTMLString))
				playVideoUsingWebView(_embedHTMLString);
		}
		
		public override void PlayVideoFromURL (URL _URL, PlayVideoCompletion _onCompletion)
		{
			base.PlayVideoFromURL(_URL, _onCompletion);
			
			if (!string.IsNullOrEmpty(_URL.URLString))
			{
				// Check if this url is a youtube link
				string _youtubeID = ExtractYoutubeVideoID(_URL.URLString);

				if(_youtubeID != null)
				{
					PlayYoutubeVideo(_youtubeID, _onCompletion);
				}
				else
				{
					playVideoFromURL(_URL.URLString);
				}

			}
		}
		
		public override void PlayVideoFromGallery (PickVideoCompletion _onPickVideoCompletion, PlayVideoCompletion _onPlayVideoCompletion)
		{
			base.PlayVideoFromGallery(_onPickVideoCompletion, _onPlayVideoCompletion);
			
			// Native call
			playVideoFromGallery();
		}

		#endregion
	}
}
#endif	