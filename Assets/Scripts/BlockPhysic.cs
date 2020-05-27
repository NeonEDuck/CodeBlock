using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlockPhysic : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler {
    public GameObject blockGridPrefab = null;
    public GameObject placeHolderPrefab = null;
    public CanvasGroup canvasGroup = null;
    public BlockInfo blockInfo = null;
    public GameManager gameManager = null;
    [HideInInspector]
    public List<Transform> placeHolderParents = new List<Transform>();            // Where placeHolder is going to be
    private Transform placeHolderParent = null;

    private RectTransform rect;                     // Current Block size
    private Vector2 pointerOffset;                  // From center of the block to cursor
    private float onHoldTimer = 0.0f;               // How much time did player start holding
    private Transform target;                       // What cursor is holding
    private bool holding = false;                   // Check if player is holding
    private List<Transform> childList;              // List of what player is holding
    private GameObject placeHolder;                 // To prefill the gap for holding Block
    private Transform oldParent;                    // What the Block was belong to
    private float gridHeight;                       // GridHeight
    private bool accessAllow;
    private bool additional;

    public void OnPointerDown( PointerEventData eventData ) {
        onHoldTimer = 0.0f;
        holding = true;
        pointerOffset = new Vector2( transform.position.x, transform.position.y ) - eventData.position;
    }

    public void OnBeginDrag( PointerEventData eventData ) {
        gameManager.isDraging = true;

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
        placeHolder = Instantiate( placeHolderPrefab );
        placeHolder.transform.SetSiblingIndex(transform.GetSiblingIndex());

        float height = GameUtility.CONNECTOR_HEIGHT;

        // Transfer All block to the new BlockGrid
        for (int i = 0; i < childList.Count; i++) {
            childList[i].SetParent(newParent.transform);
            childList[i].SetSiblingIndex(i);
            height += childList[i].GetComponent<RectTransform>().sizeDelta.y - GameUtility.CONNECTOR_HEIGHT;
            Transform phc = Instantiate(childList[i]);
            phc.SetParent(placeHolder.transform);
            phc.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        //placeHolder.GetComponent<RectTransform>().sizeDelta = new Vector2( GameUtility.BLOCK_WIDTH, height );
        SizeFitter.CheckForChanges( placeHolder.transform );

        // PlaceHolder's transparent effect && turn off raycast
        Color newColor = new Color( 1.0f, 1.0f, 1.0f, 0.2f );

        foreach ( Image img in placeHolder.GetComponentsInChildren<Image>() ) {
            img.color = newColor;
            img.raycastTarget = false;
        }
        foreach ( CanvasGroup cg in placeHolder.GetComponentsInChildren<CanvasGroup>() ) {
            cg.blocksRaycasts = false;
            cg.interactable = false;
        }

        // Turning blocksRaycasts off to not disturb OnDrop Event
        gameManager.BlockBlockRaycast( false );
        target = newParent;
        BlockRaycastWithChildGrid(target, false);

        foreach ( BlockGridDropZone cbg in GetComponentsInChildren<BlockGridDropZone>() ) {
            gameManager.blockGridsUnderPointer.Remove( cbg.transform );
        }

        // if the original BlockGrid doesn't have Block anymore, there is no reason to return back
        // Prevent snapping to an seemingly empty space
        if ( oldParent.childCount == 0 && oldParent.GetComponent<BlockGridInfo>().blockGridType == BlockGridType.Block && oldParent.parent == gameManager.gameBoard ) {
            oldParent.gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            gameManager.blockGridsUnderPointer.Remove( oldParent );
            foreach ( BlockGridDropZone opc in oldParent.GetComponentsInChildren<BlockGridDropZone>() ) {
                gameManager.blockGridsUnderPointer.Remove( opc.transform );
            }
            gameManager.blockGridsUnderPointer.Remove( transform.parent );
        }
        gameManager.blockGridsUnderPointer.Clear();

        gameManager.ResetAll();
    }

    public void OnDrag( PointerEventData eventData ) {


        // It's holding BlockGrid, move it up half of height ( BlockGrid's pivot point is on the top )
        target.position = eventData.position + pointerOffset + new Vector2(0f, rect.sizeDelta.y / 2);

        //Debug.DrawLine(eventData.position, eventData.position + new Vector2(0, (GameUtility.BLOCK_HEIGHT - GameUtility.CONNECTOR_HEIGHT) / 2) , Color.red);
        
        accessAllow = false;

        // placeHolderParent will be null if the cursor is not pointing to any BlockGrid
        if ( gameManager.blockGridsUnderPointer.Count != 0 ) {

            int _p;
            int biggestPriority = -1;

            foreach ( Transform bg in gameManager.blockGridsUnderPointer ) {
                if ( !bg.transform.IsChildOf( transform.parent ) && bg.GetComponent<BlockGridInfo>().priority > biggestPriority) {
                    _p = bg.GetComponent<BlockGridInfo>().priority;
                    placeHolderParent = bg;
                    biggestPriority = _p;
                }
            }
            if ( placeHolderParent != gameManager.preSelectedBlockGrids ) {
                //Debug.Log( placeHolderParent );
                //Debug.Log( gameManager.preSelectedBlockGrids );
                gameManager.whichStack = -1;
                additional = false;
            }

            //int priority = 0;
            //foreach ( Transform php in placeHolderParents ) {
            //    if ( php.GetComponent<BlockGridInfo>().priority >= priority ) {
            //        placeHolderParent = php;
            //    }
            //}

            BlockGridType phpbt = placeHolderParent.GetComponent<BlockGridInfo>().blockGridType;

            switch ( phpbt ) {
                case BlockGridType.Block:
                    Debug.Log( "Block" );
                    if ( blockInfo.blockType == BlockType.ValueBlock || blockInfo.blockType == BlockType.LogicBlock ) {
                        goto jump_out;
                    }
                    break;
                case BlockGridType.Value:
                    Debug.Log( "Value" );
                    if ( blockInfo.blockType != BlockType.ValueBlock ) {
                        goto jump_out;
                    }
                    break;
                case BlockGridType.Logic:
                    Debug.Log( "Logic" );
                    if ( blockInfo.blockType != BlockType.LogicBlock ) {
                        goto jump_out;
                    }
                    break;
            }

            if ( placeHolderParent.parent.name.StartsWith( "Beam" ) ) {
                if ( target.GetChild(0).GetComponent<BlockInfo>().connectRule[0] == false || target.GetChild( target.childCount - 1 ).GetComponent<BlockInfo>().connectRule[1] == false ) {
                    accessAllow = false;
                    goto jump_out;
                }
            }


            accessAllow = true;
            // Moving the placeHolder to the desired location
            if ( gameManager.whichStack == -1 ) {
                for ( int i = 0; i < placeHolderParent.childCount; i++ ) {
                    Transform phpc = placeHolderParent.GetChild( i );
                    if ( phpc == placeHolder.transform ) continue;
                    // Desired location: where cursor is pointing > Block's center without connector (Block's center + connector's height/2)
                    if ( eventData.position.y > phpc.position.y + GameUtility.CONNECTOR_HEIGHT / 2 ) {
                        if ( ( blockInfo.connectRule[0] == false && i > 0 ) || 
                            ( i < placeHolderParent.childCount && phpc.GetComponent<BlockInfo>().connectRule[0] == false ) ||
                            ( blockInfo.connectRule[1] == false) || 
                            ( i > 0 && placeHolderParent.GetChild( i - 1 ).GetComponent<BlockInfo>().connectRule[1] == false ) ) {
                            accessAllow = false;
                            break;
                        }
                        gameManager.whichStack = i;
                        //Debug.DrawLine(phpc.position + new Vector3(0, GameUtility.CONNECTOR_HEIGHT,1f) / 2, phpc.position + new Vector3(2f, GameUtility.CONNECTOR_HEIGHT / 2,1f), Color.red);
                        break;
                    }
                }

                if ( accessAllow && gameManager.whichStack == -1 && placeHolderParent.childCount > 0 ) {

                    BlockInfo phplcbi = placeHolderParent.GetChild( placeHolderParent.childCount - 1 ).GetComponent<BlockInfo>();
                        
                    if ( phplcbi.gameObject == placeHolder ) {
                        if ( placeHolderParent.childCount > 1 ) {
                            phplcbi = placeHolderParent.GetChild( placeHolderParent.childCount - 2 ).GetComponent<BlockInfo>();
                        }
                        else {
                            goto connectRuleSkip;
                        }
                    }

                    if ( phplcbi.connectRule[1] == false ) {
                        if ( phplcbi.connectRule[0] == false ) {
                            accessAllow = false;
                        }
                        else {
                            gameManager.whichStack = placeHolderParent.childCount - 1;
                        }
                    }
                    if ( blockInfo.connectRule[0] == false ) {
                        accessAllow = false;
                    }

                    if ( blockInfo.connectRule[0] == false ) {
                        accessAllow = false;
                        gameManager.whichStack = -1;
                    }
                }
            connectRuleSkip:;

            }
            else  {
                if ( gameManager.whichStack != 0 ) {
                    Transform phpc = placeHolderParent.GetChild( gameManager.whichStack - 1 );
                    if ( eventData.position.y > phpc.position.y + ( phpc.GetComponent<RectTransform>().sizeDelta.y / 2 - GameUtility.BLOCK_HEIGHT / 2 ) ) {
                        if ( blockInfo.connectRule[1] == true && phpc.GetComponent<BlockInfo>().connectRule[0] == true ) {
                            gameManager.whichStack--;
                            Debug.Log( "Up" );
                            goto jump_out;
                        }
                    }
                }
                if ( gameManager.whichStack < placeHolderParent.childCount - 1 ) {
                    Transform phpc = placeHolderParent.GetChild( gameManager.whichStack + 1 );
                    if ( eventData.position.y < phpc.position.y - ( phpc.GetComponent<RectTransform>().sizeDelta.y / 2 - GameUtility.BLOCK_HEIGHT / 2 )  ) {
                        if ( blockInfo.connectRule[0] == true && phpc.GetComponent<BlockInfo>().connectRule[1] == true ) {
                            gameManager.whichStack++;
                            Debug.Log( "Down" );
                            goto jump_out;
                        }
                    }
                }
            }

            if ( gameManager.whichStack != placeHolderParent.childCount - 1 && gameManager.whichStack != -1 ) {
                additional = true;
            }

            if ( additional ) {
                placeHolderParent.GetComponent<BlockGridDropZone>().Resize();
            }

        }
        else {
            gameManager.preSelectedBlockGrids = null;
            additional = false;
        }

    jump_out:
        if (!accessAllow)
        {
            placeHolder.transform.SetParent( gameManager.gameBoard );
            placeHolder.transform.position = new Vector3(0f, -100f - placeHolder.transform.childCount * (GameUtility.BLOCK_HEIGHT + GameUtility.CONNECTOR_HEIGHT));
        }
        else {
            placeHolder.transform.SetParent( placeHolderParent );
            placeHolder.transform.localScale = Vector3.one;
            placeHolder.transform.SetSiblingIndex( gameManager.whichStack );
        }
        if ( ( gameManager.blockGridsUnderPointer.Count == 0 && placeHolderParent != null ) || ( gameManager.preSelectedBlockGrids != placeHolderParent ) ) {
            placeHolderParent.GetComponent<BlockGridDropZone>().Resize();
        }
        if ( gameManager.blockGridsUnderPointer.Count > 0 ) {
            if ( gameManager.preSelectedBlockGrids != null ) gameManager.preSelectedBlockGrids.GetComponent<BlockGridDropZone>().Resize();
            gameManager.preSelectedBlockGrids = placeHolderParent;
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
        if ( !gameManager.wannaTrash ) {
            if ( placeHolderParent != null && accessAllow ) {
                // Transfer all block to the new BlockGrid
                for ( int i = 0; i < childList.Count; i++ ) {
                    Transform child = childList[i];
                    child.SetParent( placeHolderParent );
                    child.SetSiblingIndex( placeHolder.transform.GetSiblingIndex() + i );
                }
                BlockRaycastWithChildGrid( placeHolderParent, true );

                // Destroy the BlockGrid you were holding
                Destroy( target.gameObject );
            }
            else {
                BlockRaycastWithChildGrid( target, true );
                placeHolderParent = transform.parent;
            }
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

        if ( gameManager.wannaTrash ) {
            Destroy( target.gameObject );
        }

        // Reset value
        holding = false;
        childList = null;
        placeHolderParent = null;
    }

    void Start()
    {
        createBlockGrid( transform.position );
        rect = GetComponent<RectTransform>();
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

    public void setAlpha(Transform gameObject ,float alpha)
    {
        Color newColor = new Color(1.0f, 1.0f, 1.0f, alpha);

        foreach ( Image img in gameObject.GetComponentsInChildren<Image>() ) {
            img.color = newColor;
        }
    }

    public void BlockRaycastWithChildGrid( Transform target, bool enable ) {
        BlockGridDropZone[] bgs = target.GetComponentsInChildren<BlockGridDropZone>();
        foreach ( BlockGridDropZone bg in bgs ) {
            bg.transform.GetComponent<CanvasGroup>().blocksRaycasts = enable;
        }
    }
}
