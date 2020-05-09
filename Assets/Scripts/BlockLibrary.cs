using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BlockLibrary : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    public GameManager gameManager;
    public Transform content;
    public List<BlockType> blockList;

    void Awake() {
        if ( gameManager == null ) {
            gameManager = GameUtility.getGameManager();
        }
    }

    void Start() {

        float height = 0f;
        foreach ( BlockType block in blockList ) {
            Transform spawner = Instantiate( gameManager.spawnerPrefab ).transform;
            spawner.SetParent( content );
            spawner.localScale = Vector3.one;
            spawner.GetComponent<BlockSpawner>().blockType = block;
            height += gameManager.getBlockPrefab( block ).GetComponent<RectTransform>().sizeDelta.y;
        }
        height += ( blockList.Count + 1 ) * 24f;

        content.GetComponent<RectTransform>().sizeDelta = new Vector2( content.GetComponent<RectTransform>().sizeDelta.x, height );
    }

    public void OnPointerEnter( PointerEventData eventData ) {

        gameManager.wannaTrash = true;

        if ( !gameManager.isDraging ) {
            gameManager.showTrashIcon = false;
        }

    }
    public void OnPointerExit( PointerEventData eventData ) {

        gameManager.wannaTrash = false;
        gameManager.showTrashIcon = true;

    }
}
