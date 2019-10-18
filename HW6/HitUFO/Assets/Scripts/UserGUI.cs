using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitUFOGAME;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;
    private int chooseMode = 0;
    private int start = 1;
    private int restart = 0;
	// Use this for initialization
	void Start () {
        action = SSDirector.getInstance().CurrentSceneController as IUserAction;
	}
 
     void OnGUI() {
		GUIStyle textStyle = new GUIStyle ();
		GUIStyle buttonStyle = new GUIStyle ();
		textStyle.fontSize = 30;
		buttonStyle.fontSize = 15;
        if (start == 1) {
            if (GUI.Button (new Rect (Screen.width / 2 - 60, Screen.height / 2 + 150, 100, 30), "Start")) 
            {
                chooseMode = 1;
                start = 0;
            }
        }
        if (chooseMode == 1) {
            if (GUI.Button(new Rect(Screen.width / 2 - 120, Screen.height / 2 + 150, 100, 30), "KINEMATIC"))
            {
                action.SetMode(ActionMode.KINEMATIC);
                action.BeginGame ();
                chooseMode = 0;
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 0, Screen.height / 2 + 150, 100, 30), "PHYSICS"))
            {
                action.SetMode(ActionMode.PHYSICS);
                action.BeginGame ();
                chooseMode = 0;
            }
        }
        if (action.GetBlood() > 0) {
            GUI.Label(new Rect(10, 5, 200, 50), "回合:");
            GUI.Label(new Rect(55, 5, 200, 50), action.GetRound().ToString());
            GUI.Label(new Rect(10, 25, 200, 50), "分数:");
            GUI.Label(new Rect(55, 25, 200, 50), action.GetScore().ToString());
            GUI.Label(new Rect(10, 45, 200, 50), "血量:");
            GUI.Label(new Rect(55, 45, 200, 50), action.GetBlood().ToString());
            if (Input.GetButtonDown("Fire1")) {
                    Vector3 pos = Input.mousePosition;
                    action.hit(pos);
                    Debug.Log("hit: " + pos);
            }
        }
        if (action.GetBlood() <= 0) {
            action.GameOver ();
            GUI.Label(new Rect(Screen.width / 2 - 60, Screen.height / 2 + 80, 100, 50), "You are dead!");
            GUI.Label(new Rect(Screen.width / 2 - 60, Screen.height / 2 + 100, 100, 50), "Your score: ");
            GUI.Label(new Rect(Screen.width / 2 + 10, Screen.height / 2 + 100, 100, 50), action.GetScore().ToString());
            if (GUI.Button (new Rect (Screen.width / 2 - 60, Screen.height / 2 + 190, 100, 30), "Restart")) {
			    action.Restart ();
                start = 1;
		    }
        }
    }
 
   
}
