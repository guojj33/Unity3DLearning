## HitUFO 打飞碟 - 运动与物理兼容版

### 演示视频

<a href = "">视频地址</a>  
(<a href = "https://github.com/guojj33/Unity3DLearning/blob/master/HW6/assets/HitUFO.mp4" target = "_blank" >备用地址</a>)

### 文件说明

- 代码放在 [HitUFO/Assets/Scripts](https://github.com/guojj33/Unity3DLearning/tree/master/HW6/HitUFO/Assets/Scripts) 中
- 工程下载到本地后，双击 HitUFO/Assets/ufo.unity 即可打开工程

### 游戏要求
- 按 adapter 模式设计图修改飞碟游戏  
- 使它同时支持物理运动与运动学（变换）运动  

### 代码设计
- 定义运动模式  
    ```C#
    public enum ActionMode {PHYSICS, KINEMATIC, NOTSET};
    ```
- SSFlyAction  
  在之前的版本中，已经在 Update 函数中实现运动学运动，现在修改其 Start 函数并添加 FixedUpdate 函数，使之既能实现运动学运动又能实现物理运动，而且只需通过修改其 ActionMode 即可切换。  

    ```C#
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
        public override void Update () ;    //运动学

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

        public static SSFlyAction GetSSAction() ;
    }
    ```
- PyhsicsActionManager  
    创建物理运动管理器，用于调度物理运动。其实现基本上与 MyActionManager 运动学运动管理器相同，但是在 Update 函数中调用的是动作的 FixedUpdate 函数而非 Update函数。这就是前面 SSFlyAction 修改的，用于实现物理运动的函数。  
    ```C#
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

        protected void Update() {
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

        SSFlyAction GetSSFlyAction ();

        public void FreeSSAction (SSAction action);

        public void SSActionEvent(SSAction source,
            SSActionEventType events = SSActionEventType.Competeted,
            int intParam = 0,
            string strParam = null,
            Object objectParam = null);

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
    ```
- UserGUI
    修改菜单，使得玩家可以在进入游戏时选择运动模式。  
    使用变量（布尔值）实现多层菜单。  
    ```C#
    private IUserAction action;
    private int chooseMode = 0;
    private int start = 1;
    private int restart = 0;
    ```
    ```C#
    void OnGUI() {
        ...
        if (start == 1) {
            if (GUI.Button (new Rect (Screen.width / 2 - 60, Screen.height / 2 + 150, 100, 30), "Start")) 
            {
                chooseMode = 1;
                start = 0;
            }
        }
        if (chooseMode == 1) {
            if (GUI.Button(new Rect(Screen.width / 2 - 120, Screen.height / 2 + 150, 100, 30), "KINEMATIC"))
            {
                action.SetMode(ActionMode.KINEMATIC);
                action.BeginGame ();
                chooseMode = 0;
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 0, Screen.height / 2 + 150, 100, 30), "PHYSICS"))
            {
                action.SetMode(ActionMode.PHYSICS);
                action.BeginGame ();
                chooseMode = 0;
            }
        }
        ...
    }
    ```
- Controller
    实现 IUserAction 新增的 SetMode 函数，用于设置动作管理器的类型。  
    ```C#
    public void SetMode(ActionMode m) {  
        if (m == ActionMode.KINEMATIC) {
            myActionManager = gameObject.AddComponent<MyActionManager>() as MyActionManager;
        }
        else if (m == ActionMode.PHYSICS) {
            physicsActionManager = gameObject.AddComponent<PhysicsActionManager>() as PhysicsActionManager;
        }
        actionMode = m;
    }
    ```