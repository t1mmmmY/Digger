using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour 
{
	public static System.Action OnLogoComplite;

	public void EndLogoAnimation()
	{
		if (OnLogoComplite != null)
		{
			OnLogoComplite();
		}
	}

}
