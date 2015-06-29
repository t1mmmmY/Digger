using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TavernManager : BaseSingleton<TavernManager>
{
	[SerializeField] Transform background;
	[SerializeField] ScrollArea scrollArea;
	[SerializeField] WallPart tavernPartPrefab;
	[SerializeField] Text characterNameLabel;
	[SerializeField] Button buyCharacterButton;
	[SerializeField] Button playCharacterButton;
	[SerializeField] Animator canvasAnimator;

	List<WallPart> walls;
	List<Character> characters;

	int showDescriptionHash = Animator.StringToHash("ShowDescription");
	int hideDescriptionHash = Animator.StringToHash("HideDescription");
	int selectCharacterHash = Animator.StringToHash("SelectCharacter");

	protected override void Awake()
	{
		walls = new List<WallPart>();
		characters = new List<Character>();

		base.Awake();
	}

	void Start()
	{
		StartCoroutine("LoadCharacters");
		ShowDescription(0);
	}

	void OnEnable()
	{
		ScrollArea.onChangePosition += OnChangePosition;
		ScrollArea.onStartMoving += OnStartMoving;
		ScrollArea.onEndMoving += OnEndMoving;

	}

	void OnDestroy()
	{
		ScrollArea.onChangePosition -= OnChangePosition;
		ScrollArea.onStartMoving -= OnStartMoving;
		ScrollArea.onEndMoving -= OnEndMoving;
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


	void OnStartMoving()
	{
//		Debug.Log("HideDescription");
		HideDescription();
	}

	void OnEndMoving()
	{
//		ShowDescription();
	}

	void OnChangePosition(int newPositionNumber)
	{
//		Debug.Log("OnChangePosition");
		ShowDescription(newPositionNumber);
	}

	void HideDescription()
	{
		canvasAnimator.SetTrigger(hideDescriptionHash);
	}

	void ShowDescription(int positionNumber)
	{
		switch (PlayerStatsController.Instance.GetStatus(positionNumber))
		{
		case PlayerStatus.NotBought:
			buyCharacterButton.gameObject.SetActive(true);
			playCharacterButton.gameObject.SetActive(false);
			break;
		case PlayerStatus.Bought:
			buyCharacterButton.gameObject.SetActive(false);
			playCharacterButton.gameObject.SetActive(true);
			break;
		}

		characterNameLabel.text = CONST.DESCRIPTOIN_NAMES[positionNumber];

		AnimatorStateInfo stateInfo = canvasAnimator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.IsName("HideDescription"))
		{
			canvasAnimator.SetTrigger(showDescriptionHash);
		}
	}


	public void SelectCharacter(Character character)
	{
//		Debug.Log("Select " + character.number);
		PlayerStatus playerStatus = PlayerStatsController.Instance.GetStatus(character.number);
		switch (playerStatus)
		{
		case PlayerStatus.Bought:
			PlaySelectAnimation(character);
			if (GeneralGameController.Instance != null)
			{
				GeneralGameController.Instance.SelectCharacter(character);
			}
			break;
		case PlayerStatus.NotBought:

			PlayerStatsController.Instance.SetStatus(character.number, PlayerStatus.Bought);
			//Temp
			PlaySelectAnimation(character);
			if (GeneralGameController.Instance != null)
			{
				GeneralGameController.Instance.SelectCharacter(character);
			}
			break;
		}


	}

	public void ReturnToMenu()
	{
		LevelLoader.Instance.LoadLevel(Scene.Lobby);
	}

	void PlaySelectAnimation(Character character)
	{
		canvasAnimator.SetTrigger(selectCharacterHash);
		StartCoroutine("InvokeLoadLevel", 1.0f);
//		playCharacterButton.Select();
	}

	IEnumerator InvokeLoadLevel(float time)
	{
		yield return new WaitForSeconds(time);
		LevelLoader.Instance.LoadLevel(Scene.Lobby);
	}
}
