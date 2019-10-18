using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HitUFOGAME;

public enum ActionMode {PHYSICS, KINEMATIC, NOTSET};

public enum SSActionEventType:int { Started, Competeted }
 
public interface ISSActionCallback {
    void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0,
        string strParam = null,
        Object objectParam = null);
	
}
 
public interface IUserAction {
    void BeginGame ();
    void GameOver ();
    void Restart ();
    int GetScore ();
    int GetRound ();
    int GetBlood ();
    ActionMode GetMode();
    void SetMode(ActionMode m);
    void hit (Vector3 pos);
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

public class SSActionManager : MonoBehaviour {
 
    public Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();        //保存所以已经注册的动作
    public List<SSAction> waitingAdd = new List<SSAction>();                           //动作的等待队列，在这个对象保存的动作会稍后注册到动作管理器里
    public List<int> waitingDelete = new List<int>();                                  //动作的删除队列，在这个对象保存的动作会稍后删除
 
    
    // Use this for initialization
    protected void Start()
    {
 
    }
 
    // Update is called once per frame
    protected void Update()
    {
        //把等待队列里所有的动作注册到动作管理器里
        foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
        waitingAdd.Clear();
 
        //管理所有的动作，如果动作被标志为删除，则把它加入删除队列，被标志为激活，则调用其对应的Update函数
        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
            }
        }
 
        //把删除队列里所有的动作删除
        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }
 
    //初始化一个动作
    public void RunAction(GameObject gameobject, SSAction action, ISSActionCallback manager)
    {
        action.gameobject = gameobject;
        action.transform = gameobject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }
}

public class SSFlyAction : SSAction {
    float acceleration; //重力加速度
    public float horizontalSpeed = 5F;  //horizontalSpeed是飞碟水平方向的速度
    float time;//time是飞碟已经飞行的时间
    public ActionMode m = ActionMode.NOTSET;
    public Rigidbody rigidbody;
 
	public override void Start () {
        enable = true;
        if(m == ActionMode.PHYSICS) {   //物理学
            rigidbody = this.gameobject.GetComponent<Rigidbody> ();
            if (!rigidbody) {
                this.gameobject.AddComponent<Rigidbody> ();
            }
            Debug.Log(this.gameobject.GetComponent<Rigidbody>() + "addForce");
            this.gameobject.GetComponent<Rigidbody>().velocity = horizontalSpeed * gameobject.GetComponent<DiskData>().direction * 2;
        }
    }
 
    // Update is called once per frame
    public override void Update () {    //运动学
        if (gameobject.activeSelf) {
            transform.Translate(new Vector3(gameobject.GetComponent<DiskData>().direction.x*Time.deltaTime * horizontalSpeed, -Time.deltaTime*2F, 0));
            //飞碟落地
            if (this.transform.position.y < -6) {
                if(this.transform.position.y > -9){
                    BloodRecorder bloodRecorder;
                    bloodRecorder = Singleton<BloodRecorder>.Instance;
                    bloodRecorder.Record(this.gameobject);
                }
                this.destroy = true;
                this.enable = false;
                this.callback.SSActionEvent(this);
            }
        }
	}

    //add in HitUFO_v2
    public override void FixedUpdate() {    //物理学
        if (gameobject.activeSelf) {
            //飞碟落地
            if (this.transform.position.y < -6) {
                if(this.transform.position.y > -9){
                    BloodRecorder bloodRecorder;
                    bloodRecorder = Singleton<BloodRecorder>.Instance;
                    bloodRecorder.Record(this.gameobject);
                }
                Destroy(this.gameobject.GetComponent<Rigidbody> ());
                this.destroy = true;
                this.enable = false;
                this.callback.SSActionEvent(this);
            }
        }
    }

    public static SSFlyAction GetSSAction()
    {
        SSFlyAction action = ScriptableObject.CreateInstance<SSFlyAction>();
        return action;
    }
}

public class MyActionManager : SSActionManager, ISSActionCallback {
    public Controller sceneController;
    public List<SSFlyAction> flyActions = new List<SSFlyAction> ();
    //public int DiskNumber = 0;

    //回收动作
    private List<SSFlyAction> used = new List<SSFlyAction> ();
    private List<SSFlyAction> free = new List<SSFlyAction> ();


    protected new void Start () {
        sceneController = SSDirector.getInstance().CurrentSceneController as Controller;
        sceneController.myActionManager = this;
        flyActions.Add(SSFlyAction.GetSSAction());
    }

    // Update is called once per frame
    protected void Update()
    {
        //把等待队列里所有的动作注册到动作管理器里
        foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
        waitingAdd.Clear();
 
        //管理所有的动作，如果动作被标志为删除，则把它加入删除队列，被标志为激活，则调用其对应的Update函数
        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();//
            }
        }
 
        //把删除队列里所有的动作删除
        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    SSFlyAction GetSSFlyAction () {
        SSFlyAction action = null;
        if (free.Count > 0) {
            action = free[0];
            free.Remove(free[0]);
        }
        else {
            action = ScriptableObject.Instantiate<SSFlyAction>(flyActions[0]);
        }
        //action = ScriptableObject.CreateInstance<SSFlyAction>();
        used.Add(action);
        return action;
    }

    public void FreeSSAction (SSAction action) {
        SSFlyAction tmp = null;
        foreach (SSFlyAction i in used) {
            if (action.GetInstanceID () == i.GetInstanceID ()) {
                tmp = i;
                break;//
            }
        }
        if (tmp != null) {
            tmp.reset ();
            free.Add (tmp);
            used.Remove(tmp);
        }
    }

    public void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0,
        string strParam = null,
        Object objectParam = null) {
        if (source is SSFlyAction) {
            DiskFactory df = Singleton<DiskFactory>.Instance;
            df.FreeDisk (source.gameobject);
            FreeSSAction(source);
        }
    }

    public void throwDisk (GameObject disk) {
        SSFlyAction fly = GetSSFlyAction();
        fly.m = ActionMode.KINEMATIC;
        if (sceneController.GetRound() == 1){
            fly.horizontalSpeed = Random.Range(5F, 7F);
        }
        else if (sceneController.GetRound() == 2){
            fly.horizontalSpeed = Random.Range(7F, 9F);
        }
        else {
            fly.horizontalSpeed = Random.Range(9F, 11F);
        }
        RunAction (disk, fly, this);
    }
}

public class PhysicsActionManager : SSActionManager, ISSActionCallback {
    public Controller sceneController;
    public List<SSFlyAction> flyActions = new List<SSFlyAction> ();
    //public int DiskNumber = 0;

    //回收动作
    private List<SSFlyAction> used = new List<SSFlyAction> ();
    private List<SSFlyAction> free = new List<SSFlyAction> ();


    protected new void Start () {
        sceneController = SSDirector.getInstance().CurrentSceneController as Controller;
        sceneController.physicsActionManager = this;
        flyActions.Add(SSFlyAction.GetSSAction());
    }

    protected void Update()
    {
        //把等待队列里所有的动作注册到动作管理器里
        foreach (SSAction ac in waitingAdd) actions[ac.GetInstanceID()] = ac;
        waitingAdd.Clear();
 
        //管理所有的动作，如果动作被标志为删除，则把它加入删除队列，被标志为激活，则调用其对应的Update函数
        foreach (KeyValuePair<int, SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.destroy)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.FixedUpdate();//与 MyActionManager 的区别
            }
        }
 
        //把删除队列里所有的动作删除
        foreach (int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
    }

    SSFlyAction GetSSFlyAction () {
        SSFlyAction action = null;
        if (free.Count > 0) {
            action = free[0];
            free.Remove(free[0]);
        }
        else {
            action = ScriptableObject.Instantiate<SSFlyAction>(flyActions[0]);
        }
        used.Add(action);
        return action;
    }

    public void FreeSSAction (SSAction action) {
        SSFlyAction tmp = null;
        foreach (SSFlyAction i in used) {
            if (action.GetInstanceID () == i.GetInstanceID ()) {
                tmp = i;
                break;//
            }
        }
        if (tmp != null) {
            tmp.reset ();
            free.Add (tmp);
            used.Remove(tmp);
        }
    }

    public void SSActionEvent(SSAction source,
        SSActionEventType events = SSActionEventType.Competeted,
        int intParam = 0,
        string strParam = null,
        Object objectParam = null) {
        if (source is SSFlyAction) {
            DiskFactory df = Singleton<DiskFactory>.Instance;
            df.FreeDisk (source.gameobject);
            FreeSSAction(source);
        }
    }

    public void throwDisk (GameObject disk) {
        SSFlyAction fly = GetSSFlyAction();
        fly.m = ActionMode.PHYSICS;
        if (sceneController.GetRound() == 1){
            fly.horizontalSpeed = Random.Range(5F, 7F);
        }
        else if (sceneController.GetRound() == 2){
            fly.horizontalSpeed = Random.Range(7F, 9F);
        }
        else {
            fly.horizontalSpeed = Random.Range(9F, 11F);
        }
        RunAction (disk, fly, this);
    }
}