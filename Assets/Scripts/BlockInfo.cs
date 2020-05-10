using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockInfo : MonoBehaviour
{

    public BlockType blockType = BlockType.setBlock;
    public List<Transform> inputFields;
    public bool[] connectRule = { true, true };

    void Start() {
        switch ( blockType ) {
            case BlockType.ifBlock:
            case BlockType.forBlock:
                for ( int i = 1; i < transform.childCount; i += 2 ) {
                    transform.GetChild( i - 1 ).GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                    transform.GetChild( i ).GetChild( 0 ).GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                }

                transform.GetChild( transform.childCount - 1 ).GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                break;
            case BlockType.placeHolder:
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

    //            if (inputFields.Count >= 2)
    //            {
    //                outputCode = "set " + inputFields[0].text + " = " + inputFields[1].text + ";";
    //            }

    //            break;

    //        case BlockType.defineBlock:
    //            if (inputFields.Count == 1)
    //            {
    //                outputCode = "let " + inputFields[0].text + ";";
    //            }
    //            else if (inputFields.Count >= 2)
    //            {
    //                outputCode = "let " + inputFields[0].text + " = " + inputFields[1].text + ";";
    //            }
    //            break;

    //        case BlockType.forBlock:
    //            if (inputFields.Count >= 1)
    //            {
    //                outputCode = "for (int i = 0; i < " + inputFields[0].text + "; i++) {";
    //            }
    //            break;
    //    }
    //    return outputCode;
    //}
}
