using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TavernManager : BaseSingleton<TavernManager>
{
	[SerializeField] Transform background;
	[SerializeField] ScrollArea scrollArea;
	[SerializeField] WallPart tavernPartPrefab;

	[SerializeField] List<WallPart> walls;
	[SerializeField] List<Character> characters;



	protected override void Awake()
	{
		walls = new List<WallPart>();
		characters = new List<Character>();

		base.Awake();
	}

	void Start()
	{
		StartCoroutine("LoadCharacters");
	}

	IEnumerator LoadCharacters()
	{
		ResourceRequest request;
		int number = 0;

		foreach (string name in CONST.PLAYER_NAMES)
		{
			if (number < 2)
			{
				Object obj = Resources.Load(string.Format("{0}{1}{2}", CONST.TAVERN_PLAYERS_PATH, name, CONST.TAVERN_PREFIX));

				AddPart(obj);
			}
			else
			{
				request = Resources.LoadAsync(string.Format("{0}{1}{2}", CONST.TAVERN_PLAYERS_PATH, name, CONST.TAVERN_PREFIX));
				yield return request;
				
				if (request.isDone)
				{
					AddPart(request.asset);
				}
			}

			number++;


		}

		//Add empty right wall
		AddWall(false);

		yield return null;

		Debug.Log("Done loading characters");
	}


	void AddPart(Object obj)
	{
		GameObject characterGameObject = (GameObject)obj;
		if (characterGameObject != null)
		{
			WallPart wallPart = AddWall(true);
			AddCharacter(wallPart, characterGameObject);
		}
	}

	WallPart AddWall(bool changeScroll)
	{
		GameObject go = GameObject.Instantiate<GameObject>(tavernPartPrefab.gameObject);
		go.transform.parent = background;

		Vector3 position = Vector3.zero;
		if (walls.Count > 0)
		{
			position = walls[walls.Count-1].transform.localPosition;
			position.x += walls[walls.Count-1].wallSize;
		}
		go.transform.localPosition = position;

		WallPart part = go.GetComponent<WallPart>();

		if (part != null)
		{
			Vector3 center = part.bounds.center;
			center.x = part.boundsExtendsX * walls.Count;
			part.bounds.center = center;
			walls.Add(part);

			if (changeScroll)
			{
				scrollArea.AddPoint(part.bounds);

				Bounds area = scrollArea.GetScrollArea();
				//Change center
				center = area.center;
				center.x = (walls.Count - 1) * part.wallSize / 2;
				area.center = center;

				//Change width
				center = area.extents;
				center.x = area.center.x + 0.5f;
				area.extents = center;

				scrollArea.ChangeScrollArea(area);
			}
		}
		else
		{
			Debug.LogWarning("WallPart == null!");
		}

		return part;
	}

	Character AddCharacter(WallPart wallPart, GameObject obj)
	{
		GameObject go = GameObject.Instantiate<GameObject>(obj);
		go.transform.parent = wallPart.characterPosition;
		go.transform.localPosition = Vector3.zero;

		Character character = go.GetComponent<Character>();

		if (character != null)
		{
			characters.Add(character);
		}
		else
		{
			Debug.LogWarning("Character == null!");
		}

		return character;
	}

	public void SelectCharacter(Character character)
	{
		Debug.Log("Select " + character.number);

		GeneralGameController.Instance.SelectCharacter(character);
	}

	public void ReturnToMenu()
	{
		LevelLoader.Instance.LoadLevel(Scene.Lobby);
	}
}
