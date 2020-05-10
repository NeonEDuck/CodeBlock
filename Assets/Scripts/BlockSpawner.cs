using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameManager gameManager;
    public BlockType blockType = BlockType.setBlock;
    public bool trigger = true;
    private Dictionary<string, string> args = new Dictionary<string, string>();
    private GameObject blockPrefab;
    private Transform blockGrid;
    private List<Transform> blocks = new List<Transform>();

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
        Debug.Log( size.y - blockGrid.position.y );
        blockGrid.position = blockGrid.position + new Vector3( 0, size.y / 2 * 0.75f, 0 );
    }

    void Update()
    {
        if ( trigger ) {
            if ( blockGrid.childCount == 0 && !gameManager.isDraging ) {
                Transform block = Instantiate( blockPrefab ).transform;
                blocks.Add( block );
                block.SetParent( blockGrid );
                block.localScale = Vector3.one;
                foreach ( CanvasGroup cg in block.GetComponentsInChildren<CanvasGroup>() ) {
                    cg.blocksRaycasts = false;
                }
                block.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }

            if ( args.ContainsKey("value_name") ) {
                foreach ( Transform block in blocks ) {
                    if ( block == null ) {
                        continue;
                    }
                    TMP_Text text = block.GetComponent<BlockInfo>().inputFields[0].GetComponent<TMP_Text>();
                    if ( text != null ) {
                        text.text = args["value_name"];
                    }
                }
            }
        }
        else {
            if ( blockGrid.childCount > 0 ) {
                Destroy( blockGrid.GetChild( 0 ).gameObject );
            }
        }
    }

    public void ModifyArgs( string key, string value ) {
        if ( args.ContainsKey( key ) ) {
            args[key] = value;
        }
        else {
            args.Add( key, value );
        }
    }
}
