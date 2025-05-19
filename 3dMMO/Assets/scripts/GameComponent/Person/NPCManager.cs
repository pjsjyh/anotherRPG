using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance;  // 싱글톤 패턴

    private Dictionary<string, InteractPlayer> npcDictionary = new Dictionary<string, InteractPlayer>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void RegisterNPC(InteractPlayer npc)
    {
        if (!npcDictionary.ContainsKey(npc.npcCharaterID))
        {
            npcDictionary[npc.npcCharaterID] = npc;
        }
    }

    public void SetNPCState(string npcID, npcState state)
    {
        if (npcDictionary.TryGetValue(npcID, out InteractPlayer npc))
        {
            npc.ThisState = state;
            Debug.Log($"📝 NPC {npcID} 상태 변경: {state}");
        }
    }

    public npcState GetNPCState(string npcID)
    {
        if (npcDictionary.TryGetValue(npcID, out InteractPlayer npc))
        {
            return npc.ThisState;
        }
        return npcState.idle;
    }
}
