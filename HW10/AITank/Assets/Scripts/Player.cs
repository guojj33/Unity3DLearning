using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AITANKGAME;

public class Player : MonoBehaviour {
    public BloodRecorder bloodRecorder;
    private Controller controller;
    public Canvas canvas;

    void Awake() {
        canvas.worldCamera = Camera.main;    
    }

    void Start()
    {
        controller = SSDirector.getInstance().CurrentSceneController as Controller;
        bloodRecorder = this.GetComponent<BloodRecorder>();
    }

    void Update()
    {
        if(controller.GetGameState() == GameState.Play) {
            if (bloodRecorder.getBlood() <= 0) {
                this.gameObject.SetActive(false);
                Singleton<GameEventManager>.Instance.PlayerDead();
            }
        }
    }

    public void reset()
    {
        transform.position = new Vector3(0, 0, 0);
        bloodRecorder.reset();
    }
}