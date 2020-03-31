using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlockPhysic : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler {
    //GameObject parentToReturnTo = null;
    public GameObject blockGridPrefab = null;
    public CanvasGroup canvasGroup = null;
    public Transform dropOn = null;

    private Vector2 pointerOffset;

    public void OnBeginDrag( PointerEventData eventData ) {
        Transform parent = transform.parent;
        transform.SetParent( transform.parent.parent );

        if ( parent.childCount == 0 )
        {
            Destroy( parent.gameObject );
        }

        canvasGroup.blocksRaycasts = false;
        pointerOffset = new Vector2( transform.position.x, transform.position.y ) - eventData.position;
    }

    public void OnDrag( PointerEventData eventData ) {
        transform.position = eventData.position + pointerOffset;
    }

    public void OnEndDrag( PointerEventData eventData )
    {


        dropOn = null;
        canvasGroup.blocksRaycasts = true;
        createBlockGrid(eventData.position + pointerOffset );
    }

    public void OnDrop( PointerEventData eventData ) {

        //BlockPhysic holding = eventData.pointerDrag.GetComponent<BlockPhysic>();
        //holding.dropOn = transform;

        if ( eventData.pointerDrag != gameObject ) {
            GameObject holding = eventData.pointerDrag;
            Debug.Log( holding.name + " is being drop on " + name );
            Debug.Log( transform.GetSiblingIndex() );

            holding.transform.SetParent( transform.parent );
            if ( eventData.position.y < transform.position.y ) {
                holding.transform.SetSiblingIndex( transform.GetSiblingIndex()+1 );
            }
            else {
                holding.transform.SetSiblingIndex( transform.GetSiblingIndex() );
            }
        }
    }

    void Start()
    {
        createBlockGrid( transform.position );
    }

    void createBlockGrid( Vector3 position )
    {
        if (blockGridPrefab == null)
        {
            Debug.Log("blockGridPrefab is somehow null!");
            return;
        }

        string parent = transform.parent.name;

        if ( !parent.StartsWith( "BlockGrid" ) )
        {
            GameObject blockGrid = Instantiate(blockGridPrefab, transform.parent);
            blockGrid.transform.position = position;
            transform.SetParent(blockGrid.transform);
        }
    }
}
