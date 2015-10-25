using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class MediaLibrary : MonoBehaviour 
	{
		#region Delegates

		/// <summary>
		/// Use this delegate type to get callback when pick image action is done. Check _reason for success or failure. If successful, returns the texture of picked image.
		/// </summary>
		public delegate void PickImageCompletion (ePickImageFinishReason _reason, Texture2D _image);

		/// <summary>
		/// Use this delegate type to get callback when image is saved to gallery. 
		/// </summary>
		public delegate void SaveImageToGalleryCompletion (bool _savedSuccessfully);

		/// <summary>
		/// Use this delegate type to get callback when pick video action is done. Check _reason for success or failure.
		/// </summary>
		public delegate void PickVideoCompletion (ePickVideoFinishReason _reason);

		/// <summary>
		/// Use this delegate type to get callback when play video action is done. Check _reason for success or failure.
		/// </summary>
		public delegate void PlayVideoCompletion (ePlayVideoFinishReason _reason);

		#endregion

		#region Events

		protected PickImageCompletion			OnPickImageFinished;
		protected SaveImageToGalleryCompletion	OnSaveImageToGalleryFinished;
		protected PickVideoCompletion			OnPickVideoFinished;
		protected PlayVideoCompletion			OnPlayVideoFinished;

		#endregion

		#region Image Callback Methods

		protected void PickImageFinished (string _responseJson)
		{
			IDictionary	_responseDict		= JSONUtility.FromJSON(_responseJson) as IDictionary;
			string _imagePath;
			ePickImageFinishReason _finishReason;

			// Parse received data
			ParsePickImageFinishedData(_responseDict, out _imagePath, out _finishReason);

			if (string.IsNullOrEmpty(_imagePath) && _finishReason != ePickImageFinishReason.CANCELLED)
			{
				_finishReason	= ePickImageFinishReason.FAILED;
				_imagePath		= null;
			}

			// Triggers event
			PickImageFinished(_imagePath, _finishReason);
		}
		
		protected void PickImageFinished (string _imagePath, ePickImageFinishReason _finishReason)
		{
			// Resume unity player
			this.ResumeUnity();
			
			Console.Log(Constants.kDebugTag, "[MediaLibrary] Finishing pick image, Path=" + _imagePath + " Reason=" + _finishReason);
			if (OnPickImageFinished != null)
			{
				// Failed opertation
				if (_finishReason != ePickImageFinishReason.SELECTED)
				{
					OnPickImageFinished(_finishReason, null);
					return;
				}
				
				// Download image from given path
				URL _imagePathURL				= URL.FileURLWithPath(_imagePath);
				DownloadTexture _newDownload	= new DownloadTexture(_imagePathURL, true, true);
				_newDownload.ScaleFactor		= m_scaleFactor;
				_newDownload.OnCompletion		= (Texture2D _texture, string _error)=>{
					
					if (string.IsNullOrEmpty(_error))
					{
						OnPickImageFinished(ePickImageFinishReason.SELECTED, _texture);
					}
					else
					{
						Console.LogError(Constants.kDebugTag, "[MediaLibrary] Texture download failed, URL=" + _imagePathURL.URLString);
						OnPickImageFinished(ePickImageFinishReason.FAILED, null);
					}
				};
				
				// Start download
				_newDownload.StartRequest();
			}
		}	
		
		protected void SaveImageToGalleryFinished (string _savedStatus)
		{
			bool _savedSuccessfully	= bool.Parse(_savedStatus);

			// Triggers event
			SaveImageToGalleryFinished(_savedSuccessfully);
		}
		
		protected void SaveImageToGalleryFinished (bool _savedSuccessfully)
		{
			Console.Log(Constants.kDebugTag, "[MediaLibrary] Saving image to gallery finished, Success=" + _savedSuccessfully);
			
			if (OnSaveImageToGalleryFinished != null)
				OnSaveImageToGalleryFinished(_savedSuccessfully);
		}

		#endregion

		#region Video Callback Methods 

		protected void PickVideoFinished (string _reasonStr)
		{
			ePickVideoFinishReason _finishReason;

			// Parse received data
			ParsePickVideoFinishedData(_reasonStr, out _finishReason);
			
			// Triggers event
			PickVideoFinished(_finishReason);
		}
		
		protected void PickVideoFinished (ePickVideoFinishReason _finishReason)
		{
			Console.Log(Constants.kDebugTag, "[MediaLibrary] Pick video finished, Reason=" + _finishReason);
			
			// If pick video reason is Selected then dont resume, as operation still incomplete
			if (_finishReason != ePickVideoFinishReason.SELECTED)
			{
				// Resume unity player
				this.ResumeUnity();
			}
			
			if (OnPickVideoFinished != null)
				OnPickVideoFinished(_finishReason);
		}

		protected void PlayVideoFinished (string _reasonStr)
		{
			ePlayVideoFinishReason _finishReason;

			// Parse received data
			ParsePlayVideoFinishedData(_reasonStr, out _finishReason);
			
			// Triggers event
			PlayVideoFinished(_finishReason);
		}

		protected void PlayVideoFinished(ePlayVideoFinishReason _finishReason)
		{
			// Resume unity player
			this.ResumeUnity();
			
			Console.Log(Constants.kDebugTag, "[MediaLibrary] Playing video finished, Reason=" + _finishReason);

			if (OnPlayVideoFinished != null)
				OnPlayVideoFinished(_finishReason);
		}

		#endregion

		#region Parse Methods

		protected virtual void ParsePickImageFinishedData (IDictionary _infoDict, out string _selectedImagePath, out ePickImageFinishReason _finishReason)
		{
			// Default values
			_selectedImagePath	= null;
			_finishReason		= ePickImageFinishReason.FAILED;
		}

		protected virtual void ParsePickVideoFinishedData (string _reasonString, out ePickVideoFinishReason _finishReason)
		{
			// Default values
			_finishReason		= ePickVideoFinishReason.FAILED;
		}

		protected virtual void ParsePlayVideoFinishedData (string _reasonString, out ePlayVideoFinishReason _finishReason)
		{
			// Default values
			_finishReason		= ePlayVideoFinishReason.PLAYBACK_ERROR;
		}

		#endregion
	}
}