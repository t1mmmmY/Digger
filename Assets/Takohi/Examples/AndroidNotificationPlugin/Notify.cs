using System;
using UnityEngine;

public class Notify : MonoBehaviour {
	public const int NotificationID = 1;
	
	public Texture2D LargeIcon;
	
	private string _contentTitle, _contentText;
	private bool _sticky = false;
	private int _number = 1;
	
	void Start() {
		_contentTitle = "Hello there!";
		_contentText = "I have no idea what I'm doing.";
	}
	
	void OnGUI() {
		GUI.skin.label.fontStyle = FontStyle.Bold;
		GUI.skin.button.fontSize = 40;
		GUI.skin.textField.fontSize = 40;
		GUI.skin.textArea.fontSize = 40;

		GUI.skin.label.fontSize = 45;
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height * 0.2f), "Android Notification Plugin\nFor Unity, by Takohi");
		
		GUI.skin.label.fontSize = 30;
		GUI.skin.label.alignment = TextAnchor.LowerCenter;
		Rect layoutRect = new Rect(Screen.width * 0.05f, Screen.height * 0.2f, Screen.width * 0.9f, Screen.height * 0.6f);
		GUILayout.BeginArea (layoutRect, GUI.skin.box);
		
		GUILayout.BeginVertical();
		
		_contentTitle = GUILayout.TextField(_contentTitle);
		GUILayout.Space(25f);
		_contentText = GUILayout.TextArea(_contentText, GUILayout.Height(200f));
		_sticky = GUILayout.Toggle (_sticky, "Sticky notification");
		
		GUILayout.FlexibleSpace();
		
		if(GUILayout.Button("Show Notification")) {
			Notification notification = PrepareNotification ();
			NotificationManager.ShowNotification(NotificationID, notification);
			++_number;
		}

		if(GUILayout.Button("Show Notification with 10s delay")) {
			Notification notification = new Notification("octopus", "1", "1");
			NotificationManager.ShowNotification(1, notification, 5000);
		
			notification = new Notification("octopus", "2", "2");
			NotificationManager.ShowNotification(2, notification, 10000);

		}
		GUILayout.Label ("Notification will be displayed even when the game is closed.");
		
		GUILayout.Space(35f);
		
		if(GUILayout.Button("Cancel Notification")) {
			NotificationManager.Cancel(NotificationID);
		}
		 
		GUILayout.EndVertical();
		
		GUILayout.EndArea();
	}

	private Notification PrepareNotification() {
		/*
		 * Unfortunately, from Unity 5.0, providing Android resources became obsolete.
		 * Before Unity 5.0, you can provide the name of the drawable in the folder Plugins/Android/res/drawable.
		 */
		Notification notification = new Notification("octopus", _contentTitle, _contentText);
		notification.SetContentInfo("By Takohi");
		notification.EnableSound(true);

		/*
		 * Requires VIBRATE permission.
		 */ 
		notification.SetVibrate(new long[] {200, 100, 100, 200, 200, 100});

		/*
		 * Lights or LED notification are only working when screen is off.
		 */
		notification.SetLights(new Color32(255, 0, 0, 255), 500, 500);

		/*
		 * If you pass a texture, it has to be readable. 
		 * Tick Read/Write Enabled option for the Texture in the inspector
		 */ 
		notification.SetLargeIcon(LargeIcon);

		if(_number > 1)
			notification.SetNumber(_number);

		notification.SetSticky(_sticky);

		return notification;
	}
}
