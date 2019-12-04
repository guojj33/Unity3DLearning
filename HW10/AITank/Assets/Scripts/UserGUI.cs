using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITANKGAME;

public class UserGUI : MonoBehaviour
{
    IUserAction action;
    GUIStyle labelStyle;
    GUIStyle buttonStyle;

    void Start() {
        labelStyle = new GUIStyle("label");
        labelStyle.alignment = TextAnchor.MiddleCenter;
        labelStyle.fontSize = 20;
        labelStyle.normal.textColor = Color.white;

        buttonStyle = new GUIStyle("button");
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle.fontSize = 20;
        buttonStyle.normal.textColor = Color.white;

        action = SSDirector.getInstance().CurrentSceneController as IUserAction;
    }

    void Update() {
        if (action.GetGameState() == GameState.Play) {
            // 获取键盘输入
            float translationX = Input.GetAxis("Horizontal");
            float translationZ = Input.GetAxis("Vertical");
            //移动玩家
            action.MovePlayer(translationX, translationZ);
            if(Input.GetKeyDown(KeyCode.Return)) {
                action.PlayerShoot();
            }
        }
    }

    void OnGUI() {        
        string buttonText = "";
        if (action.GetGameState() == GameState.Start) {
            buttonText = "Start";
        }
        else if (action.GetGameState() == GameState.Lose || action.GetGameState() == GameState.Win) {
            buttonText = "Restart";
        }

        if(action.GetGameState() == GameState.Win) {
            GUI.Label(new Rect(Screen.width/2 - Screen.width/8, Screen.height/2 - Screen.height/8, Screen.width/4, Screen.height/4), "You Win!",labelStyle);
        }
        else if(action.GetGameState() == GameState.Lose) {
            GUI.Label(new Rect(Screen.width/2 - Screen.width/8, Screen.height/2 - Screen.height/8, Screen.width/4, Screen.height/4), "You Lose!",labelStyle);
        }

        if(buttonText != "") {
            if (GUI.Button(new Rect(Screen.width/2 - Screen.width/16, Screen.height/2 - Screen.height/8 + Screen.height/4, Screen.width/8, Screen.height/8), buttonText, buttonStyle)) {
                if (buttonText == "Start") {
                    action.StartGame();
                } else if (buttonText == "Restart") {
                    action.RestartGame();
                }
            }
        }
    }
}
