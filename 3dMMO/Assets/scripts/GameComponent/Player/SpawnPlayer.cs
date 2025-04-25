using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private void Awake()
    {
        NetworkManager n = GameObject.Find("NetworkManagerPre").GetComponent<NetworkManager>();
        //StartCoroutine(n.StratSpawn());
    }
}
