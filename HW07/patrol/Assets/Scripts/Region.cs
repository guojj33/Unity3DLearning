using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatrolGAME;

public class Region : MonoBehaviour
{
    public int region;
    Controller sceneController;

    void OnTriggerEnter(Collider collider)
    {
        sceneController = SSDirector.getInstance().CurrentSceneController as Controller;
        if (collider.gameObject.tag == "Player")
        {
            Debug.Log("player enter area " + region);
            sceneController.playerRegion = region;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Patrol")
        {
            Debug.Log("area " + region + ": patrol try to exit");
            collider.gameObject.GetComponent<PatrolData>().isCollided = true;
        }
    }
}