using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockPhysic : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    //GameObject parentToReturnTo = null;
    public GameObject blockGridPrefab = null;
    public void OnBeginDrag( PointerEventData eventData ) {
        Transform parent = transform.parent;
        transform.SetParent( transform.parent.parent );

        if ( parent.childCount == 0 )
        {
            Destroy( parent.gameObject );
        }
    }

    public void OnDrag( PointerEventData eventData ) {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag( PointerEventData eventData )
    {
        createBlockGrid(eventData.position);
    }

    public void OnDrop( PointerEventData eventData ) {
        throw new System.NotImplementedException();
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

        if (parent != "BlockGrid")
        {
            GameObject blockGrid = Instantiate(blockGridPrefab, transform.parent);
            blockGrid.transform.position = position;
            transform.SetParent(blockGrid.transform);
        }
    }
}
