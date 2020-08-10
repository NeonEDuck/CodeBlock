using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockLibrary : MonoBehaviour {
    public GameManager gameManager;
    public Transform content;
    public Dictionary<int, (BlockType, int)> blockList;

    void Awake() {
        if ( gameManager == null ) {
            gameManager = GameUtility.getGameManager();
        }
    }

    void Start() {

        float height = 0f;
        foreach ( KeyValuePair<int, (BlockType, int)> block in blockList ) {
            Transform spawner = Instantiate( gameManager.spawnerPrefab ).transform;
            spawner.SetParent( content );
            spawner.localScale = Vector3.one;
            spawner.GetComponent<BlockSpawner>().blockType = block.Value.Item1;
            spawner.GetComponent<BlockSpawner>().maxCount = block.Value.Item2;
            height += gameManager.getBlockPrefab( block.Value.Item1 ).GetComponent<RectTransform>().sizeDelta.y;
        }
        height += ( blockList.Count + 1 ) * 24f;

        content.GetComponent<RectTransform>().sizeDelta = new Vector2( content.GetComponent<RectTransform>().sizeDelta.x, height );
    }
}
