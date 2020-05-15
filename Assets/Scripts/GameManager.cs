using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    [Header( "BlockPrefab" )]
    public GameObject blockGridPrefab;
    public GameObject setBlockPrefab;
    public GameObject defineBlockPrefab;
    public GameObject forBlockPrefab;
    public GameObject ifBlockPrefab;
    public GameObject startBlockPrefab;
    public GameObject valueBlockPrefab;
    public GameObject moveBlockPrefab;
    public GameObject spawnerPrefab;
    [Header( "MiniPrefab" )]
    public GameObject obstaclePrefab;
    public GameObject playerPrefab;

    public Transform gameBoard = null;
    public Transform gameView = null;
    public short[] gameEnv;

    [Header("Private")]
    public List<Transform> blockGrids = new List<Transform>();
    public List<Transform> blockGridsUnderPointer = new List<Transform>();
    public bool isDraging = false;
    public bool wannaTrash = false;
    public bool showTrashIcon = true;
    public int whichStack = 0;
    public Transform preSelectedBlockGrids = null;

    public void ResetGameView() {
        foreach ( Transform child in gameView.transform ) {
            Destroy( child.gameObject );
        }
        Vector3 origin = gameView.position + new Vector3( 0, gameView.GetComponent<RectTransform>().sizeDelta.y, 0 );
        if ( gameEnv != null ) {
            for ( int i = 0; i < gameEnv.Length; i++ ) {
                Transform spawn = null;
                switch ( gameEnv[i] ) {
                    case 0:
                        break;
                    case 1:
                        spawn = Instantiate( obstaclePrefab, gameView ).transform;
                        break;
                    case 2:
                        spawn = Instantiate( playerPrefab, gameView ).transform;
                        break;
                }
                if ( spawn != null ) {
                    spawn.position = origin + new Vector3( ( i % 7 + 0.5f ) * 50f, -( Mathf.Floor( i / 7 ) + 0.5f ) * 50f, 0f );
                }
            }
        }
    }

    public void StartGame() {
        ResetGameView();
        List<Tuple<string, List<object>>> commands = CreateCommand();
        
        foreach ( Tuple<string, List<object>> command in commands ) {
            switch ( command.Item1 ) {
                case "start":
                    Debug.Log( "start");
                    break;
                case "define":
                    Debug.Log( "define " + command.Item2[0]);
                    break;
                case "set":
                    Debug.Log( "set " + command.Item2[0] + " " + command.Item2[1] );
                    break;
                case "for":
                    Debug.Log( "for " + command.Item2[0] + " " + command.Item2[1] );
                    break;
                case "if":
                    Debug.Log( "if " + command.Item2[0] + " " + command.Item2[1] );
                    break;
                case "move":
                    Debug.Log( "move " + command.Item2[0] + " " + command.Item2[1] );
                    break;
            }
        }

    }

    public List<Tuple<string, List<object>>> CreateCommand( Transform target = null ) {

        string output = "";
        List<Tuple<string, List<object>>> commands = new List<Tuple<string, List<object>>>();

        if ( target == null ) {
            GameObject[] list = GameObject.FindGameObjectsWithTag( "Start" );
            foreach ( GameObject start in list ) {
                if ( start.transform.parent.parent.name.StartsWith( "BlockSpawner" ) ) continue;
                target = start.transform.parent;
                break;
            }
        }

        if ( target != null ) {
            for ( int i = 0; i < target.childCount; i++ ) {
                BlockInfo blockInfo = target.GetChild(i).GetComponent<BlockInfo>();
                string type = "";
                List<object> infos = new List<object>();
                switch ( blockInfo.blockType ) {
                    case BlockType.startBlock:
                        type = "start";
                        break;
                    case BlockType.defineBlock:
                        type = "define";
                        infos.Add( blockInfo.refField[0].GetComponent<TMP_InputField>().text );
                        if ( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount > 0 ) {
                            infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                        }
                        else {
                            infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text );
                        }
                        break;
                    case BlockType.setBlock:
                        type = "set";
                        infos.Add( blockInfo.refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                        if ( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount > 0 ) {
                            infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                        }
                        else {
                            infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text );
                        }
                        //output += "set " +  + " = "
                        //    + blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text + "\n";
                        break;
                    case BlockType.forBlock:
                        type = "for";
                        infos.Add( CreateCommand( blockInfo.refField[0] ) );
                        //output += "for {\n";
                        //output += CreateCommand( blockInfo.refField[0] );
                        //output += "}\n";
                        break;
                    case BlockType.ifBlock:
                        type = "if";
                        infos.Add( CreateCommand( blockInfo.refField[0] ) );
                        infos.Add( CreateCommand( blockInfo.refField[1] ) );
                        //output += "if {";
                        //output += CreateCommand( blockInfo.refField[0] );
                        //output += "} else {";
                        //output += CreateCommand( blockInfo.refField[1] );
                        //output += "}";
                        break;
                    case BlockType.moveBlock:
                        type = "move";
                        infos.Add( blockInfo.refField[0].GetComponent<TMP_Dropdown>().value );
                        if ( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount > 0 ) {
                            infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                        }
                        else {
                            infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text );
                        }
                        //output += "move " + blockInfo.refField[0].GetComponent<TMP_Dropdown>().value + " " + blockInfo.refField[1].GetComponent<TMP_InputField>().text + "\n";
                        break;
                }

                commands.Add( new Tuple<string, List<object>>( type, infos ) );
            }
        }

        return commands;
    }

    public void BlockGridBlockRaycast( bool enable ) {
        foreach ( Transform bg in blockGrids ) {
            if ( bg != null ) {
                bg.GetComponent<CanvasGroup>().blocksRaycasts = enable;
            }
        }
    }

    public void ResetAll() {
        foreach ( Transform bg in blockGrids ) {
            if ( bg != null && bg.parent.name.StartsWith( "GameBoard" ) ) {
                BlockGridDropZone bgdz = bg.GetComponent<BlockGridDropZone>();
                if ( bgdz != null ) {
                    bgdz.InfoReset();
                }
            }
        }
    }

    public void BlockBlockRaycast( bool enable ) {
        List<Transform> removeList = new List<Transform>();
        foreach ( Transform bg in blockGrids ) {
            if ( bg == null ) {
                removeList.Add( bg );
                continue;
            }
            for ( int i = 0; i < bg.childCount; i++ ) {
                CanvasGroup cg = bg.GetChild( i ).GetComponent<CanvasGroup>();
                if ( cg != null ) {
                    cg.blocksRaycasts = enable;
                }
                Image img = bg.GetChild( i ).GetComponent<Image>();
                if ( img != null ) {
                    img.raycastTarget = enable;
                }
            }
        }
        foreach ( Transform bg in removeList ) {
            blockGrids.Remove( bg );
        }
    }

    public GameObject getBlockPrefab( BlockType blockType ) {

        GameObject blockPrefab = null;
        switch ( blockType ) {
            case BlockType.setBlock:
                blockPrefab = setBlockPrefab;
                break;
            case BlockType.defineBlock:
                blockPrefab = defineBlockPrefab;
                break;
            case BlockType.forBlock:
                blockPrefab = forBlockPrefab;
                break;
            case BlockType.ifBlock:
                blockPrefab = ifBlockPrefab;
                break;
            case BlockType.startBlock:
                blockPrefab = startBlockPrefab;
                break;
            case BlockType.valueBlock:
                blockPrefab = valueBlockPrefab;
                break;
            case BlockType.moveBlock:
                blockPrefab = moveBlockPrefab;
                break;
        }
        return blockPrefab;
    }
}
