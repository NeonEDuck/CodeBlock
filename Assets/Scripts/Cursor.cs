using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Cursor : MonoBehaviour {
    public GameManager gameManager = null;
    public Image cursorImage = null;
    public Sprite stopSprite = null;
    public Sprite trashcanSprite = null;
    private Sprite currentSprite = null;

    void Awake() {
        if ( gameManager == null ) {
            gameManager = GameUtility.getGameManager();
        }
    }

    void Start() {
        if ( cursorImage == null ) {
            cursorImage = transform.GetChild( 0 ).GetComponent<Image>();
        }
    }

    void Update() {
        transform.position = Input.mousePosition;
        
        if ( gameManager.wannaTrash && gameManager.showTrashIcon && gameManager.isDraging ) {
            currentSprite = trashcanSprite;
        }
        else {
            currentSprite = null;
            
        }

        if ( cursorImage.sprite != currentSprite ) {
            cursorImage.sprite = currentSprite;
            cursorImage.color = new Color( 1f, 1f, 1f, ( currentSprite == null ) ? 0f : 1f );
        }

    }
}
