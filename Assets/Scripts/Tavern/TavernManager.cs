using UnityEngine;
using System.Collections;

public class TavernManager : MonoBehaviour
{

	public void ReturnToMenu()
	{
		LevelLoader.Instance.LoadLevel(Scene.Lobby);
	}
}
