using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlockInfo : MonoBehaviour
{
    public enum BlockType
    {
        setBlcok,
        defineBlock,
        forBlock,
        ifBlock,
        endForBlock,
        endIfBlock
    }

    public BlockType blockType = BlockType.setBlcok;
    public List<TMP_InputField> inputFields;

    public string getBlockCodeString()
    {
        string outputCode = "";
        switch (blockType)
        {
            case BlockType.setBlcok:

                if (inputFields.Count >= 2)
                {
                    outputCode = "set " + inputFields[0].text + " = " + inputFields[1].text + ";";
                }

                break;

            case BlockType.defineBlock:
                if (inputFields.Count == 1)
                {
                    outputCode = "let " + inputFields[0].text + ";";
                }
                else if (inputFields.Count >= 2)
                {
                    outputCode = "let " + inputFields[0].text + " = " + inputFields[1].text + ";";
                }
                break;

            case BlockType.forBlock:
                if (inputFields.Count >= 1)
                {
                    outputCode = "for (int i = 0; i < " + inputFields[0].text + "; i++) {";
                }
                break;
        }
        return outputCode;
    }
}
