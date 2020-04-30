using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueBlockSwap : MonoBehaviour
{
    public GameManager gameManager = null;
    public Transform valueBlockGrid = null;
    public Transform inputField = null;

    private void Update() {
        if ( gameManager.isDraging || valueBlockGrid.childCount > 0 ) {
            inputField.SetSiblingIndex( 0 );
        }
        else {
            valueBlockGrid.SetSiblingIndex( 0 );
        }
    }

    void Awake() {
        if ( gameManager == null ) {
            if ( GameObject.FindGameObjectWithTag( "GameManager" ) == null ) {
                Debug.LogError( "No GameManager Found" );
            }
            else {
                gameManager = GameObject.FindGameObjectWithTag( "GameManager" ).GetComponent<GameManager>();
            }
        }
    }
}
