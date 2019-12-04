using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    private Dictionary<int, GameObject> tanks;
    private Dictionary<int, GameObject> freeTanks;
    private Dictionary<int, GameObject> bullets;
    private Dictionary<int, GameObject> freeBullets;
    private List<ParticleSystem> psQueue;

    void Awake() {
		tanks = new Dictionary<int, GameObject>();
		freeTanks = new Dictionary<int, GameObject>();
		bullets = new Dictionary<int, GameObject>();
		freeBullets = new Dictionary<int, GameObject>();
		psQueue = new List<ParticleSystem>();
        Debug.Log("Factory.Start()");     
    }

    void OnEnable() {
        //订阅事件
        GameEventManager.OnNPCDead += OnNPCDead;
    }

    void OnDisable() {
        GameEventManager.OnNPCDead -= OnNPCDead;
    }

    public GameObject getTank() {
        if(freeTanks.Count == 0) {
            GameObject newTank = Instantiate(Resources.Load<GameObject>("Prefabs/npc_2"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            tanks.Add(newTank.GetInstanceID(), newTank);
            newTank.transform.position = new Vector3(Random.Range(-20,20), 0, Random.Range(-20,20));
            newTank.SetActive(true);
            return newTank;
        }
        else {
            foreach (KeyValuePair<int, GameObject> pair in freeTanks) {
                pair.Value.SetActive(true);
                freeTanks.Remove(pair.Key);
                tanks.Add(pair.Key, pair.Value);
                pair.Value.transform.position = new Vector3(Random.Range(-20,20), 0, Random.Range(-20,20));
                pair.Value.GetComponent<NPC>().reset();
                return pair.Value;
            }
        }
        return null;
    }

    public GameObject getBullet(string tagName) {
        if(freeBullets.Count == 0) {
            GameObject newBullet = Instantiate(Resources.Load<GameObject>("Prefabs/bullet"), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            newBullet.tag = tagName;
            bullets.Add(newBullet.GetInstanceID(), newBullet);
            return newBullet;
        }
        else {
            foreach (KeyValuePair<int, GameObject> pair in freeBullets) {
                pair.Value.SetActive(true);
                freeBullets.Remove(pair.Key);
                bullets.Add(pair.Key, pair.Value);
                pair.Value.tag = tagName;
                return pair.Value;
            }
        }
        return null;
    }

	public ParticleSystem getPS(string tagName) {
		for (int i = 0; i < psQueue.Count; i++) {
			if(!psQueue[i].isPlaying && psQueue[i].tag == tagName) {
				return psQueue[i];
			}
		}
        string prefabName = "Prefabs/" + tagName;
		ParticleSystem newPS = Instantiate(Resources.Load<ParticleSystem>(prefabName), new Vector3(0, 0, 0), Quaternion.identity) as ParticleSystem;
		newPS.tag = tagName;
        psQueue.Add(newPS);
		return newPS;
	}

	public void OnNPCDead(GameObject npc) {
        tanks.Remove(npc.GetInstanceID());
        freeTanks.Add(npc.GetInstanceID(), npc);
        npc.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        npc.SetActive(false);
    }


	public void BulletRecycle(GameObject bullet) {
		bullets.Remove(bullet.GetInstanceID());
		freeBullets.Add(bullet.GetInstanceID(), bullet);
		bullet.GetComponent<Rigidbody>().velocity = new Vector3(0,0,0);
		bullet.SetActive(false);
	}

    public void reset() {
        //回收所有
        foreach (KeyValuePair<int, GameObject> pair in tanks) {
            pair.Value.SetActive(false);
            freeTanks.Add(pair.Key, pair.Value);
        }
        tanks.Clear();
    }
}
