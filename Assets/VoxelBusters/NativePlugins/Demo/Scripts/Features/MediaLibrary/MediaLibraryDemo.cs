using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.Utility.UnityGUI.MENU;
using VoxelBusters.NativePlugins;
using VoxelBusters.AssetStoreProductUtility.Demo;

namespace VoxelBusters.NativePlugins.Demo
{
	public class MediaLibraryDemo : DemoSubMenu 
	{
		#region Properties
		
		[SerializeField]
		private Texture2D		m_texture;

		[Tooltip ("This needs to be direct link to the video file. ex: http://www.google.com/video.mp4")]		
		[SerializeField]
		private string			m_videoURL;

		[SerializeField]
		private string			m_youtubeVideoID;

		[SerializeField]
		private string			m_embedHTMLString;

		#endregion

		#region API Calls
		
		private bool IsCameraSupported ()
		{
			return NPBinding.MediaLibrary.IsCameraSupported();
		}

		private void PickImageFromAlbum ()
		{
			// Set popover to last touch position
			NPBinding.UI.SetPopoverPointAtLastTouchPosition();

			// Pick image
			NPBinding.MediaLibrary.PickImage(eImageSource.ALBUM, 1.0f, PickImageFinished);
		}

		private void PickImageFromCamera ()
		{
			// Set popover to last touch position
			NPBinding.UI.SetPopoverPointAtLastTouchPosition();
			
			// Pick image
			NPBinding.MediaLibrary.PickImage(eImageSource.CAMERA, 1.0f, PickImageFinished);
		}
		
		private void PickImageFromBoth ()
		{
			// Set popover to last touch position
			NPBinding.UI.SetPopoverPointAtLastTouchPosition();
			
			// Pick image
			NPBinding.MediaLibrary.PickImage(eImageSource.BOTH, 1.0f, PickImageFinished);
		}

		private void SaveScreenshotToGallery ()
		{
			NPBinding.MediaLibrary.SaveScreenshotToGallery(SaveImageToGalleryFinished);
		}
		
		private void PlayYoutubeVideo ()
		{
			NPBinding.MediaLibrary.PlayYoutubeVideo(m_youtubeVideoID, PlayVideoFinished);
		}
		
		private void PlayVideoFromURL ()
		{
			NPBinding.MediaLibrary.PlayVideoFromURL(URL.URLWithString(m_videoURL), PlayVideoFinished);
		}
		
		private void PlayVideoFromGallery ()
		{
			// Set popover to last touch position
			NPBinding.UI.SetPopoverPointAtLastTouchPosition();
			
			// Play video from gallery
			NPBinding.MediaLibrary.PlayVideoFromGallery(PickVideoFinished, PlayVideoFinished);
		}
		
		private void PlayEmbeddedVideo ()
		{
			NPBinding.MediaLibrary.PlayEmbeddedVideo(m_embedHTMLString, PlayVideoFinished);
		}
		
		#endregion

		#region API Callbacks
		
		private void PickImageFinished (ePickImageFinishReason _reason, Texture2D _image)
		{
			AddNewResult("Image picker was closed");
			AppendResult("Reason = " + _reason);
			AppendResult("Texture Image = " + _image);
		}
		
		private void SaveImageToGalleryFinished (bool _saved)
		{
			AddNewResult("Received Finished saving image to gallery Event");
			AppendResult("Saved successfully ? " + _saved);
		}
		
		private void PickVideoFinished (ePickVideoFinishReason _reason)
		{
			AddNewResult("Finished picking video from gallery");
			AppendResult("Reason = " + _reason);
		}
		
		private void PlayVideoFinished (ePlayVideoFinishReason _reason)
		{
			AddNewResult("Finished playing video");
			AppendResult("Reason = " + _reason);
		}

		#endregion

		#region UI
		
		protected override void OnGUIWindow()
		{		
			base.OnGUIWindow();
			
			RootScrollView.BeginScrollView();
			{
				DrawImageAPI();
				DrawVideoAPI();
			}
			RootScrollView.EndScrollView();
			
			DrawResults();
			DrawPopButton();
		}

		private void DrawImageAPI ()
		{
			GUILayout.Label("Image", kSubTitleStyle);
			
			if (GUILayout.Button("IsCameraSupported"))
			{
				bool _isSupported = IsCameraSupported();
				AddNewResult("IsCameraSupported ? " + _isSupported);
			}
			
			GUILayout.BeginHorizontal();
			{
				if (GUILayout.Button("PickImage From ALBUM"))
				{
					PickImageFromAlbum();
				} 
				
				if (GUILayout.Button("PickImage From CAMERA"))
				{
					PickImageFromCamera();
				}
			}
			GUILayout.EndHorizontal(); 
			
			if (GUILayout.Button("PickImage From BOTH - ALBUM & CAMERA"))
			{
				PickImageFromBoth();
			} 
			
			if (GUILayout.Button("SaveScreenshotToAlbum"))
			{						
				SaveScreenshotToGallery();
			}
		}

		private void DrawVideoAPI ()
		{
			GUILayout.Label("Video", kSubTitleStyle);
			
			if (GUILayout.Button("Play Youtube Video"))
			{						
				PlayYoutubeVideo();
			} 
			
			if (GUILayout.Button("Play Video From URL"))
			{		
				PlayVideoFromURL();
			} 
			
			if (GUILayout.Button("Play Video From Gallery"))
			{						
				PlayVideoFromGallery();
			} 
			
			if (GUILayout.Button("Play Embedded Video"))
			{		
				PlayEmbeddedVideo();				
			} 
		}
		
		#endregion
	}
}