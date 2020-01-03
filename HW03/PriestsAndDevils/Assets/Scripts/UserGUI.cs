using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using myGame;

public class UserGUI : MonoBehaviour {
	public int guiFlag = 1;	//0 for restart, 1 for continue, 2 for win, 3 for lose, used to communicate with the controller
	// Use this for initialization
	void Start () {
	}

	void OnGUI() {
		Debug.Log (guiFlag);
		GUIStyle textStyle = new GUIStyle ();
		GUIStyle buttonStyle = new GUIStyle ();
		textStyle.fontSize = 30;
		buttonStyle.fontSize = 15;
		if (GUI.Button (new Rect (Screen.width / 2 - 60, Screen.height / 2 + 150, 100, 50), "Restart")) {
			guiFlag = 0;
		}
		if (guiFlag == 2) GUI.Label (new Rect (Screen.width / 2 - 60, Screen.height / 2 + 60, 200, 50), "You Win!\nPress Restart to play again");
		if (guiFlag == 3) GUI.Label (new Rect (Screen.width / 2 - 60, Screen.height / 2 + 60, 200, 50), "You Lose!\nPress Restart to play again");
	}
}
