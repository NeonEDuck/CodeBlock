using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablesStorage : MonoBehaviour {

    public int levelId = 0;
    public string levelJson = "";

    void Awake() {
        DontDestroyOnLoad( gameObject );
    }
}
