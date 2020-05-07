﻿using System.Collections;
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
            case BlockType.forBlock:
                transform.GetChild( 0 ).GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                transform.GetChild( 1 ).GetChild( 0 ).GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
                transform.GetChild( 2 ).GetChild( 0 ).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.5f;
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
