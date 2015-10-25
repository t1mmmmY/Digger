using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using System.Text.RegularExpressions;
using VoxelBusters.DebugPRO;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	///	Media library provides an unique way to access devices's media gallery and camera for picking images and playing videos.
	/// </summary>
	public partial class MediaLibrary : MonoBehaviour 
	{
		#region Properties

		private		float		m_scaleFactor;

		#endregion

		#region Unity Methods
		
		protected virtual void Awake()
		{}
		
		#endregion

		#region Pick Image

		/// <summary>
		/// Determines if camera is supported.
		/// </summary>
		/// <returns><c>true</c> if camera is supported; otherwise, <c>false</c>.</returns>
		public virtual bool IsCameraSupported ()
		{
			bool _isSupported	= false;
			Console.Log(Constants.kDebugTag, "[MediaLibrary] IsCameraSupported=" + _isSupported);

			return _isSupported;
		}
		
		/// <summary>
		/// Picks the image.
		/// </summary>
		/// <param name="_source"> Specify from where you want to pick the image from. <see cref="eImageSource"/> </param>
		/// <param name="_scaleFactor">Specify if scaled up or scaled down version of pick image is required. 1.0f returns the image with out any modification.</param>
		/// <param name="_onCompletion">Callback triggered once Pick from the source is finised.</param>
		public virtual void PickImage (eImageSource _source, float _scaleFactor, PickImageCompletion _onCompletion)
		{
			// Pause unity player
			this.PauseUnity();

			// Cache properties
			m_scaleFactor			= _scaleFactor;
			OnPickImageFinished		= _onCompletion;

			if (_scaleFactor <= 0f)
			{
				PickImageFinished(null, ePickImageFinishReason.FAILED);
				return;
			}
		}

		#endregion

		#region Album

		/// <summary>
		/// Saves the screenshot to gallery/media library of the users device.
		/// </summary>
		/// <param name="_onCompletion">Callback triggered once  save to gallery is done.</param>
		public void SaveScreenshotToGallery (SaveImageToGalleryCompletion _onCompletion)
		{
			// First capture screenshot
			StartCoroutine(TextureExtensions.TakeScreenshot((_texture)=>{

				// Now save it
				SaveImageToGallery(_texture, _onCompletion);
			}));
		}
		
		/// <summary>
		/// Saves an image from the specified url to gallery/media library of the users device.
		///	\note The path needs to be absolute path if its local file. Take care of the path on multiple platforms as the file structure will be different.
		/// </summary>
		/// <param name="_URL">URL to pick the source from. This can be a file url existing on local storage or a web url at remote location.</param>
		/// <param name="_onCompletion">Callback triggered once  save to gallery is done.</param>
		public void SaveImageToGallery (URL _URL, SaveImageToGalleryCompletion _onCompletion)
		{
			// Download texture from given URL
			DownloadTexture _newDownload	= new DownloadTexture(_URL, true, false);
			_newDownload.OnCompletion		= (Texture2D _texture, string _error)=>{

				// Save downloaded texture
				if (!string.IsNullOrEmpty(_error))
				{
					Console.LogError(Constants.kDebugTag, "[MediaLibrary] Texture download failed, URL=" + _URL.URLString);
				}

				// Save image
				SaveImageToGallery(_texture, _onCompletion);
			};

			// Start download
			_newDownload.StartRequest();
		}

		/// <summary>
		/// Saves the specified texture to gallery.
		/// </summary>
		/// <param name="_texture">Texture that needs to be saved to gallery.</param>
		/// <param name="_onCompletion">Callback triggered once  save to gallery is done.</param>
		public void SaveImageToGallery (Texture2D _texture, SaveImageToGalleryCompletion _onCompletion)
		{
			byte[] _imageByteArray	= null;

			// Convert texture to byte array
			if (_texture != null)
			{
				_imageByteArray	= _texture.EncodeToPNG();
			}

			// Use api to save
			SaveImageToGallery(_imageByteArray, _onCompletion);
		}
		
		/// <summary>
		/// Saves the specified image data source to gallery.
		/// </summary>
		/// <param name="_imageByteArray">image byte array to use as source.</param>
		/// <param name="_onCompletion">Callback triggered once  save to gallery is done.</param>
		public virtual void SaveImageToGallery (byte[] _imageByteArray, SaveImageToGalleryCompletion _onCompletion)
		{
			// Cache callback
			OnSaveImageToGalleryFinished	= _onCompletion;

			if (_imageByteArray == null)
			{
				Console.LogError(Constants.kDebugTag, "[MediaLibrary] Saving image to album failed, texture data is null");
				SaveImageToGalleryFinished(false);
				return;
			}
		}

		#endregion

		#region Video

		/// <summary>
		/// Plays youtube video for the corresponding video id specified.
		/// </summary>
		/// <param name="_videoID">Video id of the youtube video.</param>
		/// <param name="_onCompletion">Callback triggered on completion or failure. see <see cref="ePlayVideoFinishReason"/> for status.</param>
		public virtual void PlayYoutubeVideo (string _videoID, PlayVideoCompletion _onCompletion)
		{	
			// Pause unity player
			this.PauseUnity();
		
			// Cache callback
			OnPlayVideoFinished	= _onCompletion;

			if (string.IsNullOrEmpty(_videoID))
			{
				Console.LogError(Constants.kDebugTag, "[MediaLibrary] Play youtube video failed, Video ID can't be null");
				PlayVideoFinished(ePlayVideoFinishReason.PLAYBACK_ERROR);
				return;
			}
		}

		/// <summary>
		/// Plays embedded video using web view.
		/// </summary>
		/// <param name="_embedHTMLString">Embed HTML string to load in the webview.</param>
		/// <param name="_onCompletion">Callback triggered on completion or failure. see <see cref="ePlayVideoFinishReason"/> for status.</param>
		public virtual void PlayEmbeddedVideo (string _embedHTMLString, PlayVideoCompletion _onCompletion)
		{
			// Pause unity player
			this.PauseUnity();
			
			// Cache callback
			OnPlayVideoFinished	= _onCompletion;

			if (string.IsNullOrEmpty(_embedHTMLString))
			{
				Console.LogError(Constants.kDebugTag, "[MediaLibrary] Play video using webview failed, HTML string cant be null");
				PlayVideoFinished(ePlayVideoFinishReason.PLAYBACK_ERROR);
				return;
			}
		}

		/// <summary>
		/// Play video from specified URL. This can be an URL pointing to local/remote file. 
		/// </summary>
		///	<remarks>\warning This URL needs to point directly to the video. Direct video link ex: www.voxelbusters.com/movie.mp4 </remarks>
		/// <param name="_URL">URL of the video to play.</param>
		/// <param name="_onCompletion">Callback triggered on completion or failure. see <see cref="ePlayVideoFinishReason"/> for status.</param>
		public virtual void PlayVideoFromURL (URL _URL, PlayVideoCompletion _onCompletion)
		{
			// Pause unity player
			this.PauseUnity();

			// Cache callback
			OnPlayVideoFinished	= _onCompletion;

			if (string.IsNullOrEmpty(_URL.URLString))
			{
				Console.LogError(Constants.kDebugTag, "[MediaLibrary] Play video from URL failed, URL can't be null");
				PlayVideoFinished(ePlayVideoFinishReason.PLAYBACK_ERROR);
				return;
			}
		}

		/// <summary>
		/// Allows to pick a video from gallery and play it.
		/// </summary>
		/// <param name="_onPickVideoCompletion">Triggered once pick video action is done. </param>
		/// <param name="_onPlayVideoCompletion">Callback triggered on completion or failure. see <see cref="ePlayVideoFinishReason"/> for status.</param>
		public virtual void PlayVideoFromGallery (PickVideoCompletion _onPickVideoCompletion, PlayVideoCompletion _onPlayVideoCompletion)
		{
			// Pause unity player
			this.PauseUnity();

			// Cache callback
			OnPickVideoFinished	= _onPickVideoCompletion;
			OnPlayVideoFinished	= _onPlayVideoCompletion;
		}

		#endregion

		#region Helpers

		protected string ExtractYoutubeVideoID (string _url)
		{
			string _youtubeID = null;

			//Regex for youtube from - http://fiddle.re/w1nn6
			Match regexMatch = Regex.Match(_url, "^(?:https?\\:\\/\\/)?(?:www\\.)?(?:youtu\\.be\\/|youtube\\.com\\/(?:embed\\/|v\\/|watch\\?v\\=))([\\w-]{10,12})(?:[\\&\\?\\#].*?)*?(?:[\\&\\?\\#]t=([\\dhm]+s))?$", 
			                               RegexOptions.IgnoreCase);
			if (regexMatch.Success)
			{
				foreach(Group each in regexMatch.Groups)
				{
					Console.Log(Constants.kDebugTag, "Value "+each.Value);
				}

				if(regexMatch.Groups.Count > 1)
				{
					_youtubeID = regexMatch.Groups[1].Value;
				}
			}
			return _youtubeID;
		}

		protected string GetYoutubeEmbedHTMLString (string _videoID)
		{
			// Load Youtube player HTML script
			TextAsset _youtubePlayerHTML	= Resources.Load("YoutubePlayer", typeof(TextAsset)) as TextAsset;
			string _embedHTMLString			= null;
			
			if (_youtubePlayerHTML != null)
			{
				_embedHTMLString			= _youtubePlayerHTML.text.Replace("%@", _videoID);
			}

			return _embedHTMLString;
		}

		#endregion
	}
}