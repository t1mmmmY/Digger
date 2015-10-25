using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Network Connectivity Settings provides interface to configure properties related to network connectivity.
	/// </summary>
	[System.Serializable]
	public class NetworkConnectivitySettings
	{
		#region Editor Settings

		/// <summary>
		/// Network Connectivity Settings specific to Unity Editor.
		/// </summary>
		[System.Serializable]
		public class EditorSettings
		{
			#region Fields

			[SerializeField]
			private 	int 		m_timeOutPeriod 		= 60;
			[SerializeField]
			private 	int 		m_maxRetryCount 		= 2;
			[SerializeField]
			private 	float 		m_timeGapBetweenPolling = 2.0f;

			#endregion

			#region Properties
		
			/// <summary>
			/// Gets or sets the time out period.
			/// </summary>
			/// <value>The time out period.</value>
			public int TimeOutPeriod
			{
				get 
				{ 
					return m_timeOutPeriod; 
				}

				private set
				{
					m_timeOutPeriod	= value;
				}
			}

			/// <summary>
			/// Gets or sets the max retry count, whenever polling fails.
			/// </summary>
			/// <value>The max retry count.</value>
			public int MaxRetryCount
			{
				get 
				{ 
					return m_maxRetryCount; 
				}
				
				private set
				{
					m_maxRetryCount	= value;
				}
			}
			
			/// <summary>
			/// Gets or sets the time gap between successive polling.
			/// </summary>
			/// <value>The time gap between polling.</value>
			public float TimeGapBetweenPolling
			{
				get 
				{ 
					return m_timeGapBetweenPolling;
				}
				
				private set
				{
					m_timeGapBetweenPolling	= value;
				}
			}
		}

		#endregion

		#endregion

		#region Android Settings

		/// <summary>
		/// Network Connectivity Settings specific to Android Editor.
		/// </summary>
		[System.Serializable]
		public class AndroidSettings
		{
			#region Fields

			[Tooltip ("On Android, it uses sockets to connect. So port is required. For DNS ip it will be 53 else 80.")]		
			[SerializeField]
			private 	int 		m_port 					= 53;
			[SerializeField]
			private 	int 		m_timeOutPeriod 		= 60;
			[SerializeField]
			private 	int 		m_maxRetryCount 		= 2;
			[SerializeField]
			private 	float 		m_timeGapBetweenPolling = 2.0f;

			#endregion

			#region Properties

			/// <summary>
			/// Gets or sets the Port to connect to.
			/// </summary>
			/// <value>Port to connect.</value>
			public int Port
			{
				get 
				{ 
					return m_port; 
				}
				
				private set
				{
					m_port	= value;
				}
			}

			/// <summary>
			/// Gets or sets the time out period.
			/// </summary>
			/// <value>The time out period.</value>
			public int TimeOutPeriod
			{
				get 
				{ 
					return m_timeOutPeriod; 
				}
				
				private set
				{
					m_timeOutPeriod	= value;
				}
			}
			
			/// <summary>
			/// Gets or sets the max retry count, whenever polling fails.
			/// </summary>
			/// <value>The max retry count.</value>
			public int MaxRetryCount
			{
				get 
				{ 
					return m_maxRetryCount; //TODO should allow setters as well
				}
				
				private set
				{
					m_maxRetryCount	= value;
				}
			}
			
			/// <summary>
			/// Gets or sets the time gap between successive polling.
			/// </summary>
			/// <value>The time gap between polling.</value>
			public float TimeGapBetweenPolling
			{
				get 
				{ 
					return m_timeGapBetweenPolling; //TODO should allow setters as well
				}
				
				private set
				{
					m_timeGapBetweenPolling	= value;
				}
			}
		}

		#endregion

		#endregion

		#region Fields
		
		[SerializeField]
		private 	string 			m_ipAddress 	= "8.8.8.8";
		[SerializeField]
		private 	EditorSettings	m_editor;
		[SerializeField]
		private 	AndroidSettings	m_android;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the IP Address. IP Address is used to check connectivity.
		/// </summary>
		/// <value>Address to check reachabilty.</value>
		public string IPAddress
		{
			get 
			{ 
				return m_ipAddress; 
			}

			private set 
			{ 
				m_ipAddress = value; 
			}
		}

		/// <summary>
		/// Gets or sets the Network Connectivity Settings specific to Unity Editor.
		/// </summary>
		/// <value>The android.</value>
		public EditorSettings Editor
		{
			get 
			{ 
				return m_editor; 
			}
			
			private set 
			{ 
				m_editor = value; 
			}
		}

		/// <summary>
		/// Gets or sets the Network Connectivity Settings specific to Android platform.
		/// </summary>
		/// <value>The android.</value>
		public AndroidSettings Android
		{
			get 
			{ 
				return m_android; 
			}

		 	private set 
			{ 
				m_android = value; 
			}
		}
		
		#endregion

		#region Constructor

		public NetworkConnectivitySettings ()
		{
			Android		= new AndroidSettings();
			Editor		= new EditorSettings();
		}

		#endregion
	}
}