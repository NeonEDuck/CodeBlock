using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlockPhysic : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {
    //GameObject parentToReturnTo = null;
    public GameObject blockGridPrefab = null;
    public CanvasGroup canvasGroup = null;
    public Transform dropOn = null;
    public Transform placeHolderParent;

    private bool draging = false;

    private Vector2 pointerOffset;
    private Vector2 pointerOrigin;
    private float onHoldTimer = 0.0f;
    private Transform target;
    private bool holding = false;
    private List<Transform> childList;
    private GameObject placeHolder;
    private Transform oldParent;
    private int maxStack = 0;

    public void OnPointerDown( PointerEventData eventData ) {
        onHoldTimer = 0.0f;
        holding = true;
        pointerOffset = new Vector2(transform.position.x, transform.position.y) - eventData.position;
    }

    public void OnBeginDrag( PointerEventData eventData ) {
        target = transform;
        pointerOrigin = eventData.position;
        maxStack = 0;

        oldParent = transform.parent;

        if (onHoldTimer > 0.25f)
        {

            placeHolder = new GameObject();
            placeHolderParent = transform.parent;
            placeHolder.AddComponent<RectTransform>().sizeDelta = GetComponent<RectTransform>().sizeDelta;
            placeHolder.transform.SetSiblingIndex(transform.GetSiblingIndex());

            Transform parent = transform.parent;
            transform.SetParent(transform.parent.parent);

            if (parent.childCount == 0)
            {
                Destroy(parent.gameObject);
            }

            canvasGroup.blocksRaycasts = false;
            draging = true;
        }
        else
        {

            childList = new List<Transform>();

            Transform newParent = Instantiate(blockGridPrefab, transform.parent).GetComponent<Transform>();
            newParent.position = transform.position;
            newParent.SetParent(oldParent.parent);

            for (int i = transform.GetSiblingIndex(); i < oldParent.childCount; i++)
            {
                childList.Add(oldParent.GetChild(i));
            }

            placeHolder = new GameObject();
            placeHolderParent = transform.parent;
            placeHolder.AddComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y * childList.Count);
            placeHolder.transform.SetSiblingIndex(transform.GetSiblingIndex());

            int j = 0;
            foreach (Transform child in childList)
            {
                child.SetParent(newParent.transform);
                child.SetSiblingIndex(j++);
            }

            canvasGroup.blocksRaycasts = false;
            target = newParent;
            target.GetComponent<CanvasGroup>().blocksRaycasts = false;
            draging = true;
        }


        if (oldParent.childCount == 0)
        {
            oldParent.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag( PointerEventData eventData ) {

        // if cursor move, select one block
        target.position = eventData.position + pointerOffset;

        if (placeHolderParent != null)
        {
            placeHolder.transform.SetParent(placeHolderParent);

            for (int i = 0; i < placeHolderParent.childCount; i++)
            {
                Transform phpc = placeHolderParent.GetChild(i);
                if (transform.position.y + phpc.GetComponent<RectTransform>().sizeDelta.y / 2 > phpc.position.y)
                {
                    placeHolder.transform.SetSiblingIndex(i);
                    break;
                }
            }
        }
        else
        {
            Debug.Log("null");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draging = false;
        dropOn = null;
        target.GetComponent<CanvasGroup>().blocksRaycasts = true;
        canvasGroup.blocksRaycasts = true;


        float height = 0f;
        if (target.gameObject.name.StartsWith("BlockGrid"))
        {

            if (placeHolderParent != null)
            {
                int i = 0;
                while (target.childCount > 0)
                {
                    Transform child = target.GetChild(0);
                    height += child.GetComponent<RectTransform>().sizeDelta.y;
                    child.SetParent(placeHolderParent);
                    child.SetSiblingIndex(placeHolder.transform.GetSiblingIndex() + i++);
                }
                Destroy(target.gameObject);
            }
            else
            {
                for (int i = 0; i < target.childCount; i++)
                {
                    height += target.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
                }
                placeHolderParent = transform.parent;
            }
        }
        else
        {

            if (placeHolderParent != null)
            {
                height += GetComponent<RectTransform>().sizeDelta.y;
                transform.SetParent(placeHolderParent);
                transform.SetSiblingIndex(placeHolder.transform.GetSiblingIndex());
            }
            else
            {
                createBlockGrid(eventData.position + pointerOffset);
                placeHolderParent = transform.parent;
            }
        }
        if (placeHolderParent != null) {
            Vector2 phpSize = placeHolderParent.GetComponent<RectTransform>().sizeDelta;
            placeHolderParent.GetComponent<RectTransform>().sizeDelta = new Vector2( Mathf.Max(maxStack * 10f+300f, phpSize.x), phpSize.y + height);
            placeHolderParent.position = new Vector2(placeHolderParent.position.x, placeHolderParent.position.y - height/2);
        }

        Vector2 opSize = oldParent.GetComponent<RectTransform>().sizeDelta;
        oldParent.GetComponent<RectTransform>().sizeDelta = new Vector2(opSize.x, opSize.y - height);
        oldParent.position = new Vector2( oldParent.position.x , oldParent.position.y + height/2);

        holding = false;
        childList = null;

        placeHolder.transform.SetParent(placeHolder.transform.parent.parent);
        Destroy(placeHolder);

        if (oldParent.childCount == 0)
        {
            Destroy(oldParent.gameObject);
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
            blockGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, GetComponent<RectTransform>().sizeDelta.y * ( getChildList().Count + 1) );
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
