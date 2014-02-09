using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions; //<- Regexp ! hehehe

public class StartMenuScript : MonoBehaviour {
	
	[SerializeField]
	string _MainSceneName;
	
	private Animator planeAnimator;

	private bool waitingForPlayer;

	private string ipAddress = "";

	private float mainScreenLeft;
	private float mainScreenCurrentTop;
	
	private float boxMultiWidth;
	private float boxMultiHeight;
	
	private float boxMonoWidth;
	private float boxMonoHeight;
	
	private float boxQuitWidth;
	private float boxQuitHeight;
	
	void Start()
	{
		// Reseting preferences :
		PlayerPrefs.SetInt ("isBotGame", 0);  // < - TO DELETE / REPLACE
		PlayerPrefs.SetInt ("isServer", 0);  // < - TO DELETE / REPLACE
		PlayerPrefs.SetString ("ipAddress", "");  // < - TO DELETE / REPLACE

		waitingForPlayer = false;

		boxMultiWidth = 250f;
		boxMultiHeight = 400f;
		
		boxMonoWidth = 250f;
		boxMonoHeight = 400f;
		
		boxQuitWidth = 250f;
		boxQuitHeight = 200f;
		
		mainScreenLeft = (Screen.width / 2) - (boxMultiWidth / 2);
		mainScreenCurrentTop = (Screen.height / 4); //- (mainContHeight / 2);

		planeAnimator = this.GetComponent<Animator>();

		if(planeAnimator){
			planeAnimator.SetBool("started", true);
		}
		
	}
	
	void OnGUI () {
		
		if (!waitingForPlayer) {
			drawMainMenu();
		}else
			drawLobby();
	}

	private void drawMainMenu()
	{

		GUILayout.BeginArea (new Rect (mainScreenLeft, (Screen.height / 6), boxMultiWidth, boxMultiHeight));

		GUILayout.Label ("Multi");
		
		if (GUILayout.Button("Héberger")) {
			PlayerPrefs.SetInt("isServer", 1); // < - TO DELETE / REPLACE
			waitingForPlayer = true;
			//planeAnimator.SetBool("launchGame", true);
		}
		
		if(GUILayout.Button("Rejoindre")) {
			if(Regex.IsMatch(ipAddress, "^[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}$")){ // <- Fixer cette putain de regex que c-sharp ne gere pas bien
				PlayerPrefs.SetString("ipAddress", ipAddress); // < - TO DELETE / REPLACE
				planeAnimator.SetBool("launchGame", true);
			}else
				planeAnimator.Play("Angry");
		}
		
		ipAddress = GUILayout.TextField (ipAddress, 25);
		
		GUILayout.EndArea ();
		
		GUILayout.BeginArea (new Rect (mainScreenLeft,(float)(Screen.height / 2.5f), boxQuitWidth, boxQuitHeight));
		
		GUILayout.Label ("Mono");
		
		if(GUILayout.Button("Jouer")) {
			PlayerPrefs.SetInt("isBotGame", 1); // < - TO DELETE / REPLACE
			planeAnimator.SetBool("launchGame", true);
		}
		
		GUILayout.EndArea ();
		
		GUILayout.BeginArea (new Rect (mainScreenLeft, (float)(Screen.height / 1.5f), boxMonoWidth, boxMonoHeight));
		
		if(GUILayout.Button("Quitter")) {
			
			planeAnimator.SetBool("quitGame", true);
			
		}
		
		GUILayout.EndArea ();
	}

	private void drawLobby()
	{
		GUILayout.BeginArea (new Rect (mainScreenLeft, (Screen.height / 6), boxMultiWidth, boxMultiHeight));
		GUILayout.Label ("En attente d'un autre joueur ...");
		if(GUILayout.Button("Revenir")) {
			planeAnimator.Play("Angry");
			waitingForPlayer = false;
		}

		// Insert condition for run the game
		// planeAnimator.SetBool("launchGame", true); <- Load the game (it'll fire "startGame" event)

		GUILayout.EndArea ();
	}

	void startGame()
	{
		Application.LoadLevel (_MainSceneName);
	}
	
	void quit()
	{
		Application.Quit();
	}
	
}

