using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	public class MessageShareComposer : IShareView
	{
		#region Properties
		
		public string Body
		{
			get;
			set;
		}
		
		public string[] ToRecipients
		{
			get;
			set;
		}
		
		public bool IsReadyToShowView 
		{
			get
			{
				return true;
			}
		}
		
		#endregion
	}
}