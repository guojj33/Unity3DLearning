using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatrolGAME 
{
    public interface ISceneController 
    {
        int GetScore();                  // 获取分数
        GameState getGameState();        // 获取游戏状态
        void setGameState(GameState gs); // 设置游戏状态
        void Restart();                  // 重新开始游戏
        void Pause();                    // 暂停游戏
        void Begin();                    // 开始游戏
        void LoadResources();
    }

	public class SSDirector : System.Object 
    {
		private static SSDirector _instance;

		public ISceneController CurrentSceneController { get; set; }

		public static SSDirector getInstance() 
        {
			if (_instance == null) 
            {
				_instance = new SSDirector ();
			}
			return _instance;
		}
	}
}