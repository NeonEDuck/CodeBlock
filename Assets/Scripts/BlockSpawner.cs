using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameManager gameManager;
    public BlockType blockType = BlockType.setBlock;
    private GameObject blockPrefab;
    private Transform blockGrid;

    void Awake() {
        gameManager = GameUtility.getGameManager();
        if ( gameManager != null ) {
            gameManager.blockGrids.Add( transform );
        }
    }

    void Start() {
        blockPrefab = gameManager.getBlockPrefab( blockType );

        if ( transform.childCount != 0 ) {
            blockGrid = transform.GetChild( 0 );
        }

        Vector2 size = blockPrefab.GetComponent<RectTransform>().sizeDelta;
        GetComponent<RectTransform>().sizeDelta = size;
        blockGrid.GetComponent<RectTransform>().sizeDelta = size;
        blockGrid.position = blockGrid.position + new Vector3( 0, size.y / 2, 0 );
    }

    void Update()
    {
        if ( blockGrid.childCount == 0 && !gameManager.isDraging ) {
            Transform block = Instantiate( blockPrefab ).transform;
            block.SetParent( blockGrid );
            foreach ( CanvasGroup cg in block.GetComponentsInChildren<CanvasGroup>() ) {
                cg.blocksRaycasts = false;
            }
            block.GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }
}
