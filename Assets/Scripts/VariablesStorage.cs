using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablesStorage : MonoBehaviour {

    public int levelId = 0;

    void Awake() {
        DontDestroyOnLoad( gameObject );
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
