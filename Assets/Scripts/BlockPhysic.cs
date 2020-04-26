using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlockPhysic : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {
    public GameObject blockGridPrefab = null;
    public CanvasGroup canvasGroup = null;
    public BlockInfo blockInfo = null;
    public GameManager gameManager = null;
    //[HideInInspector]
    public List<Transform> placeHolderParents = new List<Transform>();            // Where placeHolder is going to be
    private Transform placeHolderParent = null;

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
        gameManager.isDraging = true;

        maxStack = 0;

        oldParent = transform.parent;
        childList = new List<Transform>();

        // Create new BlockGrid to drag
        Transform newParent = Instantiate(blockGridPrefab, transform.parent).GetComponent<Transform>();
        newParent.position = transform.position;
        newParent.SetParent(gameManager.gameBoard);

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
        //placeHolderParents.Add( transform.parent );
        placeHolder.AddComponent<RectTransform>().sizeDelta = new Vector2(GameUtility.BLOCK_WIDTH, GameUtility.CONNECTOR_HEIGHT + (GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT) * childList.Count);
        placeHolder.transform.SetSiblingIndex(transform.GetSiblingIndex());
        placeHolder.AddComponent<BlockInfo>().blockType = BlockType.placeHolder;
        VerticalLayoutGroup phcg = placeHolder.AddComponent<VerticalLayoutGroup>();
        phcg.spacing = -GameUtility.CONNECTOR_HEIGHT;
        phcg.childAlignment = TextAnchor.UpperCenter;
        phcg.childControlHeight = false;
        phcg.childControlWidth = false;

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
        gameManager.BlockBlockRaycast( false );
        target = newParent;
        BlockRaycastWithChildGrid(target, false);

        // if the original BlockGrid doesn't have Block anymore, there is no reason to return back
        // Prevent snapping to an seemingly empty space
        if ( oldParent.childCount == 0 && oldParent.GetComponent<BlockGridInfo>().blockGridType == BlockGridType.Block && oldParent.parent == gameManager.gameBoard ) {
            oldParent.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            gameManager.blockGridsUnderPointer.Remove( oldParent );
            gameManager.blockGridsUnderPointer.Remove( transform.parent );
        }

        gameManager.ResetAll();
    }

    public void OnDrag( PointerEventData eventData ) {

        // It's holding BlockGrid, move it up half of height ( BlockGrid's pivot point is on the top )
        target.position = eventData.position + pointerOffset + new Vector2(0f, size.y / 2);

        //Debug.DrawLine(eventData.position, eventData.position + new Vector2(0, (GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT) / 2) , Color.red);
        
        accessAllow = false;

        // placeHolderParent will be null if the cursor is not pointing to any BlockGrid
        if ( gameManager.blockGridsUnderPointer.Count != 0 ) {

            int _p = 0;
            int biggestPriority = -1;

            foreach ( Transform bg in gameManager.blockGridsUnderPointer ) {
                if ( bg.GetComponent<BlockGridInfo>().priority > biggestPriority) {
                    _p = bg.GetComponent<BlockGridInfo>().priority;
                    placeHolderParent = bg;
                    biggestPriority = _p;
                }
            }

            Debug.Log( biggestPriority );

            //int priority = 0;
            //foreach ( Transform php in placeHolderParents ) {
            //    if ( php.GetComponent<BlockGridInfo>().priority >= priority ) {
            //        placeHolderParent = php;
            //    }
            //}

            BlockGridType phpbt = placeHolderParent.GetComponent<BlockGridInfo>().blockGridType;

            switch ( phpbt ) {
                case BlockGridType.Block:
                    if ( blockInfo.blockType == BlockType.valueBlock ) {
                        goto jump_out;
                    }
                    break;
                case BlockGridType.Value:
                    if ( blockInfo.blockType != BlockType.valueBlock ) {
                        goto jump_out;
                    }
                    break;
                case BlockGridType.Logic:
                    if ( blockInfo.blockType != BlockType.valueBlock ) {
                        goto jump_out;
                    }
                    break;
            }

            accessAllow = true;
            // Moving the placeHolder to the desired location
            placeHolder.transform.SetParent(placeHolderParent);

            for (int i = 0; i < placeHolderParent.childCount; i++)
            {
                Transform phpc = placeHolderParent.GetChild(i);

                // Desired location: where cursor is pointing > Block's center without connector (Block's center + connector's height/2)
                if (eventData.position.y > phpc.position.y+ GameUtility.CONNECTOR_HEIGHT /2)
                {
                    if ( ( blockInfo.connectRule[0] == false && i > 0 ) || ( phpc.GetComponent<BlockInfo>().connectRule[0] == false ) ||
                        ( blockInfo.connectRule[1] == false && i == 0 && placeHolderParent.childCount > 1 ) || ( i > 0 && placeHolderParent.GetChild( i-1 ).GetComponent<BlockInfo>().connectRule[1] == false ) ) {
                        accessAllow = false;
                        break;
                    }

                    //Debug.DrawLine(phpc.position + new Vector3(0, GameUtility.CONNECTOR_HEIGHT,1f) / 2, phpc.position + new Vector3(2f, GameUtility.CONNECTOR_HEIGHT / 2,1f), Color.red);
                    placeHolder.transform.SetSiblingIndex(i);
                    break;
                }
            }
        }
        jump_out:

        if (!accessAllow)
        {
            placeHolder.transform.SetParent( gameManager.gameBoard );
            placeHolder.transform.position = new Vector3(0f, -100f - placeHolder.transform.childCount * (GameUtility.BLOCK_HEIGHT + GameUtility.CONNECTOR_HEIGHT));
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        //gameManager.BlockGridBlockRaycast( false );
        gameManager.isDraging = false;

        // Turn blocksRaycasts back on
        gameManager.BlockBlockRaycast( true );

        // Calculate how's the height of the gridBlock should be
        float height = 0f;
        for (int i = 0; i < childList.Count; i++)
        {
            height += GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT;
        }

        if ( placeHolderParent != null && accessAllow)
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
        else {
            BlockRaycastWithChildGrid( target, true );
            placeHolderParent = transform.parent;
        }

        // Destroy PlaceHolder
        if (placeHolder != null)
        {
            placeHolder.transform.SetParent( gameManager.gameBoard );
            Destroy(placeHolder.gameObject);
        }

        // Resize the BlockGrid
        //Debug.Log(placeHolderParent.childCount);
        //if ( placeHolderParent.GetComponent<BlockGridInfo>().blockGridType == BlockGridType.Block ) {
        //    placeHolderParent.GetComponent<BlockGridDropZone>().Resize();
        //}
        //placeHolderParent.position = new Vector2(placeHolderParent.position.x, placeHolderParent.position.y + height);

        // Resize the previous BlockGrid
        if (oldParent != null)
        {
            if ( oldParent.GetComponent<BlockGridInfo>().blockGridType == BlockGridType.Block && oldParent.parent == gameManager.gameBoard ) {
                if ( oldParent.childCount == 0 ) {
                    Destroy( oldParent.gameObject );
                }
                //else {
                //    oldParent.GetComponent<BlockGridDropZone>().Resize();
                //}
            }
            // oldParent.position = new Vector2(oldParent.position.x, oldParent.position.y + height / 2);
            oldParent = null;
        }

        gameManager.ResetAll();

        // Reset value
        holding = false;
        childList = null;
        placeHolderParent = null;
    }

    void Start()
    {
        createBlockGrid( transform.position );
        size = GetComponent<RectTransform>().sizeDelta;
        gridHeight = blockGridPrefab.GetComponent<RectTransform>().sizeDelta.y;
    }

    void Awake() {
        if ( gameManager == null ) {
            if ( GameObject.FindGameObjectWithTag( "GameManager" ) == null ) {
                Debug.LogError( "No GameManager Found" );
            }
            else {
                gameManager = GameObject.FindGameObjectWithTag( "GameManager" ).GetComponent<GameManager>();
            }
        }
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

        if ( parent.StartsWith( "GameBoard" ) ) {
            Transform blockGrid = Instantiate(blockGridPrefab, transform.parent).GetComponent<Transform>();
            blockGrid.position = position + new Vector3(0, gridHeight/2, 0);
            transform.SetParent(blockGrid);
            blockGrid.GetComponent<BlockGridDropZone>().InfoReset();
        }
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

    public void BlockRaycastWithChildGrid( Transform target, bool enable ) {
        BlockGridDropZone[] bgs = target.GetComponentsInChildren<BlockGridDropZone>();
        foreach ( BlockGridDropZone bg in bgs ) {
            bg.transform.GetComponent<CanvasGroup>().blocksRaycasts = enable;
        }
    }
}
