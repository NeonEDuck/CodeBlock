using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
    void Update() {
        transform.Rotate( 0f, 0f, Time.deltaTime * -400f );
    }
}
