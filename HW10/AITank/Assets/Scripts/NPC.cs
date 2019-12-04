using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine;
using AITANKGAME;

public class NPC : MonoBehaviour {
    public BloodRecorder bloodRecorder;
    public Vector3 target;
    public int MIN_SHOOT_DISTANCE = 100;
    private IEnumerator shootCoroutine;
    private Controller controller;
    public Canvas canvas;
    void Start()
    {
        controller = SSDirector.getInstance().CurrentSceneController as Controller;
        shootCoroutine = shoot(1);
        StartCoroutine(shootCoroutine);
        bloodRecorder = this.GetComponent<BloodRecorder>();
        canvas.worldCamera = Camera.main;
    }

    void Update()
    {
        if(controller.GetGameState() == GameState.Play) {
            if (bloodRecorder.getBlood() <= 0) {
                this.gameObject.SetActive(false);
                Singleton<GameEventManager>.Instance.NPCDead(this.gameObject);
            }
            else {
                target = controller.getPlayerPosition();
                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                agent.SetDestination(target);
            }
        }
    }

    IEnumerator shoot(int interval) {
        while(controller.GetGameState() == GameState.Play) {
            yield return new WaitForSeconds(interval);
            if (Vector3.Distance(transform.position, target) < MIN_SHOOT_DISTANCE) {
                Factory factory = Singleton<Factory>.Instance;
                GameObject bullet = factory.getBullet("NPC");
                bullet.transform.position = new Vector3(transform.position.x, 1.2f, transform.position.z) + transform.forward*3.804f;
                bullet.GetComponent<Bullet>().playFire();
                bullet.transform.forward = transform.forward;
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.AddForce(bullet.transform.forward * 150, ForceMode.Impulse);
            }
        }
    }
    public void reset()
    {
        bloodRecorder.reset();
        shootCoroutine = shoot(1);
        StartCoroutine(shootCoroutine);
        Debug.Log("NPC reset");
    }
}