using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelButtonHolderStyler : MonoBehaviour {
    public GridLayoutGroup gridLayout;

    private void Start() {
        if ( gridLayout != null ) {

            float width = GetComponent<RectTransform>().rect.width;
            float gap = ( width - ( gridLayout.cellSize.x * 10 ) - gridLayout.padding.left - gridLayout.padding.right ) / 9 - 0.0001f;

            gridLayout.spacing = new Vector2(gap, gap);
        }
    }
}
