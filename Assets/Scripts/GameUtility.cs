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
    public const float BEAM_WIDTH = 84.0f;
    public const float GRID_HEIGHT = 84.0f;

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
    placeHolder
}