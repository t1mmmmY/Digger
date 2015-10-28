using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	using Internal;
	
	public class WhatsAppShareComposer : ShareImageUtility, IShareView
	{
		#region Properties

		public string Text
		{
			get;
			set;
		}
		
		public byte[] ImageData
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
				ImageData	= _texture.EncodeToPNG();
			else
				ImageData	= null;
		}
		
		#endregion
	}
}