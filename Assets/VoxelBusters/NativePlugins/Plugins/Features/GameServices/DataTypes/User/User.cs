using UnityEngine;
using System.Collections;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// <see cref="VoxelBusters.NativePlugins.User"/> class provides information about a user playing your game.
	/// </summary>
	public abstract class User
	{
		#region Properties

		/// <summary>
		/// Gets the identifier for this <see cref="VoxelBusters.NativePlugins.User"/>.
		/// </summary>
		/// <value>A string assigned to uniquely identify a <see cref="VoxelBusters.NativePlugins.User"/>.</value>
		public abstract string Identifier
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the name for this <see cref="VoxelBusters.NativePlugins.User"/>.
		/// </summary>
		/// <value>Display name chosen by <see cref="VoxelBusters.NativePlugins.User"/> to identify themselves to other players.</value>
		public abstract string Name
		{
			get;
			protected set;
		}
		
		#endregion
		
		#region Constructors
		
		protected User ()
		{}

		protected User (string _id, string _name)
		{
			// Initialize properties
			Identifier		= _id;
			Name			= _name;
		}
		
		#endregion

		#region Abstract Methods

		/// <summary>
		/// Asynchronously loads the image.
		/// </summary>
		/// <param name="_onCompletion">Callback to be triggered after loading the image.</param>
		public abstract void GetImageAsync (DownloadTexture.Completion _onCompletion);

		#endregion

		#region Methods

		public override string ToString ()
		{
			return string.Format("[User: Name={0}]", Name);
		}

		#endregion
	}
}