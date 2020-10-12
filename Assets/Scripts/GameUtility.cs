using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class GameUtility
{
    public const float CONNECTOR_HEIGHT = 19.0f;
    public const float BLOCK_HEIGHT = 64.0f;
    public const float BLOCK_WIDTH = 304.0f;
    public const float VALUE_HEIGHT = 34.0f;
    public const float VALUE_WIDTH = 84.0f;
    public const float BEAM_WIDTH = 64.0f;
    public const float GRID_HEIGHT = 64.0f;

    static public GameManager getGameManager() {
        GameManager gameManager = null;
        if ( GameObject.FindGameObjectWithTag( "GameManager" ) == null ) {
            Debug.LogError( "No GameManager Found" );
        }
        else {
            gameManager = GameObject.FindGameObjectWithTag( "GameManager" ).GetComponent<GameManager>();
        }
        return gameManager;
    }

    static public BlockType StringToBlockType( string s ) {
        BlockType blockType = BlockType.Other;
        if ( Enum.TryParse( s, out BlockType bt ) ) {
            blockType = bt;
        }
        //BlockType item = (BlockType)Enum.Parse( typeof( BlockType ), s, false );
        return blockType;
    }
};
public enum BlockType
{
    SetBlock,
    DefineBlock,
    ForBlock,
    IfBlock,
    EndForBlock,
    EndIfBlock,
    StartBlock,
    ValueBlock,
    Beam,
    PlaceHolder,
    MoveBlock,
    LogicBlock,
    RepeatBlock,
    Other
}
public enum BlockGridType {
    Block,
    Value,
    Logic
}
