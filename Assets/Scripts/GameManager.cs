using System;
using System.Collections;
using System.Collections.Generic;
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
