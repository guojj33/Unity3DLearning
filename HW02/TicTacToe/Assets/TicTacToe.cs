using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTacToe : MonoBehaviour {

	private int turn = 1;
	private int[,] state = new int[3,3];
	int leftMost = 260;
	int topMost = 140;
	int buttonWidth = 50;
	int posLeft = 9;

	// Use this for initialization
	void Start () {
		reset ();
	}

	void OnGUI(){
		if (GUI.Button (new Rect (leftMost, topMost + 3 * buttonWidth + 10, 60, 20), "Restart")) {
			reset ();
		}
		int result = check ();
		if (result != 0) {
			string win = check () == 1 ? "X" : "O";
			win += " wins!";
			GUI.Label (new Rect (leftMost + 1 * buttonWidth + 40, topMost + 3 * buttonWidth + 10, 50, buttonWidth), win);
		} else {
			if (posLeft == 0) {
				GUI.Label (new Rect (leftMost + 1 * buttonWidth + 40, topMost + 3 * buttonWidth + 10, 100, buttonWidth), "Tie!");
			} else {
				string whoseTurn = (turn == 1 ? "X" : "O");
				whoseTurn += " 's turn.";
				GUI.Label (new Rect (leftMost + 1 * buttonWidth + 40, topMost + 3 * buttonWidth + 10, 100, buttonWidth), whoseTurn);
			}
		}
		for (int i = 0; i < 3; ++i) {
			for (int j = 0; j < 3; ++j) {
				if (state [i,j] == 1) {
					GUI.Button (new Rect (leftMost + i * buttonWidth, topMost + j * buttonWidth, buttonWidth, buttonWidth), "X");
				} else if (state [i,j] == -1) {
					GUI.Button (new Rect (leftMost + i * buttonWidth, topMost + j * buttonWidth, buttonWidth, buttonWidth), "O");
				} else if (GUI.Button (new Rect (leftMost + i * buttonWidth, topMost + j * buttonWidth, buttonWidth, buttonWidth), "")){
					if (result == 0) {
						state [i, j] = turn;
						posLeft = (posLeft == 0) ? 0 : posLeft - 1;
					}
					turn = -turn;
				}
			}
		}
	}

	void reset(){
		for (int i = 0; i < 3; ++i) {
			for (int j = 0; j < 3; ++j) {
				state [i,j] = 0;
			}
		}
		posLeft = 9;
	}

	int check(){
		for (int i = 0; i < 3; ++i) {
			if (state [0, i] == state [1, i] && state [0, i] == state [2, i] && state [1, i] == state [2, i]) {
				return state [0, i];
			}
		}
		for (int i = 0; i < 3; ++i) {
			if (state [i, 0] == state [i, 1] && state [i, 0] == state [i, 2] && state [i, 1] == state [i, 2]) {
				return state [i, 0];
			}
		}
		if (state [0, 0] == state [1, 1] && state [0, 0] == state [2, 2] && state [1, 1] == state [2, 2]) {
			return state [0, 0];
		}
		if (state [2, 0] == state [1, 1] && state [2, 0] == state [0, 2] && state [1, 1] == state [0, 2]) {
			return state [2, 0];
		}
		return 0;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
