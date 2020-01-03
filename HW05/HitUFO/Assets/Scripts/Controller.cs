using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitUFOGAME;

public class Controller : MonoBehaviour, ISceneController, IUserAction {
    public ScoreRecorder scoreRecorder;
    public BloodRecorder bloodRecorder;
    public MyActionManager actionManager;
    public UserGUI userGUI;
    public DiskFactory diskFactory;
    public Queue<GameObject> diskQueue = new Queue<GameObject> ();
    public List<GameObject> diskMissed = new List<GameObject> ();

    //private int diskNumber = 10; //一回合发射飞碟数
    private int currentRound = 1;
    public int maxRound = 3;
    private int scoreRound2 = 20;
    private int scoreRound3 = 45;
    private float interalTime = 1.3f;
    private bool isGamePlaying = false;
    private bool isGameOver = false;
    private bool isGameStarted = false;

    void Start () {
        SSDirector director = SSDirector.getInstance ();
        director.CurrentSceneController = this;
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;
        this.gameObject.AddComponent<ScoreRecorder> ();
        this.gameObject.AddComponent<BloodRecorder> ();
        this.gameObject.AddComponent<DiskFactory> ();
        scoreRecorder = Singleton<ScoreRecorder>.Instance;
        bloodRecorder = Singleton<BloodRecorder>.Instance;
        diskFactory = Singleton<DiskFactory>.Instance;
        actionManager = gameObject.AddComponent<MyActionManager>() as MyActionManager;
    }

    void Update () {
        if (isGameStarted) {
            if (isGameOver) {
                CancelInvoke("LoadResources");
                return;
            }
            if (!isGamePlaying) {
                InvokeRepeating ("LoadResources", 1f, interalTime);
                isGamePlaying = true;
            }
            SendDisk ();
            if (scoreRecorder.score >= scoreRound2 && currentRound == 1) {
                currentRound = 2;
                interalTime -= 0.5F;
                CancelInvoke("LoadResources");
                isGamePlaying = false;
            }
            if (scoreRecorder.score >= scoreRound3 && currentRound == 2) {
                currentRound = 3;
                interalTime -= 0.3F;
                CancelInvoke("LoadResources");
                isGamePlaying = false;//
            }
        }
    }

    public void LoadResources () {
        diskQueue.Enqueue (diskFactory.GetDisk(currentRound));
    }

    private void SendDisk () {
        float position_x = 10;
        if (diskQueue.Count > 0) {
            GameObject disk = diskQueue.Dequeue ();
            diskMissed.Add(disk);
            float ran_y = Random.Range(3f, 5f);
            float ran_x = Random.Range(-1f, 1f) <= 0 ? -1F : 1F;//从左边出还是右边出
            disk.GetComponent<DiskData>().direction = new Vector3(ran_x, 0, 0);//
            Vector3 position = new Vector3(-ran_x * position_x, ran_y, 0);
            disk.transform.position = position;
            disk.SetActive (true);
            actionManager.throwDisk(disk);
        }

    }

    public void BeginGame () {
        isGameStarted = true;
    }

    public void GameOver () {
        isGameOver = true;
    }

    public void Restart () {
        isGameOver = false;
        isGamePlaying = false;
        scoreRecorder.Reset();
        bloodRecorder.Reset();
        currentRound = 1;
        interalTime = 2f;
    }

    public int GetScore () {
        return scoreRecorder.score;
    }

    public int GetRound () {
        return currentRound;
    }

    public int GetBlood () {
        return bloodRecorder.blood;
    }

    public void hit (Vector3 pos) {
        if (isGameOver == true)
            return;
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        bool not_hit = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            //射线打中物体
            if (hit.collider.gameObject.GetComponent<DiskData>() != null) {
                //射中的物体要在没有打中的飞碟列表中
                for (int j = 0; j < diskMissed.Count; j++) {
                    if (hit.collider.gameObject.GetInstanceID() == diskMissed[j].gameObject.GetInstanceID()) {
                        not_hit = true;
                    }
                }
                if(!not_hit) {
                    return;
                }
                Debug.Log("hit: " + hit.collider.gameObject.name);
                diskMissed.Remove (hit.collider.gameObject);
                scoreRecorder.Record (hit.collider.gameObject);
                hit.collider.gameObject.transform.position = new Vector3(0, -9, 0);
                break;
            }
        }

    }

}