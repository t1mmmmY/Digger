using UnityEngine;
using System.Collections;
using System.IO;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public class MailShareComposer : ShareImageUtility, IShareView
	{
		#region Properties

		public string Subject
		{
			get;
			set;
		}

		public string Body
		{
			get;
			set;
		}

		public bool IsHTMLBody
		{
			get;
			set;
		}

		public string[] ToRecipients
		{
			get;
			set;
		}

		public string[] CCRecipients
		{
			get;
			set;
		}

		public string[] BCCRecipients
		{
			get;
			set;
		}

		public byte[] AttachmentData
		{
			get;
			private set;
		}
		
		public string AttachmentFileName
		{
			get;
			private set;
		}
		
		public string MimeType
		{
			get;
			private set;
		}

		public bool IsReadyToShowView 
		{
			get
			{
				return !ImageAsyncDownloadInProgress;
			}
		}

		#endregion

		#region Methods

		public override void AttachImage (Texture2D _texture)
		{
			if (_texture != null)
			{
				AttachmentData		= _texture.EncodeToPNG();
				AttachmentFileName	= "ShareImage.png";
				MimeType			= MIMEType.kPNG;
			}
			else
			{
				AttachmentData		= null;
				AttachmentFileName	= null;
				MimeType			= null;
			}
		}

		public void AddAttachmentAtPath (string _attachmentPath, string _MIMEType)
		{
			byte[]	_attachmentData		= null;
			string	_attachmentFileName	= null;

			// File exists
			if (FileOperations.Exists(_attachmentPath))
			{
				_attachmentData			= FileOperations.ReadAllBytes(_attachmentPath);
				_attachmentFileName		= Path.GetFileName(_attachmentPath);
			}

			AddAttachment(_attachmentData, _attachmentFileName, _MIMEType);
		}

		public void AddAttachment (byte[] _attachmentData, string _attachmentFileName, string _MIMEType)
		{
			AttachmentData		= _attachmentData;
			AttachmentFileName	= _attachmentFileName;
			MimeType			= _MIMEType;
		}

		#endregion
	}
}