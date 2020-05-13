using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class GameUtility
{
    public const float CONNECTOR_HEIGHT = 24.0f;
    public const float BLOCK_HEIGHT = 84.0f;
    public const float BLOCK_WIDTH = 354.0f;
    public const float VALUE_HEIGHT = 44.0f;
    public const float VALUE_WIDTH = 104.0f;
    public const float BEAM_WIDTH = 84.0f;
    public const float GRID_HEIGHT = 84.0f;

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
};
public enum BlockType
{
    setBlock,
    defineBlock,
    forBlock,
    ifBlock,
    endForBlock,
    endIfBlock,
    startBlock,
    valueBlock,
    beam,
    placeHolder,
    moveBlock,
    other
}
public enum BlockGridType {
    Block,
    Value,
    Logic
}