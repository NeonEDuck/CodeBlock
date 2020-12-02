using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockInfo : MonoBehaviour
{

    public BlockType blockType = BlockType.SetBlock;
    public List<Transform> refField;
    public List<Transform> extraRefField;
    public bool[] connectRule = { true, true };
    public bool hasElse = true;

    void Start() {
        switch ( blockType ) {
            case BlockType.IfBlock:
            case BlockType.ForBlock:
            case BlockType.RepeatBlock:
                for ( int i = 1; i < transform.childCount; i += 2 ) {
                    transform.GetChild( i - 1 ).GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                    transform.GetChild( i ).GetChild( 0 ).GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                }

                transform.GetChild( transform.childCount - 1 ).GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                break;
            case BlockType.PlaceHolder:
                break;
            default:
                transform.GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                break;
        }
    }

    public void toggleElse() {
        hasElse = !hasElse;
        extraRefField[0].gameObject.SetActive( hasElse );
        extraRefField[1].gameObject.SetActive( hasElse );
        extraRefField[2].rotation = Quaternion.Euler(0,0, hasElse?0:180 );

        //transform.GetComponent<RectTransform>().sizeDelta = new Vector2( transform.GetComponent<RectTransform>().sizeDelta.x, hasElse ? 244 : 154 );
        //float height = 0f;
        //for ( int i = 0; i < transform.childCount; i++ ) {
        //height += transform.GetChild(i).GetComponent<RectTransform>().sizeDelta.y - GameUtility.CONNECTOR_HEIGHT;
        //}
        //transform.GetComponent<RectTransform>().sizeDelta = new Vector2( transform.GetComponent<RectTransform>().sizeDelta.x, 10 );

        if ( !hasElse ) {
            extraRefField[0].SetParent( transform.GetChild( transform.childCount - 1 ) );
            extraRefField[1].SetParent( transform.GetChild( transform.childCount - 1 ) );
            foreach ( Transform child in refField[1] ) {
                Destroy( child.gameObject );
            }
        }
        else {
            extraRefField[0].SetParent( transform );
            extraRefField[1].SetParent( transform );
            transform.GetChild( transform.childCount - 3 ).SetAsLastSibling();
            refField[1].GetComponent<BlockGridDropZone>().Resize();
        }
        //transform.parent.GetComponent<BlockGridDropZone>().Resize();

        foreach ( Transform child in transform.parent.GetComponent<BlockGridDropZone>().gameManager.gameContent ) {
            child.GetComponent<BlockGridDropZone>().Resize();
        }
    }

    //public string getBlockCodeString()
    //{
    //    string outputCode = "";
    //    switch (blockType)
    //    {
    //        case BlockType.setBlock:

    //            if (refField.Count >= 2)
    //            {
    //                outputCode = "set " + refField[0].text + " = " + refField[1].text + ";";
    //            }

    //            break;

    //        case BlockType.defineBlock:
    //            if (refField.Count == 1)
    //            {
    //                outputCode = "let " + refField[0].text + ";";
    //            }
    //            else if (refField.Count >= 2)
    //            {
    //                outputCode = "let " + refField[0].text + " = " + refField[1].text + ";";
    //            }
    //            break;

    //        case BlockType.forBlock:
    //            if (refField.Count >= 1)
    //            {
    //                outputCode = "for (int i = 0; i < " + refField[0].text + "; i++) {";
    //            }
    //            break;
    //    }
    //    return outputCode;
    //}
}
