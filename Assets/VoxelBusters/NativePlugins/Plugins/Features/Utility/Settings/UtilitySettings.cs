using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Utility Settings provides interface to configure properties related to utility components such as RateMyApp.
	/// </summary>
	[System.Serializable]
	public class UtilitySettings 
	{
		#region Fields

		[SerializeField]
		private 	RateMyApp.Settings		m_rateMyApp;

		#endregion

		#region Properties

		public RateMyApp.Settings RateMyApp
		{
			get
			{
				return m_rateMyApp;
			}

			private set
			{
				m_rateMyApp	= value;
			}
		}

		#endregion
	}
}