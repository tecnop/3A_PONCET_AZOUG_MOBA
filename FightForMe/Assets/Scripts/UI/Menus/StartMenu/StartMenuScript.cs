using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions; //<- Regexp ! hehehe

public class StartMenuScript : MonoBehaviour
{
	private Animator planeAnimator;

	private bool waitingForPlayer;

	private string ipAddress = "";

	private float mainScreenLeft;
	//private float mainScreenCurrentTop;

	private float boxMultiWidth;
	private float boxMultiHeight;

	private float boxMonoWidth;
	private float boxMonoHeight;

	private float boxQuitWidth;
	private float boxQuitHeight;

	private GameType gameType;

	void Start()
	{
		Application.runInBackground = true;
		ipAddress = PlayerPrefs.GetString("ipAddress");
		GameData.networkError = NetworkConnectionError.NoError;

		GameData.secure = true;

		waitingForPlayer = false;

		boxMultiWidth = 250f;
		boxMultiHeight = 400f;

		boxMonoWidth = 250f;
		boxMonoHeight = 400f;

		boxQuitWidth = 250f;
		boxQuitHeight = 200f;

		mainScreenLeft = (Screen.width / 2) - (boxMultiWidth / 2);
		//mainScreenCurrentTop = (Screen.height / 4); //- (mainContHeight / 2);

		planeAnimator = this.GetComponent<Animator>();

		if (planeAnimator)
		{
			planeAnimator.SetBool("started", true);
		}

	}

	void OnGUI()
	{

		if (planeAnimator.GetBool("launchGame"))
		{
			return;
		}

		if (!waitingForPlayer)
		{
			drawMainMenu();
		}
		else
			drawLobby();
	}

	private void drawMainMenu()
	{

		GUILayout.BeginArea(new Rect(mainScreenLeft, (Screen.height / 6), boxMultiWidth, boxMultiHeight));

		GameData.gameMode = (GameMode)GUILayout.SelectionGrid((int)GameData.gameMode, new string[] { "Suprématie", "Course à la Gloire" }, 2);
		GameData.secure = GUILayout.Toggle(GameData.secure, "Mode sécurisé"); // Doesn't look so good

		//if (GameData.secure)
		{
			GUILayout.Label("Multijoueur");

			if (GUILayout.Button("Héberger"))
			{
				gameType = GameType.ListenServer;
				//waitingForPlayer = true;
				planeAnimator.SetBool("launchGame", true);
			}

			if (GUILayout.Button("Rejoindre"))
			{
				if (Regex.IsMatch(ipAddress, "^[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}$"))
				{ // <- Fixer cette putain de regex que c-sharp ne gere pas bien
					PlayerPrefs.SetString("ipAddress", ipAddress);
					gameType = GameType.Client;
					planeAnimator.SetBool("launchGame", true);
				}
				else
					planeAnimator.Play("Angry");
			}

			ipAddress = GUILayout.TextField(ipAddress, 25);
		}

		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect(mainScreenLeft, (float)(Screen.height / 2.5f), boxQuitWidth, boxQuitHeight));

		GUILayout.Label("Un joueur");

		if (GUILayout.Button("Partie contre une IA"))
		{
			gameType = GameType.Local;
			planeAnimator.SetBool("launchGame", true);
		}

		if (GUILayout.Button("Tutoriel"))
		{
			//gameType = GameType.Local;
			//planeAnimator.SetBool("launchGame", true);
		}

		GUILayout.EndArea();

		GUILayout.BeginArea(new Rect(mainScreenLeft, (float)(Screen.height / 1.5f), boxMonoWidth, boxMonoHeight));

		if (GUILayout.Button("Quitter"))
		{

			planeAnimator.SetBool("quitGame", true);

		}

		GUILayout.EndArea();
	}

	private void drawLobby()
	{
		GUILayout.BeginArea(new Rect(mainScreenLeft, (Screen.height / 6), boxMultiWidth, boxMultiHeight));
		GUILayout.Label("En attente d'un autre joueur ...");
		if (GUILayout.Button("Revenir"))
		{
			planeAnimator.Play("Angry");
			waitingForPlayer = false;
		}

		// Insert condition for run the game
		planeAnimator.SetBool("launchGame", true); // <- Load the game (it'll fire "startGame" event)

		GUILayout.EndArea();
	}

	void startGame()
	{
		GameData.wentThroughMenu = true;
		GameData.gameType = gameType;
		Application.LoadLevel("Lobby");
	}

	void quit()
	{
		Application.Quit();
	}

}

