using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour 
{
	private static LevelLoader _instance = null;

	public static LevelLoader Instance 
	{
		get 
		{
			if (_instance == null) 
			{
				_instance = (new GameObject("LevelLoader", typeof(LevelLoader))).GetComponent<LevelLoader>();
			}
			return _instance;
		}
	}

	void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
	}

	public void LoadLevel(int levelNumber, System.Action callback = null)
	{
		StartCoroutine("LoadLevelCoroutine", levelNumber);
	}

	public void LoadLevel(string levelName, System.Action callback = null)
	{
		StartCoroutine("LoadLevelCoroutine", levelName);
	}

	IEnumerator LoadLevelCoroutine(int levelNumber)
	{
		AsyncOperation async = Application.LoadLevelAsync(levelNumber);
		yield return async;
		Debug.Log("Loading complete");
	}

	IEnumerator LoadLevelCoroutine(string levelName)
	{
		AsyncOperation async = Application.LoadLevelAsync(levelName);
		yield return async;
		Debug.Log("Loading complete");
	}

}
