using UnityEngine;
using System.Collections;

public class CenterTest : MonoBehaviour 
{
	void OnGUI()
	{
		if (GUILayout.Button("Authenticate"))
		{
//			NPBinding.GameServices.LocalUser.Authenticate((bool _success)=>
//          	{
//				signingIn = false;
//				if (_success)
//				{
//					Debug.Log("Sign-In Successfully");
//					Debug.Log("Local User Details : " + NPBinding.GameServices.LocalUser.ToString());
//				}
//				else
//				{
//					Debug.Log("Sign-In Failed");
//				}
//			});
			UnityEngine.Social.localUser.Authenticate((bool success) => {});
		}
		if (UnityEngine.Social.localUser.authenticated)
		{
			if (GUILayout.Button("ShowLeaderboardUI"))
			{
				UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform.ShowLeaderboardUI(CONST.IOS_LEADERBOARD_ID, UnityEngine.SocialPlatforms.TimeScope.Today);
			}
		}
	}
}
