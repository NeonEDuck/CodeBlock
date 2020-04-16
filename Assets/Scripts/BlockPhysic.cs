using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlockPhysic : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {
    public GameObject blockGridPrefab = null;
    public CanvasGroup canvasGroup = null;
    public BlockInfo blockInfo = null;
    [HideInInspector] 
    public Transform placeHolderParent;             // Where placeHolder is going to be

    private Vector2 size;                           // Current Block size
    private Vector2 pointerOffset;                  // From center of the block to cursor
    private float onHoldTimer = 0.0f;               // How much time did player start holding
    private Transform target;                       // What cursor is holding
    private bool holding = false;                   // Check if player is holding
    private List<Transform> childList;              // List of what player is holding
    private GameObject placeHolder;                 // To prefill the gap for holding Block
    private Transform oldParent;                    // What the Block was belong to
    private int maxStack = 0;                       // For resizing porpose, check for width (Ex. ForLoop IfElse)
    private float gridHeight;                       // GridHeight
    private bool accessAllow;

    public void OnPointerDown( PointerEventData eventData ) {
        onHoldTimer = 0.0f;
        holding = true;
        pointerOffset = new Vector2(transform.position.x, transform.position.y) - eventData.position;
    }

    public void OnBeginDrag( PointerEventData eventData ) {
        maxStack = 0;

        oldParent = transform.parent;
        childList = new List<Transform>();

        // Create new BlockGrid to drag
        Transform newParent = Instantiate(blockGridPrefab, transform.parent).GetComponent<Transform>();
        newParent.position = transform.position;
        newParent.SetParent(oldParent.parent);

        if (onHoldTimer > 0.25f) // Single Block
        {
            childList.Add(transform);
        }
        else // Mutiple Block
        {
            for (int i = transform.GetSiblingIndex(); i < oldParent.childCount; i++)
            {
                childList.Add(oldParent.GetChild(i));
            }
        }

        // Create PlaceHolder
        placeHolder = new GameObject();
        placeHolder.name = "PlaceHolder";
        placeHolderParent = transform.parent;
        placeHolder.AddComponent<RectTransform>().sizeDelta = new Vector2(GameUtility.BLOCK_WIDTH, GameUtility.CONNECTOR_HEIGHT + (GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT) * childList.Count);
        placeHolder.transform.SetSiblingIndex(transform.GetSiblingIndex());
        placeHolder.AddComponent<BlockInfo>().blockType = BlockType.placeHolder;
        VerticalLayoutGroup phcg = placeHolder.AddComponent<VerticalLayoutGroup>();
        phcg.spacing = -GameUtility.CONNECTOR_HEIGHT;
        phcg.childAlignment = TextAnchor.UpperCenter;

        // Transfer All block to the new BlockGrid
        for (int i = 0; i < childList.Count; i++) {
            childList[i].SetParent(newParent.transform);
            childList[i].SetSiblingIndex(i);
            Transform phc = Instantiate(childList[i]);
            phc.SetParent(placeHolder.transform);
            phc.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

        setAlpha(placeHolder.transform, 0.2f);

        // Turning blocksRaycasts off to not disturb OnDrop Event
        canvasGroup.blocksRaycasts = false;
        target = newParent;
        target.GetComponent<CanvasGroup>().blocksRaycasts = false;

        // if the original BlockGrid doesn't have Block anymore, there is no reason to return back
        // Prevent snapping to an seemingly empty space
        if (oldParent.childCount == 0)
        {
            oldParent.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
    }

    public void OnDrag( PointerEventData eventData ) {

        // It's holding BlockGrid, move it up half of height ( BlockGrid's pivot point is on the top )
        target.position = eventData.position + pointerOffset + new Vector2(0f, gridHeight / 2);

        //Debug.DrawLine(eventData.position, eventData.position + new Vector2(0, (GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT) / 2) , Color.red);

        accessAllow = false;

        // placeHolderParent will be null if the cursor is not pointing to any BlockGrid
        if (placeHolderParent != null)
        {
            // Moving the placeHolder to the desired location
            placeHolder.transform.SetParent(placeHolderParent);

            for (int i = 0; i < placeHolderParent.childCount; i++)
            {
                Transform phpc = placeHolderParent.GetChild(i);

                // Desired location: where cursor is pointing > Block's center without connector (Block's center + connector's height/2)
                if (eventData.position.y > phpc.position.y+ GameUtility.CONNECTOR_HEIGHT /2)
                {
                    if (blockInfo.blockType == BlockType.startBlock && i > 0)
                    {
                        accessAllow = false;
                        break;
                    }
                    if (phpc.GetComponent<BlockInfo>().blockType == BlockType.startBlock) break;
                    //Debug.DrawLine(phpc.position + new Vector3(0, GameUtility.CONNECTOR_HEIGHT,1f) / 2, phpc.position + new Vector3(2f, GameUtility.CONNECTOR_HEIGHT / 2,1f), Color.red);
                    placeHolder.transform.SetSiblingIndex(i);
                    accessAllow = true;
                    break;
                }
            }
        }

        if (!accessAllow)
        {
            placeHolder.transform.SetParent(target.parent);
            placeHolder.transform.position = new Vector3(0f, -100f - placeHolder.transform.childCount * (GameUtility.BLOCK_HEIGHT + GameUtility.CONNECTOR_HEIGHT));
            //Debug.Log("null");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Turn blocksRaycasts back on
        target.GetComponent<CanvasGroup>().blocksRaycasts = true;
        canvasGroup.blocksRaycasts = true;

        // Calculate how's the height of the gridBlock should be
        float height = 0f;
        for (int i = 0; i < childList.Count; i++)
        {
            height += GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT;
        }

        if (placeHolderParent != null && accessAllow)
        {
            // Transfer all block to the new BlockGrid
            for (int i = 0; i < childList.Count; i++)
            {
                Transform child = childList[i];
                child.SetParent(placeHolderParent);
                child.SetSiblingIndex(placeHolder.transform.GetSiblingIndex() + i);
            }

            // Destroy the BlockGrid you were holding
            Destroy(target.gameObject);
        }
        else
        {
            placeHolderParent = transform.parent;
        }

        // Destroy PlaceHolder
        if (placeHolder != null)
        {
            placeHolder.transform.SetParent(placeHolder.transform.parent.parent);
            Destroy(placeHolder);
        }

        // Resize the BlockGrid
        Debug.Log(placeHolderParent.childCount);
        Resize( placeHolderParent, maxStack, placeHolderParent.childCount );
        //placeHolderParent.position = new Vector2(placeHolderParent.position.x, placeHolderParent.position.y + height);

        // Resize the previous BlockGrid
        if (oldParent != null)
        {
            if (oldParent.childCount == 0)
            {
                Destroy(oldParent.gameObject);
            }
            else
            {
                Resize(oldParent, 0, oldParent.childCount);
            }
            // oldParent.position = new Vector2(oldParent.position.x, oldParent.position.y + height / 2);
            oldParent = null;
        }

        // Reset value
        holding = false;
        childList = null;

    }

    void Start()
    {
        createBlockGrid( transform.position );
        size = GetComponent<RectTransform>().sizeDelta;
        gridHeight = blockGridPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    void Update() {
        if ( holding ) {
            onHoldTimer += Time.deltaTime;
            //Debug.Log(oldParent);
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

        if ( !parent.StartsWith( "BlockGrid" ) && !parent.StartsWith("PlaceHolder"))
        {
            Transform blockGrid = Instantiate(blockGridPrefab, transform.parent).GetComponent<Transform>();
            blockGrid.position = position + new Vector3(0, gridHeight/2, 0);
            transform.SetParent(blockGrid);
            Resize( blockGrid, 0, 1 );
        }
    }
    void Resize( Transform transform, int row, int column )
    {
        float width = GameUtility.BEAM_WIDTH * row + GameUtility.BLOCK_WIDTH;
        float height = GameUtility.BLOCK_HEIGHT + (GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT) * column;

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2( width, height );
    }

    public List<Transform> GetChildList() {
        if ( childList == null ) {
            List<Transform> selfList = new List<Transform>();
            selfList.Add( transform );

            return selfList;
        }
        return childList;
    }

    public void setAlpha(Transform gameObject ,float alpha)
    {
        Color newColor = new Color(1.0f, 1.0f, 1.0f, alpha);
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Transform child = gameObject.transform.GetChild(i);

            if (child.GetComponent<Image>() != null) 
                child.GetComponent<Image>().color = newColor;

            setAlpha(child, alpha);
        }
        //
        //foreach (SpriteRenderer c in child)
        //{
        //    newColor = c.color;
        //    newColor.a = alpha;
        //    c.color = newColor;
        //}
    }
}
