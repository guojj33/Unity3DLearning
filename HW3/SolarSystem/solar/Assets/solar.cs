using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class solar : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject.Find("shui").transform.RotateAround(Vector3.zero, new Vector3(0.1F, 1, 0), 30 * Time.deltaTime);
		GameObject.Find("shui").transform.Rotate(Vector3.up * Time.deltaTime * 10000);
		GameObject.Find("jin").transform.RotateAround(Vector3.zero, new Vector3(0, 1, 0.1F), 40 * Time.deltaTime);
		GameObject.Find("jin").transform.Rotate(Vector3.up * Time.deltaTime * 10000);
		
		GameObject.Find("di").transform.RotateAround(Vector3.zero, new Vector3(0, 1.1F, 0), 20 * Time.deltaTime);
		GameObject.Find("di").transform.Rotate(Vector3.up * Time.deltaTime * 10000 * 0.01F);
		GameObject.Find("null").transform.RotateAround(Vector3.zero, new Vector3(0, 1.1F, 0), 20 * Time.deltaTime);

		GameObject yue = GameObject.Find("yue");
		Vector3 diPos = yue.transform.parent.position;
		yue.transform.RotateAround(diPos, Vector3.up, 500 * Time.deltaTime);

		GameObject.Find("huo").transform.RotateAround(Vector3.zero, new Vector3(0.12F, 1, 0), 25 * Time.deltaTime);
		GameObject.Find("huo").transform.Rotate(Vector3.up * Time.deltaTime * 10000);
		GameObject.Find("mu").transform.RotateAround(Vector3.zero, new Vector3(0, 1, 0.12F), 35 * Time.deltaTime);
		GameObject.Find("mu").transform.Rotate(Vector3.up * Time.deltaTime * 10000);
		GameObject.Find("tu").transform.RotateAround(Vector3.zero, new Vector3(0, 1.12F, 0), 25 * Time.deltaTime);
		GameObject.Find("tu").transform.Rotate(Vector3.up * Time.deltaTime * 10000);
		GameObject.Find("tian").transform.RotateAround(Vector3.zero, new Vector3(0.11F, 1, 0), 15 * Time.deltaTime);
		GameObject.Find("tian").transform.Rotate(Vector3.up * Time.deltaTime * 10000);
		GameObject.Find("hai").transform.RotateAround(Vector3.zero, new Vector3(0, 1, 0.11F), 28 * Time.deltaTime);
		GameObject.Find("hai").transform.Rotate(Vector3.up * Time.deltaTime * 10000);
	}
}
