using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	[CustomEditor(typeof(EditorNotificationCenter))]
	public class EditorNotificationCenterInspector : Editor 
	{
		#region Properties

		private EditorNotificationCenter	NotificationCenter
		{
			get 
			{
				return target as EditorNotificationCenter;
			}
		}

		private List<SerializedProperty>	m_serializableProperties;
		private bool						m_showLocalNotifications			= true;
		private GUIContent					m_localNotificationsGUIContent		= new GUIContent("Local Notifications", "Received local notifications");
		private bool						m_showRemoteNotifications			= true;
		private GUIContent					m_remoteNotificationsGUIContent		= new GUIContent("Remote Notifications", "Received remote notifications");

		#endregion

		#region Unity Methods

		private void OnEnable ()
		{
			m_serializableProperties	= serializedObject.GetSerializableProperties();
		}

		public override void OnInspectorGUI ()
		{
			// Update 
			serializedObject.Update();

			// Make all EditorGUI look like regular controls
			EditorGUIUtility.LookLikeControls();
			
			// Draw properties
			EditorGUILayout.BeginVertical(UnityEditorUtility.kOuterContainerStyle);
			{
				if (GUILayout.Button("Push Remote Notification"))
					EditorApplication.ExecuteMenuItem(Menu.kPushNotificationServiceMenuItem);

				// Draw all serializable properties
				foreach (SerializedProperty _property in m_serializableProperties)
					UnityEditorUtility.DrawPropertyField(_property);
			
				// Local notifications
				LayoutLocalNotifications();

				// Remote notifications
				LayoutRemoteNotifications();
			}
			EditorGUILayout.EndVertical();

			// Apply modified values
			if (GUI.changed)		
				serializedObject.ApplyModifiedProperties();
		}

		#endregion

		#region Local Notifications

		private void LayoutLocalNotifications ()
		{
			// Show received notifications
			EditorGUILayout.BeginVertical(UnityEditorUtility.kContainerStyle);
			{
				m_showLocalNotifications = UnityEditorUtility.DrawHeader(m_localNotificationsGUIContent, m_showLocalNotifications);

				if (m_showLocalNotifications)
				{
					DrawReceivedNotifications(NotificationCenter.LocalNotifications, (_selectedNotification)=>{
						if (_selectedNotification != null)
						{
							NotificationCenter.OnTappingLocalNotification(_selectedNotification);
						}
					});
				}
			}
			EditorGUILayout.EndVertical();
		}

		#endregion

		#region Remote Notifications

		private void LayoutRemoteNotifications ()
		{
			// Show received notifications
			EditorGUILayout.BeginVertical(UnityEditorUtility.kContainerStyle);
			{
				m_showRemoteNotifications = UnityEditorUtility.DrawHeader(m_remoteNotificationsGUIContent, m_showRemoteNotifications);
				
				if (m_showRemoteNotifications)
				{
					DrawReceivedNotifications(NotificationCenter.RemoteNotifications, (_selectedNotification)=>{
						if (_selectedNotification != null)
						{
							NotificationCenter.OnTappingRemoteNotification(_selectedNotification);
						}
					});
				}
			}
			EditorGUILayout.EndVertical();
		}

		#endregion

		#region Misc. Methods

		private void DrawReceivedNotifications (IList _notificationList, System.Action<CrossPlatformNotification> _callbackOnTap)
		{
			if (_notificationList == null || _notificationList.Count == 0)
				return;

			EditorGUILayout.BeginVertical();
			{
				foreach (CrossPlatformNotification _notification in _notificationList)
				{
					if (GUILayout.Button(_notification.AlertBody))
					{
						if (_callbackOnTap != null)
						{
							// Set editor as dirty
							EditorUtility.SetDirty(target);

							// Send callback
							_callbackOnTap(_notification);
						}

						break;
					}
				}
			}
			EditorGUILayout.EndVertical();
		}

		#endregion
	}
}
