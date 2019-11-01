using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolFactory : MonoBehaviour
{
    public GameObject patrol = null;
    private List<PatrolData> used = new List<PatrolData>(); // 正在使用的巡逻兵

    public List<GameObject> GetPatrols() {
        List<GameObject> patrols = new List<GameObject>();
        float[] pos_x = {-5f, 5f};
        float[] pos_z = {5f, -5f};
        for (int i = 0; i < 2; i++) {
            for (int j = 0; j < 2; j++) {
                patrol = Instantiate(Resources.Load<GameObject>("prefabs/patrol"));
                patrol.transform.position = new Vector3(pos_x[j], 0, pos_z[i]);
                patrol.GetComponent<PatrolData>().patrolRegion = i * 2 + j + 1;
                patrol.GetComponent<PatrolData>().playerRegion = 4;
                patrol.GetComponent<PatrolData>().isPlayerInRange = false;
                patrol.GetComponent<PatrolData>().isFollowing = false;
                patrol.GetComponent<PatrolData>().isCollided = false;
                patrol.GetComponent<Animator>().SetBool("pause", true);
                used.Add(patrol.GetComponent<PatrolData>());
                patrols.Add(patrol);
            }
        }
        return patrols;
    }

    public void PausePatrol() {
        //切换所有侦查兵的动画
        for (int i = 0; i < used.Count; i++) {
            used[i].gameObject.GetComponent<Animator>().SetBool("pause", true);
        }
    }

    public void StartPatrol() {
        //切换所有侦查兵的动画
        for (int i = 0; i < used.Count; i++) {
            used[i].gameObject.GetComponent<Animator>().SetBool("pause", false);
        }
    }
}