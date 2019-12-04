using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour 
{
    //NPC 死亡事件
    public delegate void NPCDeadEvent(GameObject npc);
    public static event NPCDeadEvent OnNPCDead;
    //玩家死亡事件
    public delegate void PlayerDeadEvent();
    public static event PlayerDeadEvent OnPlayerDead;

    public void NPCDead(GameObject npc) {
        if (OnNPCDead != null) {
            OnNPCDead(npc);
        }
    }

    public void PlayerDead() {
        if (OnPlayerDead != null) {
            OnPlayerDead();
        }
    }
}