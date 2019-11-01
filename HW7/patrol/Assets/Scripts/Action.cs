using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatrolGAME;

public enum SSActionEventType:int { Started, Competeted }

public enum GameState { RUNNING, PAUSE, START, LOSE, WIN } // 游戏状态

public interface ISSActionCallback {
    void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0,
        string strParam = null,
        Object objectParam = null);
	
}
 
public interface IUserAction {
    void MovePlayer(float translationX, float translationZ);
}

public class SSAction : ScriptableObject {
 
    public bool enable = false;
    public bool destroy = false;
 
    public GameObject gameobject { get; set; }
    public Transform transform { get; set; }
    public ISSActionCallback callback { get; set; }
 
    protected SSAction() { }
 
    public virtual void Start () {
        throw new System.NotImplementedException();
	}
	
	// Update is called once per frame
	public virtual void Update () {
        throw new System.NotImplementedException();
    }
 
    public virtual void FixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    public void reset()
    {
        enable = false;
        destroy = false;
        gameobject = null;
        transform = null;
        callback = null;
    }
}

public class SSActionManager : MonoBehaviour
{
    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();
    private List<int> waitingDelete = new List<int>();

    protected void Update() {
        foreach (SSAction action in waitingAdd) {
            actions[action.GetInstanceID()] = action;
        }
        waitingAdd.Clear();
        foreach (KeyValuePair<int, SSAction> kv in actions) {
            SSAction action = kv.Value;
            if (action.enable) {
                Debug.Log("SSActionManager.Update()");
                action.Update(); // update action
            } else if (action.destroy) {
                waitingDelete.Add(action.GetInstanceID()); // release action
            }
        }
        foreach (int key in waitingDelete) {
            SSAction action = actions[key];
            actions.Remove(key);
            Destroy(action);
        }
        waitingDelete.Clear();
    }

    // 执行动作
    public void RunAction(GameObject gameObject, SSAction action, ISSActionCallback callback) {
        action.gameobject = gameObject;
        action.transform = gameObject.transform;
        action.callback = callback;
        waitingAdd.Add(action);
        action.Start();
    }

    public void DestroyAll() {
        foreach (KeyValuePair<int, SSAction> kv in actions) {
            SSAction ac = kv.Value;
            ac.destroy = true;
        }
    }

    protected void Start() { }
}

//巡逻兵巡逻动作
public class PatrolAction : SSAction
{
    private float pos_x, pos_z;                 // 移动前的初始x和z方向坐标
    private bool turn = true;                   // 是否选择新方向
    private PatrolData data;                    // 巡逻兵的数据

    public static PatrolAction GetAction(Vector3 location) {
        PatrolAction action = CreateInstance<PatrolAction>();
        action.enable = true;
        action.pos_x = location.x;
        action.pos_z = location.z;
        return action;
    }

    public override void Start() {
        data = this.gameobject.GetComponent<PatrolData>();
    }

    public override void Update() {
        if (SSDirector.getInstance().CurrentSceneController.getGameState().Equals(GameState.RUNNING)) {
            // 巡逻兵巡逻
            Patrol();
            
            if (!data.isFollowing && data.isPlayerInRange && data.patrolRegion == data.playerRegion && !data.isCollided) {
                this.destroy = true;
                this.enable = false;
                this.callback.SSActionEvent(this);
                this.gameobject.GetComponent<PatrolData>().isFollowing = true;
                Singleton<GameEventManager>.Instance.FollowPlayer(this.gameobject);
            }
        }
    }

    void Patrol() {
        if (turn) {
            pos_x = this.transform.position.x + Random.Range(-4f, 4f);
            pos_z = this.transform.position.z + Random.Range(-4f, 4f);
            Debug.Log("turn is true");
            this.transform.LookAt(new Vector3(pos_x, 0, pos_z));
            this.gameobject.GetComponent<PatrolData>().isCollided = false;
            turn = false;
        }
        float distance = Vector3.Distance(transform.position, new Vector3(pos_x, 0, pos_z));

        if (this.gameobject.GetComponent<PatrolData>().isCollided) {
            //碰撞，则向后转，寻找新位置
            this.transform.Rotate(Vector3.up, 180);
            GameObject temp = new GameObject();
            temp.transform.position = this.transform.position;
            temp.transform.rotation = this.transform.rotation;
            temp.transform.Translate(0, 0, Random.Range(0.01f, 0.1f));
            pos_x = temp.transform.position.x;
            pos_z = temp.transform.position.z;
            this.transform.LookAt(new Vector3(pos_x, 0, pos_z));
            this.gameobject.GetComponent<PatrolData>().isCollided = false;
            Destroy(temp);
            //turn = true;
        } else if (distance <= 0.1) {
            turn = true;
        } else {
            // 向前移动巡逻兵
            this.transform.Translate(0, 0, Time.deltaTime);
        }
    }
}

public class PatrolFollowAction : SSAction
{
    private float speed = 1.5f;          // 跟随玩家的速度
    private GameObject player;           // 玩家
    private PatrolData data;             // 巡逻兵数据

    public static PatrolFollowAction GetAction(GameObject player) {
        PatrolFollowAction action = CreateInstance<PatrolFollowAction>();
        action.enable = true;
        action.player = player;
        return action;
    }

    public override void Start() {
        data = this.gameobject.GetComponent<PatrolData>();
    }

    public override void Update() {
        if (SSDirector.getInstance().CurrentSceneController.getGameState().Equals(GameState.RUNNING)) {
            // 追击玩家
            transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime);
            this.transform.LookAt(player.transform.position);

            if (data.isFollowing && (!(data.isPlayerInRange && data.patrolRegion == data.playerRegion) || data.isCollided)) {
                this.destroy = true;
                this.enable = false;
                this.callback.SSActionEvent(this);
                this.gameobject.GetComponent<PatrolData>().isFollowing = false;
                Singleton<GameEventManager>.Instance.PlayerEscape(this.gameobject);
            }
        }
    }
}

public class PatrolActionManager : SSActionManager, ISSActionCallback
{
    public PatrolAction patrol;
    public PatrolFollowAction follow;

    // 巡逻
    public void Patrol(GameObject ptrl) {
        this.patrol = PatrolAction.GetAction(ptrl.transform.position);
        Debug.Log("PatrolActionManager.Patrol()");
        this.RunAction(ptrl, patrol, this);
    }

    // 追击
    public void Follow(GameObject player, GameObject patrol) {
        this.follow = PatrolFollowAction.GetAction(player);
        Debug.Log("PatrolActionManager.Follow()");
        this.RunAction(patrol, follow, this);
    }

    //停止所有动作
    public void DestroyAllActions() {
        DestroyAll();
    }

    public void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0,
        string strParam = null,
        Object objectParam = null){ }
}