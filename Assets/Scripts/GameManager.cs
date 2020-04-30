using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<Transform> blockGrids = new List<Transform>();
    public List<Transform> blockGridsUnderPointer = new List<Transform>();
    public bool isDraging = false;
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
                bg.GetComponent<BlockGridDropZone>().InfoReset();
            }
        }
    }

    public void BlockBlockRaycast( bool enable ) {
        foreach ( Transform bg in blockGrids ) {
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
    }
}
