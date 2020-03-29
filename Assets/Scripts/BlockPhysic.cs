using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

public class BlockPhysic : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    GameObject parentToReturnTo = null;
    public void OnBeginDrag( PointerEventData eventData ) {
        transform.SetParent( transform.parent.parent );
    }

    public void OnDrag( PointerEventData eventData ) {
        this.transform.position = eventData.position;
    }

    public void OnEndDrag( PointerEventData eventData ) {
        throw new System.NotImplementedException();
    }

    public void OnDrop( PointerEventData eventData ) {
        throw new System.NotImplementedException();
    }
}
