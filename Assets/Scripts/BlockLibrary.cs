using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockLibrary : MonoBehaviour {
    public GameManager gameManager;
    public Transform content;
    public Dictionary<int, (BlockType, int, Dictionary<string, string>)> blockList;
    public Dictionary<BlockType, int> blockDict = new Dictionary<BlockType, int> {};

    void Awake() {
        if ( gameManager == null ) {
            gameManager = GameUtility.getGameManager();
        }
    }

    void Start() {
        resetBlocks();
    }

    public void resetBlocks() {

        foreach ( Transform child in content ) {
            Destroy( child.gameObject );
        }

        blockDict.Clear();
        float height = 0f;
        foreach ( KeyValuePair<int, (BlockType, int, Dictionary<string, string>)> block in blockList ) {
            Transform spawner = Instantiate( gameManager.spawnerPrefab ).transform;
            spawner.SetParent( content );
            spawner.localScale = Vector3.one;
            spawner.GetComponent<BlockSpawner>().blockType = block.Value.Item1;
            spawner.GetComponent<BlockSpawner>().maxCount = block.Value.Item2;
            if ( blockDict.ContainsKey( block.Value.Item1 ) ) {
                blockDict[block.Value.Item1] += block.Value.Item2;
            }
            else {
                blockDict.Add( block.Value.Item1, block.Value.Item2 );
            }
            foreach ( string key in block.Value.Item3.Keys ) {
                spawner.GetComponent<BlockSpawner>().ModifyArgs( key, block.Value.Item3[key] );
            }

            height += gameManager.getBlockPrefab( block.Value.Item1 ).GetComponent<RectTransform>().sizeDelta.y;
        }
        height += ( blockList.Count + 1 ) * 24f;

        content.GetComponent<RectTransform>().sizeDelta = new Vector2( content.GetComponent<RectTransform>().sizeDelta.x, height );
        GetComponent<ScrollRect>().verticalScrollbar.value = 1f;
    }
}
