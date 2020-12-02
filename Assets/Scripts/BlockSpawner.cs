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
    public bool pressSpawn = false;
    public bool dontTransform = false;
    public int maxCount = 0;
    private int cnt = 0;
    private Dictionary<string, string> args = new Dictionary<string, string>();
    private GameObject blockPrefab;
    public Transform blockGrid;
    private TMP_Text count;
    private List<Transform> blocks = new List<Transform>();

    void Awake() {
        gameManager = GameUtility.getGameManager();
        BlockGridDropZone.blockGrids.Add( transform );
    }

    void Start() {
        bool preBlockGrid = blockGrid != null;
        cnt = maxCount;
        blockPrefab = gameManager.getBlockPrefab( blockType );

        if ( transform.childCount != 0 ) {
            if ( !preBlockGrid ) {
                blockGrid = transform.GetChild( 0 );
            }
            count = transform.GetChild( 1 ).GetComponent<TMP_Text>();
        }
        Debug.Log( maxCount );

        if ( maxCount == 0 ) {
            transform.GetChild( 1 ).gameObject.SetActive( false );
        }

        if (!preBlockGrid ) {
            Vector2 size = blockPrefab.GetComponent<RectTransform>().sizeDelta;
            GetComponent<RectTransform>().sizeDelta = size;
            blockGrid.GetComponent<RectTransform>().sizeDelta = size;
            Debug.Log( size.y - blockGrid.position.y );
            blockGrid.localPosition = blockGrid.localPosition + new Vector3( 0, size.y / 2, 0 );
        }
    }

    void Update()
    {
        blocks.RemoveAll( item => item == null );
        cnt = maxCount - blocks.Count;
        if ( trigger ) {
            spawnBlock();
        }
        else {
            if ( !pressSpawn && blockGrid.childCount > 0 ) {
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

    public void btnSpawnBlock() {
        pressSpawn = true;
        if ( blockGrid.childCount == 0 ) {
            spawnBlock();
        }
        else {
            Destroy( blockGrid.GetChild( 0 ).gameObject );
        }
    }
    private void spawnBlock() {
        if ( ( maxCount == 0 || cnt > 0 ) && blockGrid.childCount == 0 && !gameManager.isDraging ) {
            Transform block = Instantiate( blockPrefab ).transform;
            blocks.Add( block );
            block.SetParent( blockGrid );
            block.localScale = Vector3.one;
            foreach ( CanvasGroup cg in block.GetComponentsInChildren<CanvasGroup>() ) {
                cg.blocksRaycasts = false;
            }
            block.GetComponent<CanvasGroup>().blocksRaycasts = true;


            if ( args.ContainsKey( "direction" ) ) {
                BlockInfo bi = block.GetComponent<BlockInfo>();
                bi.refField[0].GetComponent<TMP_Dropdown>().value = int.Parse( args["direction"] );

                TMP_Text valueText = bi.extraRefField[0].GetComponent<TMP_Text>();

                switch ( args["direction"] ) {
                    case "0":
                        valueText.text = "向前移動";
                        break;
                    case "1":
                        valueText.text = "向右移動";
                        break;
                    case "2":
                        valueText.text = "向後移動";
                        break;
                    case "3":
                        valueText.text = "向左移動";
                        break;
                }
                
            }
            if ( args.ContainsKey( "repeat_n" ) ) {
                BlockInfo bi = block.GetComponent<BlockInfo>();

                Transform valueContainer = bi.refField[1];
                valueContainer.gameObject.SetActive( false );
                valueContainer.GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text = ( args["repeat_n"] == "0" ) ? "infinity" : args["repeat_n"];

                Transform valueText = bi.extraRefField[0];
                valueText.gameObject.SetActive( true );
                valueText.GetComponent<TMP_Text>().text = ( args["repeat_n"] == "0" ) ? "無限" : args["repeat_n"];

            }
            //if ( args.ContainsKey( "direction" ) ) {
            //}
            //if ( args.ContainsKey( "direction" ) ) {
            //}
            //if ( args.ContainsKey( "direction" ) ) {
            //}
        }

        if ( args.ContainsKey( "value_name" ) ) {
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
        else if ( !dontTransform ) {
            blockGrid.GetComponent<BlockGridDropZone>().Resize();
        }
    }
}
