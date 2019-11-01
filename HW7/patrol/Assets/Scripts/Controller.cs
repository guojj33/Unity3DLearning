using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PatrolGAME;

public class Controller : MonoBehaviour, ISceneController, IUserAction
{
    // Start is called before the first frame update
    public int playerRegionIndex = 1;
    public PatrolActionManager patrolActionManager;
    public PatrolFactory patrolFactory;
    public UserGUI userGUI;
    public ScoreRecorder scoreRecorder;
    public GameObject player;
    public int playerRegion;
    private GameState gameState = GameState.START;
    private List<GameObject> patrols;
    //public Camera cameraMain;

    void Start()
    {
        SSDirector director = SSDirector.getInstance();
        director.CurrentSceneController = this;
        scoreRecorder = Singleton<ScoreRecorder>.Instance;
        patrolFactory = Singleton<PatrolFactory>.Instance;
        playerRegion = 4;
        patrolActionManager = gameObject.AddComponent<PatrolActionManager>();
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;
        LoadResources();
        for(int i = 0; i < patrols.Count; ++i){
            patrolActionManager.Patrol(patrols[i]);
        }
    }

    
    public void LoadResources()
    {
        player = Instantiate(Resources.Load<GameObject>("prefabs/player"), new Vector3(0, 0, -5f), Quaternion.identity);
        player.GetComponent<Animator>().SetBool("death", false);
        //player.GetComponent<Animator>().SetBool("run", false);
        player.GetComponent<Animator>().SetBool("pause", true);
        patrols = patrolFactory.GetPatrols();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().player = player;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < patrols.Count; i++) {
            // 更新玩家区域信息
            patrols[i].GetComponent<PatrolData>().playerRegion = playerRegion;
        }
    }

    public void MovePlayer(float translationX, float translationZ){
        if (translationX != 0 || translationZ != 0) {
            player.GetComponent<Animator>().SetBool("pause", false);
        } else {
            player.GetComponent<Animator>().SetBool("pause", true);
        }
        translationX *= Time.deltaTime;
        translationZ *= Time.deltaTime;
        
        player.transform.LookAt(new Vector3(player.transform.position.x + translationX, player.transform.position.y, player.transform.position.z + translationZ));
        if (translationX == 0)
            player.transform.Translate(0, 0, Mathf.Abs(translationZ) * 5);
        else if (translationZ == 0)
            player.transform.Translate(0, 0, Mathf.Abs(translationX) * 5);
        else
            player.transform.Translate(0, 0, (Mathf.Abs(translationZ) + Mathf.Abs(translationX)) * 2.5f);
    }

    void OnEnable() 
    {
        // 订阅游戏事件
        GameEventManager.OnGoalLost += OnGoalLost;
        GameEventManager.OnFollowing += OnFollowing;
        GameEventManager.GameOver += GameOver;
    }

    void OnDisable() 
    {
        GameEventManager.OnGoalLost -= OnGoalLost;
        GameEventManager.OnFollowing -= OnFollowing;
        GameEventManager.GameOver -= GameOver;
    }

    public void OnGoalLost(GameObject patrol){
        patrolActionManager.Patrol(patrol);
        scoreRecorder.Record();
    }

    public void OnFollowing(GameObject patrol){
        patrolActionManager.Follow(player, patrol);
    }

    public void GameOver(){
        gameState = GameState.LOSE;
        StopAllCoroutines();
        patrolFactory.PausePatrol();
        player.GetComponent<Animator>().SetTrigger("death");
        patrolActionManager.DestroyAllActions();
    }

    public int GetScore(){
        return scoreRecorder.score;
    }

    public void Restart(){
        SceneManager.LoadScene("1");
    }

    public void Pause(){
        gameState = GameState.PAUSE;
        patrolFactory.PausePatrol();
        player.GetComponent<Animator>().SetBool("pause", true);
        StopAllCoroutines();
    }

    public void Begin(){
        gameState = GameState.RUNNING;
        patrolFactory.StartPatrol();
        //player.GetComponent<Animator>().SetBool("run", true);
        player.GetComponent<Animator>().SetBool("pause", false);
        //StartCoroutine(SSDirector.getInstance().CountDown());
    }

    public GameState getGameState(){
        return gameState;
    }

    public void setGameState(GameState gs){
        gameState = gs;
    }
}
