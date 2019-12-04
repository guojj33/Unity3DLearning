using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AITANKGAME {
	public enum GameState {Start, Play, Win, Lose};

    public interface ISceneController  {
        void LoadResources();
    }

	public class SSDirector : System.Object {
		private static SSDirector _instance;

		public ISceneController CurrentSceneController { get; set; }

		public static SSDirector getInstance() {
			if (_instance == null) {
				_instance = new SSDirector ();
			}
			return _instance;
		}
	}
	public interface IUserAction {
		GameState GetGameState();
		void StartGame();
		void RestartGame();
		void PlayerShoot();
    	void MovePlayer(float translationX, float translationZ);
	}
}