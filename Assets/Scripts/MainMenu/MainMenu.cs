using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
	[SerializeField] Button[] allButtons;
	public uint timeout = 20;
	[SerializeField] Animator mainMenuAnimator;
    [SerializeField] Transform characterStartPosition;

	public static MainMenu Instance;


	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		SetActiveAllButtons(true);
        LoadCharacter();
	}

    private void LoadCharacter()
    {
        int characterNumber = 0;
        if (GeneralGameController.Instance != null)
        {
            characterNumber = GeneralGameController.Instance.characterNumber;
        }

        Object obj = Resources.Load(string.Format("{0}{1}", CONST.PLAYABLE_PLAYERS_PATH, CONST.PLAYER_NAMES[characterNumber]));
        if (obj != null)
        {
            GameObject characterGO = GameObject.Instantiate<GameObject>((GameObject)obj);
            characterGO.transform.parent = characterStartPosition;
            characterGO.transform.localPosition = Vector3.zero;

            //follower.SetTarget(characterGO.transform);
        }
        else
        {
            Debug.LogError("Cannot load character!");
        }
    }

	public void StartSingleGame()
	{
		SetActiveAllButtons(false);
		LevelLoader.Instance.LoadLevel(Scene.SinglePlayer);
	}

	public void StartMultiplayerGame()
	{
		SetActiveAllButtons(false);
		mainMenuAnimator.SetBool("ButtonsVisible", false);
		MultiplayerController.Instance.StartMatchMakingRealTime();
	}

	public void StartFastMultiplayerGame()
	{
		SetActiveAllButtons(false);
		mainMenuAnimator.SetBool("ButtonsVisible", false);
		MultiplayerController.Instance.StartMatchMakingRealTimeFast();
	}

	public void SignInToGoogle()
	{
		MultiplayerController.Instance.SignIn();
	}

	public void ShowLeaderboard()
	{
		MultiplayerController.Instance.ShowLeaderboard();
	}


	public void SetActiveAllButtons(bool isActive)
	{
		foreach (Button button in allButtons)
		{
			button.interactable = isActive;
		}
	}
	 

	public void StopTimer()
	{
		mainMenuAnimator.SetBool("ButtonsVisible", true);
		SetActiveAllButtons(true);
	}

	public void MoveToShop()
	{
//		SetActiveAllButtons(false);
//		mainMenuAnimator.SetBool("InShop", true);
	}

	public void ReturnToMenu()
	{
//		SetActiveAllButtons(true);
//		mainMenuAnimator.SetBool("InShop", false);
	}

	public void EnterTavern()
	{
		LevelLoader.Instance.LoadLevel(Scene.Tavern);
	}


}
