using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridBlockDropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rect;
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject d = eventData.pointerDrag;
        if (d != null)
        {
            d.GetComponent<BlockPhysic>().placeHolderParent = transform;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject d = eventData.pointerDrag;
        if (d != null)
        {
            d.GetComponent<BlockPhysic>().placeHolderParent = null;
        }
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
