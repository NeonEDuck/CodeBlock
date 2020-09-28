using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    [Header( "BlockPrefab" )]
    public GameObject blockGridPrefab;
    public GameObject setBlockPrefab;
    public GameObject defineBlockPrefab;
    public GameObject forBlockPrefab;
    public GameObject ifBlockPrefab;
    public GameObject startBlockPrefab;
    public GameObject valueBlockPrefab;
    public GameObject moveBlockPrefab;
    public GameObject spawnerPrefab;
    [Header( "MiniPrefab" )]
    public GameObject obstaclePrefab;
    public GameObject playerPrefab;
    public GameObject boxPrefab;

    [Header( "MiniPrefab" )]
    public Transform gameBoard = null;
    public Transform gameView = null;
    public string gameEnv; 
    [HideInInspector] 
    public MiniGameObject[,] gameEnv2d = new MiniGameObject[7,6];

    [Header( "Other" )]
    public Material matHighLight = null;
    public Material matRedHighLight = null;
    public Transform canvas = null;
    public Transform toolBar = null;
    public RectTransform outsideRect = null;

    [Header("Private")]
    //public List<Transform> blockGrids = new List<Transform>();
    public List<Transform> blockGridsUnderPointer = new List<Transform>();
    public Transform player;
    public bool isDraging = false;
    public bool wannaTrash = false;
    public bool showTrashIcon = true;
    public bool gameStarted = false;
    public int whichStack = 0;
    public Transform preSelectedBlockGrids = null;
    public Transform targetBlock = null; // What cursor is holding
    private List<Dictionary<string, object>> gameVariableLists = new List<Dictionary<string, object>>();

    public BlockLibrary blockLibrary = null;
    public VariablesStorage variables = null;

    public void Awake() {

#if UNITY_EDITOR
        if ( GameObject.FindGameObjectWithTag( "VariablesStorage" ) == null ) {
            SceneManager.LoadScene( "GameListScene" );
        }
#endif


        if ( GameObject.FindGameObjectWithTag( "VariablesStorage" ) != null ) {
            variables = GameObject.FindGameObjectWithTag( "VariablesStorage" ).GetComponent<VariablesStorage>();
        }
        if ( blockLibrary == null && GameObject.FindGameObjectWithTag( "BlockLibrary" ) != null ) {
            blockLibrary = GameObject.FindGameObjectWithTag( "BlockLibrary" ).GetComponent<BlockLibrary>();
        }

        if ( variables != null && blockLibrary != null ) {
            var jsonO = MiniJSON.Json.Deserialize( variables.levelJson ) as Dictionary< string, object >;

            gameEnv = jsonO["gameEnv"] as string;
            gameEnv = gameEnv.Replace( "\n", "" );

            Dictionary<int, (BlockType, int)> dict = new Dictionary<int, (BlockType, int)>();

            foreach ( KeyValuePair<string, object> kvp in jsonO["blocksList"] as Dictionary<string, object> ) {
                //dict[dict.Count] = ( GameUtility.StringToBlockType( kvp.Key.ToString() ), (int)kvp.Value );
                dict[dict.Count] = ( GameUtility.StringToBlockType( kvp.Key.ToString()), int.Parse( kvp.Value.ToString() ) );
            }
            blockLibrary.blockList = dict;
        }
        else {

            var jsonO = MiniJSON.Json.Deserialize( "{" +
                "\"blocksList\":{" +
                    "\"StartBlock\":1, " +
                    "\"DefineBlock\":1, " +
                    "\"SetBlock\":1," +
                    "\"ForBlock\":1," +
                "}, " +
                "\"gameEnv\":\"001001010010001000100013120000000001000100\"" +
                "}" ) as Dictionary<string, object>;

            gameEnv = jsonO["gameEnv"] as string;
            gameEnv = gameEnv.Replace( "\n", "" );

            Dictionary<int, (BlockType, int)> dict = new Dictionary<int, (BlockType, int)>();

            foreach ( KeyValuePair<string, object> kvp in jsonO["blocksList"] as Dictionary<string, object> ) {
                //dict[dict.Count] = ( GameUtility.StringToBlockType( kvp.Key.ToString() ), (int)kvp.Value );
                dict[dict.Count] = (GameUtility.StringToBlockType( kvp.Key.ToString() ), int.Parse( kvp.Value.ToString() ));
            }
            blockLibrary.blockList = dict;
        }
        ResetGameView();
    }

    public void ResetGameView() {
        player = null;
        gameEnv2d = new MiniGameObject[7, 6];
        foreach ( Transform child in gameView.transform ) {
            Destroy( child.gameObject );
        }
        Vector3 origin = gameView.position + new Vector3( 0, gameView.GetComponent<RectTransform>().sizeDelta.y, 0 );
        if ( gameEnv != null ) {
            for ( int i = 0; i < gameEnv.Length; i++ ) {
                Transform spawn = null;
                switch ( gameEnv[i] ) {
                    case '0':
                    case 'x':
                        break;
                    case '1':
                    case 'o':
                        spawn = Instantiate( obstaclePrefab, gameView ).transform;
                        break;
                    case '2':
                    case 'p':
                        if ( player == null ) {
                            spawn = Instantiate( playerPrefab, gameView ).transform;
                            player = spawn;
                        }
                        break;
                    case '3':
                    case 'b':
                        spawn = Instantiate( boxPrefab, gameView ).transform;
                        break;
                }
                if ( spawn != null ) {
                    int x = i % 7;
                    int y = (int)Mathf.Floor( i / 7 );
                    gameEnv2d[x, y] = spawn.GetComponent<MiniGameObject>();
                    gameEnv2d[x, y].posInEnv = new Vector2Int( x, y );
                    gameEnv2d[x, y].gameManager = this;
                    spawn.position = origin + new Vector3( ( x + 0.5f ) * 50f, -( y + 0.5f ) * 50f, 0f );
                }
            }
        }
    }

    public void StartGame() {
        if ( !gameStarted ) {
            gameStarted = true;
            ResetGameView();
            List<Tuple<string, Transform, List<object>>> commands = CreateCommand();
            gameVariableLists.Clear();
            Debug.Log( "before enter" );
            StartCoroutine( ExecuteCommand( commands ) );

            //foreach ( Tuple<string, Transform, List<object>> command in commands ) {
            //    switch ( command.Item1 ) {
            //        case "start":
            //            Debug.Log( "start" );
            //            break;
            //        case "define":
            //            Debug.Log( "define " + command.Item3[0] );
            //            break;
            //        case "set":
            //            Debug.Log( "set " + command.Item3[0] + " " + command.Item3[1] );
            //            break;
            //        case "for":
            //            Debug.Log( "for " + command.Item3[0] + " " + command.Item3[1] );
            //            break;
            //        case "if":
            //            Debug.Log( "if " + command.Item3[0] + " " + command.Item3[1] );
            //            break;
            //        case "move":
            //            Debug.Log( "move " + command.Item3[0] + " " + command.Item3[1] );
            //            break;
            //    }
            //}


            gameStarted = false;
        }
    }

    public IEnumerator ExecuteCommand( List<Tuple<string, Transform, List<object>>> commands ) {
        Debug.Log( "enter" );
        gameVariableLists.Add( new Dictionary<string, object>() );
        int local = gameVariableLists.Count - 1;
        string variableName = "";
        object value = null;
        double num;
        bool breakTrigger = false;

        WaitForSeconds wait = new WaitForSeconds( 0.75f );
        foreach ( Tuple<string, Transform, List<object>> command in commands ) {
            if ( command.Item3.Count == 2 && command.Item3[0] == null && command.Item3[1].GetType() == typeof( string ) ) {
                foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                    img.material = matRedHighLight;
                }
                Debug.Log( command.Item3[1].ToString() );
                breakTrigger = true;
            }
            if ( !breakTrigger ) {
                foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                    img.material = matHighLight;
                }

                switch ( command.Item1 ) {
                    case "start":
                        // do nothing
                        break;
                    case "define":
                        variableName = command.Item3[0].ToString();
                        if ( !gameVariableLists[local].ContainsKey( variableName ) ) {
                            gameVariableLists[local].Add( variableName, 0 );
                        }
                        else {
                            gameVariableLists[local][variableName] = 0;
                        }
                        break;
                    case "set":

                        variableName = command.Item3[0].ToString();
                        value = command.Item3[1];

                        for ( int i = local; i >= 0; i-- ) {
                            if ( !gameVariableLists[i].ContainsKey( variableName ) ) {
                                continue;
                            }
                            else {
                                if ( value.GetType() == typeof( string ) ) {
                                    if ( double.TryParse( value.ToString(), out num ) ) {
                                        gameVariableLists[i][variableName] = num;
                                    }
                                    else {
                                        gameVariableLists[i][variableName] = value;
                                    }
                                    Debug.Log( "variable \"" + variableName + "\" exsit!" );
                                }
                                else {
                                    Debug.Log( "Something went wrong!" );
                                }
                                break;
                            }
                        }

                        break;
                    case "for":
                        StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[0] ) );
                        break;
                    case "if":
                        StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[0] ) );
                        StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[1] ) );
                        break;
                    case "move":
                        if ( double.TryParse( command.Item3[0].ToString(), out num ) ) {
                            MiniGameObject playerMini = player.GetComponent<MiniGameObject>();
                            playerMini.Move( (int)num );
                        }
                        break;
                }
            }

            yield return wait;

            foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                img.material = null;
            }

            if ( breakTrigger ) yield break;
        }
    }

    public List<Tuple<string, Transform, List<object>>> CreateCommand( Transform target = null ) {

        List<Tuple<string, Transform, List<object>>> commands = new List<Tuple<string, Transform, List<object>>>();

        if ( target == null ) {
            GameObject[] list = GameObject.FindGameObjectsWithTag( "Start" );
            foreach ( GameObject start in list ) {
                if ( start.transform.parent.parent.name.StartsWith( "BlockSpawner" ) ) continue;
                target = start.transform.parent;
                break;
            }
        }

        if ( target != null ) {
            for ( int i = 0; i < target.childCount; i++ ) {
                BlockInfo blockInfo = target.GetChild(i).GetComponent<BlockInfo>();
                string type = "";
                List<object> infos = new List<object>();
                switch ( blockInfo.blockType ) {
                    case BlockType.StartBlock:
                        type = "start";
                        break;

                    case BlockType.DefineBlock:
                        type = "define";
                        if ( blockInfo.refField[0].GetComponent<TMP_InputField>().text == "" ) {
                            infos.Add( null );
                            infos.Add( "You need a name to define variable!" );
                        }
                        else {
                            infos.Add( blockInfo.refField[0].GetComponent<TMP_InputField>().text );
                            if ( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount > 0 ) {
                                infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                            }
                            else {
                                infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text );
                            }
                        }
                        break;

                    case BlockType.SetBlock:
                        type = "set";
                        if ( blockInfo.refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount == 0 ) {
                            infos.Add( null );
                            infos.Add( "You need to have a variable to set to!" );
                        }
                        else {
                            infos.Add( blockInfo.refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                            if ( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount > 0 ) {
                                infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                            }
                            else {
                                infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text );
                            }
                        }
                        break;

                    case BlockType.ForBlock:
                        type = "for";
                        infos.Add( CreateCommand( blockInfo.refField[0] ) );
                        break;

                    case BlockType.IfBlock:
                        type = "if";
                        infos.Add( CreateCommand( blockInfo.refField[0] ) );
                        infos.Add( CreateCommand( blockInfo.refField[1] ) );
                        break;

                    case BlockType.MoveBlock:
                        type = "move";
                        infos.Add( blockInfo.refField[0].GetComponent<TMP_Dropdown>().value );
                        break;
                }

                commands.Add( new Tuple<string, Transform, List<object>>( type, target.GetChild( i ), infos ) );
            }
        }

        return commands;
    }

    public void BlockGridBlockRaycast( bool enable ) {
        foreach ( Transform bg in BlockGridDropZone.blockGrids ) {
            if ( bg != null ) {
                bg.GetComponent<CanvasGroup>().blocksRaycasts = enable;
            }
        }
    }

    public void ResetAll() {
        foreach ( Transform bg in BlockGridDropZone.blockGrids ) {
            if ( bg != null && bg.parent.name.StartsWith( "GameBoard" ) ) {
                BlockGridDropZone bgdz = bg.GetComponent<BlockGridDropZone>();
                if ( bgdz != null ) {
                    bgdz.InfoReset();
                }
            }
        }
    }

    public void BlockBlockRaycast( bool enable ) {
        List<Transform> removeList = new List<Transform>();
        foreach ( Transform bg in BlockGridDropZone.blockGrids ) {
            if ( bg == null ) {
                removeList.Add( bg );
                continue;
            }
            for ( int i = 0; i < bg.childCount; i++ ) {
                CanvasGroup cg = bg.GetChild( i ).GetComponent<CanvasGroup>();
                if ( cg != null ) {
                    cg.blocksRaycasts = enable;
                }
                Image img = bg.GetChild( i ).GetComponent<Image>();
                if ( img != null ) {
                    img.raycastTarget = enable;
                }
            }
        }
        foreach ( Transform bg in removeList ) {
            BlockGridDropZone.blockGrids.Remove( bg );
        }
    }

    public GameObject getBlockPrefab( BlockType blockType ) {

        GameObject blockPrefab = null;
        switch ( blockType ) {
            case BlockType.SetBlock:
                blockPrefab = setBlockPrefab;
                break;
            case BlockType.DefineBlock:
                blockPrefab = defineBlockPrefab;
                break;
            case BlockType.ForBlock:
                blockPrefab = forBlockPrefab;
                break;
            case BlockType.IfBlock:
                blockPrefab = ifBlockPrefab;
                break;
            case BlockType.StartBlock:
                blockPrefab = startBlockPrefab;
                break;
            case BlockType.ValueBlock:
                blockPrefab = valueBlockPrefab;
                break;
            case BlockType.MoveBlock:
                blockPrefab = moveBlockPrefab;
                break;
        }
        return blockPrefab;
    }

    public void GoBack() {
        SceneManager.LoadScene( "GameListScene" );
    }
}
