using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITANKGAME;

public class Bullet : MonoBehaviour {
    public float explosionRadius = 5.0f;

    
    public void playFire() {
        Factory myFactory = Singleton<Factory>.Instance;
        ParticleSystem fire = myFactory.getPS("fire");
        fire.transform.position = transform.position;
        fire.Play();
    }

    void OnCollisionEnter(Collision other) {
        Factory myFactory = Singleton<Factory>.Instance;

        ParticleSystem smoke = myFactory.getPS("smoke");
        smoke.transform.position = transform.position;

        ParticleSystem explosion = myFactory.getPS("explosion");
        explosion.transform.position = transform.position;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        for (int i = 0; i < colliders.Length; i++) {
            // 如果玩家子弹击中了NPC或者NPC子弹击中了玩家坦克，伤害才有效
            if(colliders[i].tag == "Player" && this.tag == "NPC" || 
                colliders[i].tag == "NPC" && this.tag == "Player") {
                Debug.Log("对" + colliders[i].tag + "造成伤害");
                float distance = Vector3.Distance(colliders[i].transform.position, transform.position);
                int hurt = (int)(100f/distance/10); // 距离越远伤害越小;
                Debug.Log("伤害值"+hurt);
                if(colliders[i].gameObject.activeSelf){
                    colliders[i].GetComponent<BloodRecorder>().reduceBlood(hurt);
                }
            }
        }
        smoke.Play();
        explosion.Play();
        if(this.gameObject.activeSelf) {
            myFactory.BulletRecycle(this.gameObject);
        }
    }
}