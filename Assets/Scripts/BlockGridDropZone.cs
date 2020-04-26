﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockGridDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rect;
    public GameManager gameManager = null;
    public BlockGridInfo blockGridInfo = null;
    void Start()
    {
    }

    void Awake() {
        if ( gameManager == null ) {
            if ( GameObject.FindGameObjectWithTag( "GameManager" ) == null ) {
                Debug.LogError( "No GameManager Found" );
            }
            else {
                gameManager = GameObject.FindGameObjectWithTag( "GameManager" ).GetComponent<GameManager>();
                gameManager.blockGrids.Add( transform );
            }
        }
        rect = GetComponent<RectTransform>();

        if ( transform.parent.name.StartsWith( "GameBoard" ) ) {
            blockGridInfo.priority = 0;
            PriorityGiving();
        }

        Resize();
    }

    public void InfoReset() {
        Transform target = transform;
        while ( !target.parent.name.StartsWith( "GameBoard" ) ) {
            target = target.parent;
        }

        Debug.Log( target );

        target.GetComponent<BlockGridDropZone>().PriorityGiving();
        Resize();
    }

    public void PriorityGiving() {
        for (int i = 0; i < transform.childCount; i++ ) {
            Transform child = transform.GetChild( i );

            Debug.Log( child.GetComponent<BlockInfo>().blockType );

            switch ( child.GetComponent<BlockInfo>().blockType ) {
                case BlockType.forBlock:
                    Debug.Log("For");
                    BlockGridDropZone nextBlockGrid = child.GetChild( 1 ).GetChild( 1 ).GetComponent<BlockGridDropZone>();
                    nextBlockGrid.blockGridInfo.priority = blockGridInfo.priority + 1;
                    nextBlockGrid.PriorityGiving();
                    break;
            }
        }
    }

    private void OnDestroy() {
        gameManager.blockGrids.Remove( transform );
    }

    public void Resize() {
        rect.sizeDelta = Vector2.zero;
        float width = 0f;
        float height = 0f;

        ////////
        for ( int i = 0; i < transform.childCount; i++ ) {
            Transform child = transform.GetChild( i );
            switch ( child.GetComponent<BlockInfo>().blockType ) {
                case BlockType.forBlock:
                    child.GetChild( 1 ).GetChild( 1 ).GetComponent<BlockGridDropZone>().Resize();
                    break;
            }
        }
        ///////

        switch ( blockGridInfo.blockGridType ) {
            case BlockGridType.Block:
                for ( int i = 0; i < transform.childCount; i++ ) {
                    Transform child = transform.GetChild( i );
                    RectTransform cRect;
                    if ( ( cRect = child.GetComponent<RectTransform>() ) != null ) {
                        width = Mathf.Max( width, cRect.sizeDelta.x );
                        height += cRect.sizeDelta.y;
                    }
                }

                height -= Mathf.Max( 0f, transform.childCount - 1 ) * GameUtility.CONNECTOR_HEIGHT;

                if ( transform.childCount > 0 && ( transform.GetChild( 0 ).GetComponent<BlockInfo>().connectRule[1] == false  || !gameManager.isDraging) ) {

                }
                else {
                    if ( width == 0f ) width = GameUtility.BLOCK_WIDTH;
                    if ( height == 0f ) height += GameUtility.CONNECTOR_HEIGHT;
                    height += GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT;
                }
                break;

            case BlockGridType.Value:
                //height = GameUtility.VALUE_HEIGHT;
                //width = GameUtility.VALUE_WIDTH;
                break;
        }

        rect.sizeDelta = new Vector2( Mathf.Max( width, rect.sizeDelta.x ), Mathf.Max( height, rect.sizeDelta.y ) );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //GameObject d = eventData.pointerDrag;
        //if ( d != null ) {
        //    switch ( blockGridInfo.blockGridType ) {
        //        case BlockGridType.Block:
        //            if ( d.GetComponent<BlockInfo>().blockType == BlockType.valueBlock ) {
        //                return;
        //            }
        //            break;
        //        case BlockGridType.Value:
        //            if ( d.GetComponent<BlockInfo>().blockType != BlockType.valueBlock ) {
        //                return;
        //            }
        //            break;
        //        case BlockGridType.Logic:
        //            if ( d.GetComponent<BlockInfo>().blockType != BlockType.valueBlock ) {
        //                return;
        //            }
        //            break;
        //    }
        //    d.GetComponent<BlockPhysic>().placeHolderParents.Add( transform );
        //}
        //Debug.Log( "Enter to :" + transform );

        if ( GetComponent<CanvasGroup>().blocksRaycasts == true ) {
        
        }
        else {
            Debug.Log( "what" );
            return;
        }

        gameManager.blockGridsUnderPointer.Add( transform );
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        //GameObject d = eventData.pointerDrag;
        //if ( d != null ) {
        //    switch ( blockGridInfo.blockGridType ) {
        //        case BlockGridType.Block:
        //            if ( d.GetComponent<BlockInfo>().blockType == BlockType.valueBlock ) {
        //                return;
        //            }
        //            break;
        //        case BlockGridType.Value:
        //            if ( d.GetComponent<BlockInfo>().blockType != BlockType.valueBlock ) {
        //                return;
        //            }
        //            break;
        //        case BlockGridType.Logic:
        //            if ( d.GetComponent<BlockInfo>().blockType != BlockType.valueBlock ) {
        //                return;
        //            }
        //            break;
        //    }
        //    d.GetComponent<BlockPhysic>().placeHolderParents.Remove( transform );
        //}

        //Debug.Log( "Leave to :" + transform );

        gameManager.blockGridsUnderPointer.Remove( transform );
    }

    public void OnDrop(PointerEventData eventData)
    {
        //GameObject holding = eventData.pointerDrag;
        //Debug.Log(holding.name + " is being drop on " + name);

        //Transform oldParent = holding.transform.parent;

        //int i = 0;
        //foreach (Transform child in holding.GetComponent<BlockPhysic>().getChildList())
        //{
        //    Debug.Log(child);
        //    child.SetParent(transform.parent);
        //    if (eventData.position.y < transform.position.y)
        //    {
        //        Debug.Log("Down");
        //        child.SetSiblingIndex(transform.GetSiblingIndex() + i++ + 1);
        //    }
        //    else
        //    {
        //        Debug.Log("Up");
        //        child.SetSiblingIndex(transform.GetSiblingIndex());
        //    }
        //}

        //if (oldParent.name.StartsWith("BlockGrid") && oldParent.childCount == 0)
        //{
        //    Destroy(oldParent.gameObject);
        //}

        //rect.sizeDelta = new Vector2( 300f, (transform.childCount+1) * 50f );

        //string codeString = "";

        //for (int j = 0; j < transform.parent.childCount; j++)
        //{
        //    codeString += transform.parent.GetChild(j).GetComponent<BlockInfo>().getBlockCodeString() + "\n";
        //}
        //Debug.Log(codeString);
    }
}
