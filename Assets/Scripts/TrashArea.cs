using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TrashArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public GameManager gameManager;

    void Awake() {
        if ( gameManager == null ) {
            gameManager = GameUtility.getGameManager();
        }
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
