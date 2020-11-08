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

    static public bool OptionTextToNumber( string s, out int num ) {
        num = 0;
        if      ( s == "小人偶" ) {
            num = 2;
        }
        else if ( s == "箱子" ) {
            num = 3;
        }
        else if ( s == "旗子" ) {
            num = 4;
        }
        else if ( s == "牆" ) {
            num = 1;
        }
        else if ( s == "洞" ) {
            num = 5;
        }
        else if ( s == "壓力板" ) {
            num = 6;
        }
        else if ( s == "門" ) {
            num = 7;
        }
        return num > 0;
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
    TurnBlock,
    BreakBlock,
    LogicSelectorBlock,
    Other
}
public enum BlockGridType {
    Block,
    Value,
    Logic
}

public enum Direction {
    UP,
    RIGHT,
    DOWN,
    LEFT
}