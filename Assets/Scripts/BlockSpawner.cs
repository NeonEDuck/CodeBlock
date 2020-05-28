using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameManager gameManager;
    public BlockType blockType = BlockType.SetBlock;
    public bool trigger = true;
    public int maxCount = 0;
    private int cnt = 0;
    private Dictionary<string, string> args = new Dictionary<string, string>();
    private GameObject blockPrefab;
    private Transform blockGrid;
    private TMP_Text count;
    private List<Transform> blocks = new List<Transform>();

    void Awake() {
        gameManager = GameUtility.getGameManager();
        if ( gameManager != null ) {
            gameManager.blockGrids.Add( transform );
        }
    }

    void Start() {
        cnt = maxCount;
        blockPrefab = gameManager.getBlockPrefab( blockType );

        if ( transform.childCount != 0 ) {
            blockGrid = transform.GetChild( 0 );
            count = transform.GetChild( 1 ).GetComponent<TMP_Text>();
        }
        Debug.Log( maxCount );

        if ( maxCount == 0 ) {
            transform.GetChild( 1 ).gameObject.SetActive( false );
        }

        Vector2 size = blockPrefab.GetComponent<RectTransform>().sizeDelta;
        GetComponent<RectTransform>().sizeDelta = size;
        blockGrid.GetComponent<RectTransform>().sizeDelta = size;
        Debug.Log( size.y - blockGrid.position.y );
        blockGrid.position = blockGrid.position + new Vector3( 0, size.y / 2, 0 );
    }

    void Update()
    {
        blocks.RemoveAll( item => item == null );
        cnt = maxCount - blocks.Count;
        if ( trigger ) {
            if ( ( maxCount == 0 || cnt > 0 ) && blockGrid.childCount == 0 && !gameManager.isDraging ) {
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

                    BlockSizeManage vbm = block.GetComponent<BlockSizeManage>();
                    if ( vbm != null ) {
                        vbm.ChangeText( args["value_name"] );
                    }

                    //Transform inputField = block.GetComponent<BlockInfo>().refField[0];
                    //if ( inputField != null ) {
                    //    inputField.GetComponent<TMP_Text>().text = args["value_name"];
                    //    inputField.GetComponent<TMP_Text>().text = args["value_name"];
                    //}
                }
            }

            if ( !args.ContainsKey( "value_name" ) || args["value_name"].Length == 0 ) {
                blockGrid.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            }
            else {
                blockGrid.GetComponent<BlockGridDropZone>().Resize();
            }
        }
        else {
            if ( blockGrid.childCount > 0 ) {
                Destroy( blockGrid.GetChild( 0 ).gameObject );
            }
        }
        count.text = ( cnt + ( ( blockGrid.childCount == 0 )? 0:1 ) ).ToString();
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
