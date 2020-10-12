using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class MiniGameObject : MonoBehaviour {

    public GameManager gameManager;
    public Vector2Int posInEnv;
    public float moveAnimationTotal = 0.5f;
    public float moveAnimationStart = -1f;
    public int moveAnimationType = 0;
    public Vector3 lastPos;
    public Vector3 newPos;
    public short objectType = 0;

    void Update() {
        if ( moveAnimationStart != -1f ) {
            float t = Mathf.Min( 1f, ( Time.time - moveAnimationStart ) / moveAnimationTotal );
            switch ( moveAnimationType ) {
                case 0:
                    transform.position = Vector3.Lerp( lastPos, newPos, Mathf.Pow( t, 2.0f ) );
                    break;
                case 1:
                    transform.position = Vector3.Lerp( lastPos, newPos, Mathf.Sin( t * 180f * Mathf.Deg2Rad ) );
                    break;
            }
        }
    }

    public bool Move( int id ) {
        bool canMove = true;
        moveAnimationType = -1;
        lastPos = transform.position;
        newPos = lastPos;
        Vector2Int delta = new Vector2Int( 0, 0 );

        switch ( id ) {
            case 0:
                delta = new Vector2Int( 0, -1 );
                break;
            case 1:
                delta = new Vector2Int( 0, 1 );
                break;
            case 2:
                delta = new Vector2Int( -1, 0 );
                break;
            case 3:
                delta = new Vector2Int( 1, 0 );
                break;
        }

        Vector2Int newPosInEnv = posInEnv + delta;
        Debug.Log( newPosInEnv );
        if ( newPosInEnv.x >= 0 && newPosInEnv.x <= 6 && newPosInEnv.y >= 0 && newPosInEnv.y <= 5 ) {

            bool allow = false;

            if ( gameManager.gameEnv2d[newPosInEnv.x, newPosInEnv.y].Count == 0 ) {
                allow = true;
            }
            else {
                List<int> r;
                if ( ( r = containObject( newPosInEnv.x, newPosInEnv.y, 3 ) ).Count > 0 ) {
                    foreach ( int i in r ) {
                        if ( gameManager.gameEnv2d[newPosInEnv.x, newPosInEnv.y][i].Move( id ) ) {
                            allow = true;
                        }
                    }
                    Debug.Log( "BOX" );
                }
                else if ( ( r = containObject( newPosInEnv.x, newPosInEnv.y, 4 ) ).Count > 0 ) {
                    allow = true;
                    Debug.Log( "FLAG" );
                }
            }



            if ( allow ) {
                gameManager.gameEnv2d[newPosInEnv.x, newPosInEnv.y].Add( this );
                gameManager.gameEnv2d[posInEnv.x, posInEnv.y].Remove( this );
                posInEnv = newPosInEnv;
                newPos = transform.position + new Vector3( delta.x * 50.0f, delta.y * -50.0f, 0 );
                moveAnimationType = 0;
            }
        }

        if ( moveAnimationType == -1 ) {
            canMove = false;
            newPos = transform.position + new Vector3( delta.x * 10.0f, delta.y * -10.0f, 0 );
            Debug.Log( newPos );
            moveAnimationType = 1;
        }

        moveAnimationStart = Time.time;

        return canMove;
    }
    public List<int> containObject( int x, int y, int objectType ) {
        List<int> results = new List<int>();
        int i = 0;
        foreach ( MiniGameObject mgo in gameManager.gameEnv2d[x, y] ) {
            if ( mgo.objectType == objectType ) {
                results.Add( i );
            }
            i++;
        }
        return results;
    }
    public bool IsOnFlag() {
        return containObject( posInEnv.x, posInEnv.y, 4 ).Count > 0;
    }
}
