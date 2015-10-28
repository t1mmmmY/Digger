using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public class ShareSheet : ShareImageUtility, IShareView
	{
		#region Fields

		private		eShareOptions[] 	m_excludedShareOptions;

		#endregion

		#region Properties

		public string Text
		{
			get;
			set;
		}

		public string URL
		{
			get;
			set;
		}
		
		public byte[] ImageData
		{
			get;
			private set;
		}

		public virtual eShareOptions[] ExcludedShareOptions
		{
			get
			{
				return m_excludedShareOptions;
			}

			set
			{
				m_excludedShareOptions	= value;
			}
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