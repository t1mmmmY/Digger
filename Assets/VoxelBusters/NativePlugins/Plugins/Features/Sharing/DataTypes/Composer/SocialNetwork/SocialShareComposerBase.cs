using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public class SocialShareComposerBase : ShareImageUtility, IShareView
	{
		#region Properties

		public eSocialServiceType ServiceType
		{
			get;
			private set;
		}

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
		
		public bool IsReadyToShowView 
		{
			get
			{
				return !ImageAsyncDownloadInProgress;
			}
		}
		
		#endregion

		#region Constructors

		private SocialShareComposerBase ()
		{}

		protected SocialShareComposerBase (eSocialServiceType _serviceType)
		{
			ServiceType	= _serviceType;
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