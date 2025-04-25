using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterInfo;
public class CharacterDataManager : MonoBehaviour
{

    public CharacterManager myData;
    // Start is called before the first frame update
    public void Awake()
    {
        myData = new CharacterManager();

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
