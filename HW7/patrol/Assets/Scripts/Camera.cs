using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;            //相机跟随的物体
    public float smothing = 5f;          //相机跟随的平滑速度
    Vector3 offset;                      //相机与物体相对偏移位置

    void Start() {
        offset = new Vector3(0f, 7f, -4f);
    }

    void FixedUpdate() {
        // 设置摄像机目标位置
        Vector3 target = player.transform.position + offset;
        // 摄像机自身位置到目标位置平滑过渡
        transform.position = Vector3.Lerp(transform.position, target, smothing * Time.deltaTime);
    }
}