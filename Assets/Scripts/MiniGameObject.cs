using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class MiniGameObject : MonoBehaviour {

    public bool debug = false;
    public GameManager gameManager;
    public Vector2Int posInEnv;
    public float moveAnimationTotal = 0.5f;
    public float moveAnimationStart = -1f;
    public int moveAnimationType = 0;
    public bool fallToDead = false;
    public Vector3 lastPos;
    public Vector3 newPos;
    public Direction direction = Direction.DOWN;
    public short objectType = 0;
    public Transform image = null;
    public List<Sprite> textures = new List<Sprite>();

    void Start() {
        if ( objectType == 2 ) {
            changeTexture( (int)direction );
            transform.GetChild( 1 ).localRotation = Quaternion.Euler( 0, 0, (int)direction * -90f );
        }
    }

    void Update() {
        if ( moveAnimationStart != -1f ) {
            float t = Mathf.Min( 1f, ( Time.time - moveAnimationStart ) / moveAnimationTotal );
            switch ( moveAnimationType ) {
                case 0:
                    transform.localPosition = Vector3.Lerp( lastPos, newPos, Mathf.Pow( t, 2.0f ) );
                    break;
                case 1:
                    transform.localPosition = Vector3.Lerp( lastPos, newPos, Mathf.Sin( t * 180f * Mathf.Deg2Rad ) );
                    break;
                case 2:
                    transform.localScale = new Vector3( 1.0f - Mathf.Pow( t, 2.0f ), 1.0f - Mathf.Pow( t, 2.0f ), 1 );
                    break;
            }

            if ( t == 1f ) {
                if ( moveAnimationType == 2 ) {
                    Destroy( gameObject );
                }
                moveAnimationStart = -1f;
                if ( objectType == 2 ) {
                    checkDoor();
                }
            }
        }
    }

    private void OnDestroy() {
        if ( !debug )
            gameManager.gameEnv2d[posInEnv.x, posInEnv.y].Remove( this );
    }

    public void Turn( int num ) {
        num = ( num == 0 ) ? 1 : 3;

        direction = (Direction)( ((int)direction + num) % 4 );
        changeTexture( (int)direction );

        if ( objectType == 2 ) {
            transform.GetChild( 1 ).localRotation = Quaternion.Euler( 0, 0, (int)direction * -90f );
        }
    }

    public bool Move( int id ) {
        bool canMove = true;
        moveAnimationType = -1;
        lastPos = transform.localPosition;
        newPos = lastPos;
        Vector2Int delta = new Vector2Int( 0, 0 );


        if ( objectType == 2 ) {
            delta = GetDirection( direction, out id );
        }
        else {
            switch ( id ) {
                case 0:
                    delta = new Vector2Int( 0, 1 );
                    break;
                case 1:
                    delta = new Vector2Int( 0, -1 );
                    break;
                case 2:
                    delta = new Vector2Int( -1, 0 );
                    break;
                case 3:
                    delta = new Vector2Int( 1, 0 );
                    break;
            }
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
                }
                else if ( ( r = containObject( newPosInEnv.x, newPosInEnv.y, 4 ) ).Count > 0 ) {
                    allow = true;
                }
                else if ( ( r = containObject( newPosInEnv.x, newPosInEnv.y, 5 ) ).Count > 0 ) {
                    allow = true;
                    fallToDead = true;
                }
                else if ( ( r = containObject( newPosInEnv.x, newPosInEnv.y, 6 ) ).Count > 0 ) {
                    allow = true;
                }
                else if ( ( r = containObject( newPosInEnv.x, newPosInEnv.y, 0 ) ).Count > 0 ) {
                    allow = true;
                }
            }



            if ( allow ) {
                gameManager.gameEnv2d[newPosInEnv.x, newPosInEnv.y].Add( this );
                gameManager.gameEnv2d[posInEnv.x, posInEnv.y].Remove( this );
                posInEnv = newPosInEnv;
                newPos = transform.localPosition + new Vector3( delta.x * 50.0f, delta.y * -50.0f, 0 );
                moveAnimationType = 0;
            }
        }

        if ( moveAnimationType == -1 ) {
            canMove = false;
            newPos = transform.localPosition + new Vector3( delta.x * 10.0f, delta.y * -10.0f, 0 );
            moveAnimationType = 1;
        }

        moveAnimationStart = Time.time;

        return canMove;
    }
    public List<int> containObject( int x, int y, int objectType ) {
        List<int> results = new List<int>();
        int i = 0;

        if (x >= 0 && y >= 0 && x < gameManager.gameEnv2d.GetLength(0) && y < gameManager.gameEnv2d.GetLength(1) ) {
            foreach ( MiniGameObject mgo in gameManager.gameEnv2d[x, y] ) {
                if ( mgo.objectType == objectType ) {
                    results.Add( i );
                }
                i++;
            }
        }
        return results;
    }
    public bool IsOnFlag() {
        return containObject( posInEnv.x, posInEnv.y, 4 ).Count > 0;
    }
    public void checkDoor() {
        bool allHavingPress = true;

        foreach ( MiniGameObject btn in GameVariable.gamePiece[6] ) {
            if ( containObject( btn.posInEnv.x, btn.posInEnv.y, 2 ).Count == 0 && containObject( btn.posInEnv.x, btn.posInEnv.y, 3 ).Count == 0 ) {
                allHavingPress = false;
                break;
            }
        }

        if ( allHavingPress ) {
            foreach ( MiniGameObject door in GameVariable.gamePiece[7] ) {
                door.changeTexture( 1 );
                door.objectType = 0;
            }
        }
        //return containObject( posInEnv.x, posInEnv.y, 4 ).Count > 0;
    }

    public void changeTexture( int i ) {
        image.GetComponent<Image>().sprite = textures[ Mathf.Min( textures.Count-1, i ) ];
    }

    public bool IsOn( int num ) {
        if ( containObject( posInEnv.x, posInEnv.y, num ).Count > 0 ) {
            return true;
        }

        return false;

        //return containObject( posInEnv.x, posInEnv.y, 4 ).Count > 0;
    }

    public bool IsFacing( int num ) {

        if ( objectType != 2 ) return false;
        Vector2Int pos = posInEnv + GetDirection( direction );
        if ( containObject( pos.x, pos.y, num ).Count > 0 ) {
            return true;
        }

        return false;
    }

    public Vector2Int GetDirection( Direction direction, out int num ) {
        switch ( direction ) {
            case Direction.DOWN:
                num = 0;
                return new Vector2Int( 0, 1 );
            case Direction.UP:
                num = 1;
                return new Vector2Int( 0, -1 );
            case Direction.LEFT:
                num = 2;
                return new Vector2Int( -1, 0 );
            case Direction.RIGHT:
                num = 3;
                return new Vector2Int( 1, 0 );
        }
        num = -1;
        return new Vector2Int( 0, 0 );
    }
    public Vector2Int GetDirection( Direction direction ) {
        switch ( direction ) {
            case Direction.DOWN:
                return new Vector2Int( 0, 1 );
            case Direction.UP:
                return new Vector2Int( 0, -1 );
            case Direction.LEFT:
                return new Vector2Int( -1, 0 );
            case Direction.RIGHT:
                return new Vector2Int( 1, 0 );
        }
        return new Vector2Int( 0, 0 );
    }
}
