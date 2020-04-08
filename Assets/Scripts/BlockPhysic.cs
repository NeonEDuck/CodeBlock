using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlockPhysic : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerDownHandler {
    //GameObject parentToReturnTo = null;
    public GameObject blockGridPrefab = null;
    public CanvasGroup canvasGroup = null;
    public Transform dropOn = null;

    private bool draging = false;

    private Vector2 pointerOffset;
    private Vector2 pointerOrigin;
    private float onHoldTimer = 0.0f;
    private Transform target;
    private bool holding = false;
    private List<Transform> childList;

    public void OnPointerDown( PointerEventData eventData ) {
        onHoldTimer = 0.0f;
        holding = true;
        pointerOffset = new Vector2(transform.position.x, transform.position.y) - eventData.position;
    }

    public void OnBeginDrag( PointerEventData eventData ) {
        target = transform;
        pointerOrigin = eventData.position;
    }

    public void OnDrag( PointerEventData eventData ) {

        // if cursor move, select one block
        if ( !draging ) {
            if ( pointerOrigin != eventData.position ) {
                childList = new List<Transform>();
                Transform oldParent = transform.parent;

                Transform newParent = Instantiate(blockGridPrefab, transform.parent).GetComponent<Transform>();
                newParent.position = transform.position;
                newParent.SetParent(oldParent.parent);

                for (int i = transform.GetSiblingIndex(); i < oldParent.childCount; i++)
                {
                    childList.Add(oldParent.GetChild(i));
                }

                int j = 0;
                foreach (Transform child in childList)
                {
                    child.SetParent(newParent.transform);
                    child.SetSiblingIndex(j++);
                }

                if (oldParent.childCount == 0)
                {
                    Destroy(oldParent.gameObject);
                }

                canvasGroup.blocksRaycasts = false;
                target = newParent;
                draging = true;
            }

            if ( onHoldTimer > 0.25f )
            {
                Transform parent = transform.parent;
                transform.SetParent(transform.parent.parent);

                if (parent.childCount == 0)
                {
                    Destroy(parent.gameObject);
                }

                canvasGroup.blocksRaycasts = false;
                draging = true;
            }
        }
        
        if ( draging ) {
            target.position = eventData.position + pointerOffset;
        }
    }

    public void OnEndDrag( PointerEventData eventData )
    {
        draging = false;
        dropOn = null;
        canvasGroup.blocksRaycasts = true;
        createBlockGrid(eventData.position + pointerOffset );
        holding = false;
        childList = null;
    }

    public void OnDrop( PointerEventData eventData ) {

        //BlockPhysic holding = eventData.pointerDrag.GetComponent<BlockPhysic>();
        //holding.dropOn = transform;

        if ( eventData.pointerDrag != gameObject ) {
            GameObject holding = eventData.pointerDrag;
            Debug.Log( holding.name + " is being drop on " + name );
            Debug.Log( transform.GetSiblingIndex() );

            Transform oldParent = holding.transform.parent;

            int i = 0;
            foreach ( Transform child in holding.GetComponent<BlockPhysic>().getChildList() ) {
                Debug.Log( child );
                child.SetParent( transform.parent );
                if ( eventData.position.y < transform.position.y ) {
                    Debug.Log( "Down" );
                    child.SetSiblingIndex( transform.GetSiblingIndex() + i++ + 1 );
                }
                else {
                    Debug.Log( "Up" );
                    child.SetSiblingIndex( transform.GetSiblingIndex() );
                }
            }

            if ( oldParent.name.StartsWith( "BlockGrid" ) && oldParent.childCount == 0 ) {
                Destroy( oldParent.gameObject );
            }

        }
    }

    void Start()
    {
        createBlockGrid( transform.position );
    }

    void Update() {
        if ( holding ) {
            onHoldTimer += Time.deltaTime;
            //Debug.Log( onHoldTimer );
        }
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

    public List<Transform> getChildList() {
        if ( childList == null ) {
            List<Transform> selfList = new List<Transform>();
            selfList.Add( transform );

            return selfList;
        }
        return childList;
    }
}
