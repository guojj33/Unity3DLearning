using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using AITANKGAME;

public class BloodRecorder : MonoBehaviour
{
    public int MAX_BLOOD = 100;
    public int blood;
    public float oldHealth;
    public Slider healthBar;
    private Controller controller;

    void Start() {
        controller = SSDirector.getInstance().CurrentSceneController as Controller;
        blood = MAX_BLOOD;
        healthBar.value = (float)blood/(float)MAX_BLOOD;
        oldHealth = (float)blood/(float)MAX_BLOOD;
    }

    void OnGUI() {
        //更新血条
        if(controller.GetGameState() == GameState.Play && blood >= 0) {
            float health = (float)blood/(float)MAX_BLOOD;
            health = Mathf.Lerp(oldHealth, health, 0.05f);
            //Debug.Log(transform.gameObject.name+" health"+health);
            healthBar.value = health;
            oldHealth = health;
        }
    }

    public void reduceBlood(int i) {
        blood-=i;
    }

    public int getBlood() {
        return blood;
    }

    public void reset() {
        blood = MAX_BLOOD;
    }
}
