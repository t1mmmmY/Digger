using UnityEngine;
using System.Collections;

#if USES_NETWORK_CONNECTIVITY
namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Check if the device is connected to internet.
	/// </summary>
	public partial class NetworkConnectivity : MonoBehaviour 
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating whether network is connected.
		/// </summary>
		/// <value><c>true</c> if network is connected; otherwise, <c>false</c>.</value>
		public bool IsConnected
		{
			get;
		 	protected set;
		}

		#endregion

		#region API Methods

		/// <summary>
		/// Initialise Network reachability checking.
		/// </summary>
		///	<description> This starts checking if IP Address psecified in the connectivity settings is reachable or not and delivers events. </description>
		public virtual void Initialise ()
		{
			StartCoroutine(ManuallyTriggerInitialState());
		}

		#endregion

		#region Misc. Methods

		private IEnumerator ManuallyTriggerInitialState ()
		{
			yield return new WaitForSeconds(1f);

			if (IsConnected == false)
				ConnectivityChanged(false);
		}

		#endregion
	}
}
#endif