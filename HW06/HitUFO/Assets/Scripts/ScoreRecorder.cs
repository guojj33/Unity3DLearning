using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreRecorder : MonoBehaviour {
	public int score = 0;        
	private Dictionary<Color, int> scoreTable = new Dictionary<Color, int>();

	void Start () {
		score = 0;
		scoreTable.Add(Color.red, 2);
		scoreTable.Add(Color.green, 4);
		scoreTable.Add(Color.blue, 6);
	}
	public void Record(GameObject disk) {
		score += scoreTable[disk.GetComponent<DiskData>().color];
	}

	public void Reset() {
		score = 0;
	}
}
