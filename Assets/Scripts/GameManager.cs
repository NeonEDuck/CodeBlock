using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject blockGridPrefab;
    public GameObject setBlockPrefab;
    public GameObject defineBlockPrefab;
    public GameObject forBlockPrefab;
    public GameObject ifBlockPrefab;
    public GameObject startBlockPrefab;
    public GameObject valueBlockPrefab;
    public GameObject spawnerPrefab;
    public List<Transform> blockGrids = new List<Transform>();
    public List<Transform> blockGridsUnderPointer = new List<Transform>();
    public bool isDraging = false;
    public bool wannaTrash = false;
    public bool showTrashIcon = true;
    public Transform gameBoard = null;
    public int whichStack = 0;
    public Transform preSelectedBlockGrids = null;

    public void StartGame() {
        Debug.Log( CreateCommand() );
    }

    public string CreateCommand( Transform target = null ) {

        string output = "";

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
                Debug.Log( target.GetChild( i ).name );
                switch ( blockInfo.blockType ) {
                    case BlockType.startBlock:
                        output += "start\n";
                        break;
                    case BlockType.defineBlock:
                        output += "define " + blockInfo.refField[0].GetComponent<TMP_InputField>().text + "\n" ;
                        break;
                    case BlockType.setBlock:

                        output += "set " + blockInfo.refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text + " = "
                            + blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text + "\n";
                        break;
                    case BlockType.forBlock:
                        output += "for {\n";
                        output += CreateCommand( blockInfo.refField[0] );
                        output += "}\n";
                        break;
                    case BlockType.ifBlock:
                        output += "if {";
                        output += CreateCommand( blockInfo.refField[0] );
                        output += "} else {";
                        output += CreateCommand( blockInfo.refField[1] );
                        output += "}";
                        break;
                    case BlockType.moveBlock:
                        //output += "move " + blockInfo.refField[0].GetComponent<TMP_Dropdown>().value + " " + blockInfo.refField[1].GetComponent<TMP_InputField>().text + "\n";
                        break;
                }
            }
        }

        return output;
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
        }
        return blockPrefab;
    }
}
