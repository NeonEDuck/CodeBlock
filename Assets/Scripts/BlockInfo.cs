using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockInfo : MonoBehaviour
{

    public BlockType blockType = BlockType.SetBlock;
    public List<Transform> refField;
    public bool[] connectRule = { true, true };

    void Start() {
        switch ( blockType ) {
            case BlockType.IfBlock:
            case BlockType.ForBlock:
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
