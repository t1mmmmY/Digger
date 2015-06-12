using UnityEngine;
using System.Collections;

public class BaseSingleton<T> : MonoBehaviour where T : BaseSingleton<T>
{
	public static T Instance{ get; private set; }
	
	virtual protected void Awake()
	{
		Instance = this as T;
	}
	
	virtual protected void OnDestroy()
	{
		Instance = null;
	}
}
