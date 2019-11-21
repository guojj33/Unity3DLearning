## 血条的预制设计 - Health Bar

### 演示视频

<a href = "https://www.ixigua.com/i6761802929105010702/">视频地址</a>  
(<a href = "https://github.com/guojj33/Unity3DLearning/blob/master/HW9/assets/HealthBar.mp4" target = "_blank" >备用地址</a>)

### 文件说明

- 代码放在 [HealthBar/Assets/Scripts](https://github.com/guojj33/Unity3DLearning/tree/master/HW9/HealthBar/Assets/Scripts) 中
- 预制放在 [HealthBar/Assets/Prefabs](https://github.com/guojj33/Unity3DLearning/tree/master/HW9/HealthBar/Assets/Prefabs) 中
- 工程下载到本地后，双击 HealthBar/Assets/1.unity 即可打开工程

### 设计过程

#### 1. 使用 IMGUI
- 定义一个 HorizontalScrollbar 表示血条，用两个按钮调节血量。创建一个空对象名为 IMGUIHealthBar ，将脚本挂载其上。
    ```C#  
    //IMGUIHealthBar.cs
    public class IMGUIHealthBar : MonoBehaviour
    {
        public float health = 0f;
        private float resultHealth;
        private Rect healthBar;
        public Rect healthUp;
        public Rect healthDown;

        // Start is called before the first frame update
        void Start()
        {    
            healthBar = new Rect(50, 50, 200, 30);
            healthUp = new Rect(100, 80, 40, 30);
            healthDown = new Rect(150, 80, 40, 30);
            resultHealth = health;
        }

        void OnGUI()
        {
            if(GUI.Button(healthUp, "加血"))
            {
                resultHealth = resultHealth + 0.1f > 1f ? 1f : resultHealth + 0.1f;
                Debug.Log("加血");
            }
            if(GUI.Button(healthDown, "减血"))
            {
                resultHealth = resultHealth - 0.1f < 0 ? 0 : resultHealth - 0.1f;
                Debug.Log("减血");
            }

            health = Mathf.Lerp(health, resultHealth, 0.05f);
            GUI.HorizontalScrollbar(healthBar, 0f, health, 0f, 1f);
        }
    }
    ```

- 将 IMGUIHealthBar 拉到 Assets/Prefabs 文件夹中即可创建预制。要使用时直接拖到场景中即可。

#### 2. 使用 UGUI
- 创建一个 Capsule 作为人物，添加 Canvas 子对象，在 Canvas 上添加 Slider 子对象  
![](assets/1.png)

- 将 Slider 的 Handle Slide Area 取消激活，将 Fill Area 的 Fill 的 Image 组件的颜色改称红色，就可以得到血条效果  
![](assets/2.png)

- 给 Capsule 添加控制移动脚本，Capsule 可以在 XOZ 平面上移动  
    ```C#
    //Move.cs
    public class Move : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            float translationX = Input.GetAxis("Horizontal");
            float translationZ = Input.GetAxis("Vertical");
            MovePlayer(translationX, translationZ);
        }
        
        public void MovePlayer(float translationX, float translationZ){
            translationX *= Time.deltaTime;
            translationZ *= Time.deltaTime;
            this.transform.LookAt(new Vector3(this.transform.position.x + translationX, this.transform.position.y, this.transform.position.z + translationZ));
            if (translationX == 0)
                this.transform.Translate(0, 0, Mathf.Abs(translationZ) * 5);
            else if (translationZ == 0)
                this.transform.Translate(0, 0, Mathf.Abs(translationX) * 5);
            else
                this.transform.Translate(0, 0, (Mathf.Abs(translationZ) + Mathf.Abs(translationX)) * 2.5f);
        }
    }
    ```

- 给 Canvas 添加脚本使血条一直面向摄像机  
    ```C#
    //LookAtCamera.cs
    public class LookAtCamera : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            this.transform.LookAt(Camera.main.transform.position);
        }
    }
    ```
- 将 Canvas 拉到 Assets/Prefabs 文件夹中即可创建预制，要使用时，将 Canvas 拖到 Capsule 中，使 Canvas 成为 Capsule 的子对象即可

### 两种方式优缺点
#### IMGUI
- 优点
  - IMGUI 的存在符合游戏编程的传统
  - 在修改模型，渲染模型这样的经典游戏循环编程模式中，在渲染阶段之后，绘制 UI 界面无可挑剔
  - 这样的编程既避免了 UI 元素保持在屏幕最前端，又有最佳的执行效率，一切控制掌握在程序员手中
- 缺点
  - 效率低下
  - 难以调试

#### UGUI
- 优点
  - 所见即所得（WYSIWYG）设计工具
  - 设计师也能参与参与程序开发
  - 支持多模式、多摄像机渲染
  - UI 元素与场景融为一体，支持复杂布局
  - 面向对象的编程