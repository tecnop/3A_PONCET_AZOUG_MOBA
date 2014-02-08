using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions; //<- Regexp ! hehehe

public class StartMenuScript : MonoBehaviour {
	
	[SerializeField]
	string _MainSceneName;
	
	private Animator planeAnimator;
	
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
		PlayerPrefs.SetInt ("isBotGame", 0);
		PlayerPrefs.SetInt ("isServer", 0);
		PlayerPrefs.SetString ("ipAddress", "");

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
		
		GUILayout.BeginArea (new Rect (mainScreenLeft, 100f, boxMultiWidth, boxMultiHeight));
		
		GUILayout.Label ("Multi");
		
		if (GUILayout.Button("Héberger")) { // GUILayout.Height(100)
			PlayerPrefs.SetInt("isServer", 1);
			planeAnimator.SetBool("launchGame", true);
		}
		
		if(GUILayout.Button("Rejoindre")) {
			if(Regex.IsMatch(ipAddress, "^[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}$")){ // <- Fixer cette putain de regex que c-sharp ne gere pas bien
				PlayerPrefs.SetString("ipAddress", ipAddress);
				planeAnimator.SetBool("launchGame", true);
			}else
				planeAnimator.Play("Angry");
		}
		
		ipAddress = GUILayout.TextField (ipAddress, 25);
		
		GUILayout.EndArea ();
		
		GUILayout.BeginArea (new Rect (mainScreenLeft, mainScreenCurrentTop + 100f, boxQuitWidth, boxQuitHeight));
		
		GUILayout.Label ("Mono");
		
		if(GUILayout.Button("Jouer")) {
			PlayerPrefs.SetInt("isBotGame", 1);
			planeAnimator.SetBool("launchGame", true);
		}
		
		GUILayout.EndArea ();
		
		GUILayout.BeginArea (new Rect (mainScreenLeft, mainScreenCurrentTop + 200f, boxMonoWidth, boxMonoHeight));
		
		if(GUILayout.Button("Quitter")) {
			
			planeAnimator.SetBool("quitGame", true);
			
		}
		
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

