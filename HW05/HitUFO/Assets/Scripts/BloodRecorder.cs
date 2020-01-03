using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodRecorder : MonoBehaviour {
	public int blood = 30;
	private Dictionary<Color, int> injuryTable = new Dictionary<Color, int>();

	void Start () {
		injuryTable.Add(Color.red, 1);
		injuryTable.Add(Color.green, 2);
		injuryTable.Add(Color.blue, 3);
	}

	public void Record(GameObject disk) {
		blood -= injuryTable[disk.GetComponent<DiskData>().color];
	}

	public void Reset() {
		blood = 30;
	}
}
