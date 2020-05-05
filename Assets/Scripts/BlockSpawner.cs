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

        switch ( blockType ) {
            case BlockType.setBlock:
                blockPrefab = gameManager.setBlockPrefab;
                break;
            case BlockType.defineBlock:
                blockPrefab = gameManager.defineBlockPrefab;
                break;
            case BlockType.forBlock:
                blockPrefab = gameManager.forBlockPrefab;
                break;
            case BlockType.ifBlock:
                blockPrefab = gameManager.ifBlockPrefab;
                break;
            case BlockType.startBlock:
                blockPrefab = gameManager.startBlockPrefab;
                break;
        }

        if ( transform.childCount != 0 ) {
            blockGrid = transform.GetChild( 0 );
        }

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
