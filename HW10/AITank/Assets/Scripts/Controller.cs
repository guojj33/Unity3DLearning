using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITANKGAME;

public class Controller : MonoBehaviour, IUserAction, ISceneController
{
    public GameObject player;
    public GameState gameState;
    public Factory factory;
    public SSDirector director;
    public int MAX_NPC_COUNT = 3;
    public int NPCCount;
    public UserGUI userGUI;
    public GameEventManager gameEventManager;
    public float moveSpeed = 20f;
    public 

    void Start()
    {
        director = SSDirector.getInstance();
        director.CurrentSceneController = this;
        gameEventManager = gameObject.AddComponent<GameEventManager>() as GameEventManager;
        factory = gameObject.AddComponent<Factory>() as Factory;
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;
        gameState = GameState.Start;
        NPCCount = MAX_NPC_COUNT;
    }

    public void StartGame() {
        gameState = GameState.Play;
        LoadResources();
        CameraController cc = gameObject.AddComponent<CameraController>() as CameraController;
        cc.player = player;
    }

    public void RestartGame() {
        gameState = GameState.Play;
        NPCCount = MAX_NPC_COUNT;
        //重置 player
        player.SetActive(true);
        player.GetComponent<Player>().reset();
        //将 NPC 全部回收
        factory.reset();
        LoadNPCs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() {
        //订阅事件
        GameEventManager.OnNPCDead += OnNPCDead;
        GameEventManager.OnPlayerDead += OnPlayerDead;
    }

    void OnDisable() {
        GameEventManager.OnNPCDead -= OnNPCDead;
        GameEventManager.OnPlayerDead -= OnPlayerDead;
    }

    public void LoadResources() {
        player = Instantiate(Resources.Load<GameObject>("Prefabs/player_2"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        LoadNPCs();
    }

    public void LoadNPCs() {        
        for (int i = 0; i < NPCCount; ++i) {
            factory.getTank();
        }
    }

    public void MovePlayer(float translationX, float translationZ){
        translationX *= Time.deltaTime;
        translationZ *= Time.deltaTime;
        
        player.transform.LookAt(new Vector3(player.transform.position.x + translationX, player.transform.position.y, player.transform.position.z + translationZ));
        if (translationX == 0)
            player.transform.Translate(0, 0, Mathf.Abs(translationZ) * moveSpeed);
        else if (translationZ == 0)
            player.transform.Translate(0, 0, Mathf.Abs(translationX) * moveSpeed);
        else
            player.transform.Translate(0, 0, (Mathf.Abs(translationZ) + Mathf.Abs(translationX)) * moveSpeed/2);
    }

    public void PlayerShoot() {
        GameObject bullet = factory.getBullet("Player");
        bullet.transform.position = new Vector3(player.transform.position.x, 0.76f, player.transform.position.z) + player.transform.forward*3.683f;
        bullet.transform.forward = player.transform.forward;
        bullet.GetComponent<Bullet>().playFire();
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(bullet.transform.forward * 200, ForceMode.Impulse);
    }

    public Vector3 getPlayerPosition() {
        return player.transform.position;
    }

    public void OnNPCDead(GameObject npc) {
        NPCCount --;
        if(NPCCount <= 0) {
            Win();
        }
    }

    public void OnPlayerDead() {
        Lose();
    }

    public void Win() {
        gameState = GameState.Win;
    }

    public void Lose() {
        gameState = GameState.Lose;
    }

    public GameState GetGameState() {
        return gameState;
    }
}
