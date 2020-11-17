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
    public GameObject repeatBlockPrefab;
    public GameObject startBlockPrefab;
    public GameObject valueBlockPrefab;
    public GameObject moveBlockPrefab;
    public GameObject turnBlockPrefab;
    public GameObject breakBlockPrefab;
    public GameObject logicBlockPrefab;
    public GameObject logicSelectorBlockPrefab;
    public GameObject spawnerPrefab;
    [Header( "MiniPrefab" )]
    public GameObject obstaclePrefab;
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject flagPrefab;
    public GameObject holePrefab;
    public GameObject buttonPrefab;
    public GameObject doorPrefab;

    [Header( "MiniPrefab" )]
    public Transform gameBoard = null;
    public Transform gameView = null;
    public Transform gameContent = null;
    public string gameEnv;
    public Direction dir;
    [HideInInspector] 
    public List<MiniGameObject>[,] gameEnv2d = new List<MiniGameObject>[7,6];

    [Header( "Other" )]
    public Material matHighLight = null;
    public Material matRedHighLight = null;
    public Transform canvas = null;
    public Transform toolBar = null;
    public RectTransform outsideRect = null;
    public Transform nonReactablePanel = null;
    public WinPanel winPanel = null;
    public bool debugBack = true;

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
    private Coroutine gameCoroutine = null;
    private List<Coroutine> gameLoopCoroutines = new List<Coroutine>();
    private List<MiniGameObject> miniGameObjects = new List<MiniGameObject>();
    private bool gameBreakTrigger = false;
    private bool loopBreakTrigger = false;

    private int score_time = 0;
    private int score_amount = 0;
    private int score_blocks = 0;

    public BlockLibrary blockLibrary = null;

    public void Awake() {

#if UNITY_EDITOR
        if ( debugBack && VariablesStorage.levelJson == "" ) {
            SceneManager.LoadScene( "GameListScene" );
        }
#endif


        if ( blockLibrary == null && GameObject.FindGameObjectWithTag( "BlockLibrary" ) != null ) {
            blockLibrary = GameObject.FindGameObjectWithTag( "BlockLibrary" ).GetComponent<BlockLibrary>();
        }

        if ( blockLibrary != null ) {
            var jsonO = MiniJSON.Json.Deserialize( VariablesStorage.levelJson ) as Dictionary< string, object >;

            gameEnv = jsonO["gameEnv"] as string;
            gameEnv = gameEnv.Replace( "\n", "" );
            dir = Direction.DOWN;

            if ( jsonO.ContainsKey( "playerDir" ) ) {
                dir = (Direction)(long)jsonO["playerDir"];

                Debug.Log( (int)dir );
                Debug.Log( (int)Direction.UP );
                Debug.Log( (int)Direction.RIGHT );
                Debug.Log( (int)Direction.DOWN );
                Debug.Log( (int)Direction.LEFT );
            }

            Dictionary<int, (BlockType, int, Dictionary<string, string>)> dict = new Dictionary<int, (BlockType, int, Dictionary<string, string>)>();

            foreach ( KeyValuePair<string, object> kvp in jsonO["blocksList"] as Dictionary<string, object> ) {
                //dict[dict.Count] = ( GameUtility.StringToBlockType( kvp.Key.ToString() ), (int)kvp.Value );
                string[] keyargs = kvp.Key.ToString().Split( ':' );
                Dictionary<string, string> args = new Dictionary<string, string>();
                BlockType blockType = GameUtility.StringToBlockType( keyargs[0] );

                if      ( blockType == BlockType.MoveBlock ) {
                    if ( keyargs.Length-1 >= 1 ) {
                        args.Add( "direction", keyargs[1] );
                    }
                }
                else if ( blockType == BlockType.RepeatBlock ) {
                    if ( keyargs.Length - 1 >= 1 ) {
                        args.Add( "repeat_n", keyargs[1] );
                    }
                }

                dict[dict.Count] = ( blockType, int.Parse( kvp.Value.ToString() ), args );
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


            Dictionary<int, (BlockType, int, Dictionary<string, string>)> dict = new Dictionary<int, (BlockType, int, Dictionary<string, string>)>();

            foreach ( KeyValuePair<string, object> kvp in jsonO["blocksList"] as Dictionary<string, object> ) {
                //dict[dict.Count] = ( GameUtility.StringToBlockType( kvp.Key.ToString() ), (int)kvp.Value );
                string[] keyargs = kvp.Key.ToString().Split( ':' );
                Dictionary<string, string> args = new Dictionary<string, string>();
                BlockType blockType = GameUtility.StringToBlockType( keyargs[0] );

                if ( blockType == BlockType.MoveBlock ) {
                    if ( keyargs.Length - 1 >= 1 ) {
                        args.Add( "direction", keyargs[1] );
                    }
                }
                else if ( blockType == BlockType.RepeatBlock ) {
                    if ( keyargs.Length - 1 >= 1 ) {
                        args.Add( "repeat_n", keyargs[1] );
                    }
                }

                dict[dict.Count] = (blockType, int.Parse( kvp.Value.ToString() ), args);
            }
            blockLibrary.blockList = dict;
        }
        ResetGameView();
    }
    public void ResetGame() {

        blockLibrary.resetBlocks();

        foreach ( Transform child in gameContent ) {
            Destroy( child.gameObject );
        }
        ResetGameView();
    }

    public void ResetGameView() {

        if ( gameCoroutine != null ) {
            StopCoroutine( gameCoroutine );
            StopGame();
        }

        GameVariable.gamePiece.Clear();
        GameVariable.gamePiece.Add( 1, new List<MiniGameObject>() );
        GameVariable.gamePiece.Add( 2, new List<MiniGameObject>() );
        GameVariable.gamePiece.Add( 3, new List<MiniGameObject>() );
        GameVariable.gamePiece.Add( 4, new List<MiniGameObject>() );
        GameVariable.gamePiece.Add( 5, new List<MiniGameObject>() );
        GameVariable.gamePiece.Add( 6, new List<MiniGameObject>() );
        GameVariable.gamePiece.Add( 7, new List<MiniGameObject>() );
        player = null;
        gameEnv2d = new List<MiniGameObject>[7, 6];
        for ( int i = 0; i < 7; i++ ) {
            for ( int j = 0; j < 6; j++ ) {
                gameEnv2d[i, j] = new List<MiniGameObject>();
            }
        }
        foreach ( Transform child in gameView.transform ) {
            Destroy( child.gameObject );
        }
        Vector3 origin = gameView.position + new Vector3( 0, gameView.GetComponent<RectTransform>().sizeDelta.y, 0 );
        if ( gameEnv != null ) {

            List<List<Transform>> spawnList = new List<List<Transform>>();
            for ( int i = 0; i < 3; i++ ) {
                spawnList.Add(new List<Transform>());
            }

            for ( int i = 0; i < gameEnv.Length; i++ ) {
                Transform spawn = null;
                switch ( gameEnv[i] ) {
                    case '0':
                    case 'x':
                        break;
                    case '1':
                    case 'o':
                        spawn = Instantiate( obstaclePrefab ).transform;
                        GameVariable.gamePiece[1].Add( spawn.GetComponent<MiniGameObject>() );
                        spawnList[0].Add( spawn );
                        break;
                    case '2':
                    case 'p':
                        if ( player == null ) {
                            spawn = Instantiate( playerPrefab ).transform;
                            spawn.GetComponent<MiniGameObject>().direction = dir;
                            player = spawn;
                            GameVariable.gamePiece[2].Add( spawn.GetComponent<MiniGameObject>() );
                            spawnList[2].Add( spawn );
                        }
                        break;
                    case '3':
                    case 'b':
                        spawn = Instantiate( boxPrefab ).transform;
                        GameVariable.gamePiece[3].Add( spawn.GetComponent<MiniGameObject>() );
                        spawnList[2].Add( spawn );
                        break;
                    case '4':
                    case 'f':
                        spawn = Instantiate( flagPrefab ).transform;
                        GameVariable.gamePiece[4].Add( spawn.GetComponent<MiniGameObject>() );
                        spawnList[1].Add( spawn );
                        break;
                    case '5':
                    case 'h':
                        spawn = Instantiate( holePrefab ).transform;
                        GameVariable.gamePiece[5].Add( spawn.GetComponent<MiniGameObject>() );
                        spawnList[0].Add( spawn );
                        break;
                    case '6':
                    case 'j':
                        spawn = Instantiate( buttonPrefab ).transform;
                        GameVariable.gamePiece[6].Add( spawn.GetComponent<MiniGameObject>() );
                        spawnList[0].Add( spawn );
                        break;
                    case '7':
                    case 'd':
                        spawn = Instantiate( doorPrefab ).transform;
                        GameVariable.gamePiece[7].Add( spawn.GetComponent<MiniGameObject>() );
                        spawnList[1].Add( spawn );
                        break;
                }
                if ( spawn != null ) {
                    int x = i % 7;
                    int y = (int)Mathf.Floor( i / 7 );
                    MiniGameObject s = spawn.GetComponent<MiniGameObject>();
                    s.posInEnv = new Vector2Int( x, y );
                    s.gameManager = this;
                    gameEnv2d[x, y].Add( s );
                    spawn.position = origin + new Vector3( ( x + 0.5f ) * 50f, -( y + 0.5f ) * 50f, 0f );
                }
            }

            foreach ( List<Transform> minis in spawnList ) {
                foreach ( Transform mini in minis ) {
                    mini.SetParent( gameView );
                }
            }
        }
    }

    public void StartGame() {
        winPanel.gameObject.SetActive( false );
        if ( gameCoroutine == null ) {
            ResetGameView();
            score_time = 0;
            score_amount = 0;
            score_blocks = 0;
            List<Tuple<string, Transform, List<object>>> commands = CreateCommand();
            gameVariableLists.Clear();
            Debug.Log( "before enter" );
            nonReactablePanel.gameObject.SetActive( true );
            gameCoroutine = StartCoroutine( ExecuteCommand( commands, false ) );
        }
        else {
            StopGame();
        }
    }

    public void StopGame( bool win = false ) {

        foreach ( Image img in gameContent.GetComponentsInChildren<Image>() ) {
            img.material = null;
        }

        nonReactablePanel.gameObject.SetActive( false );
        if ( win ) {
            winPanel.gameObject.SetActive( true );
            winPanel.time.text = score_time.ToString() + ":" + VariablesStorage.levelTime.ToString();
            winPanel.amount.text = score_amount.ToString() + ":" + VariablesStorage.levelAmount.ToString();
            winPanel.blocks.text = score_blocks.ToString() + ":" + VariablesStorage.levelBlocks.ToString();
            if ( VariablesStorage.levelTime > score_time || VariablesStorage.levelAmount > score_amount || VariablesStorage.levelBlocks < score_blocks ) {
                winPanel.newScoreText.gameObject.SetActive( true );
                winPanel.upload.interactable = true;
            }
        }

        foreach ( Coroutine c in gameLoopCoroutines ) {
            StopCoroutine( c );
        }

        gameCoroutine = null;
        gameStarted = false;
    }

    private int findVariableInLists( string variableName, int layer ) {
        for ( int i = layer; i >= 0; i-- ) {
            if ( gameVariableLists[i].ContainsKey( variableName ) ) {
                return i;
            }
        }
        return -1;
    }

    public IEnumerator ExecuteCommand( List<Tuple<string, Transform, List<object>>> commands, bool child = true ) {
        Debug.Log( "enter" );
        gameStarted = true;
        gameVariableLists.Add( new Dictionary<string, object>() );
        int local = gameVariableLists.Count - 1;
        loopBreakTrigger = false;
        gameBreakTrigger = false;
        score_time = 0;
        if ( !child ) gameLoopCoroutines.Clear();

        WaitForSeconds wait = new WaitForSeconds( 0.75f );
        foreach ( Tuple<string, Transform, List<object>> command in commands ) {
            if ( command.Item3.Count >= 2 && command.Item3[0] == null && command.Item3[1].GetType() == typeof( string ) ) {
                foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                    img.material = matRedHighLight;
                }
                Debug.Log( command.Item3[1].ToString() );
                gameBreakTrigger = true;
            }
            if ( !gameBreakTrigger ) {
                foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                    img.material = matHighLight;
                }

                string variableName = "";
                string value = "";
                int layer;
                double num;

                switch ( command.Item1 ) {
                    case "start":
                        // do nothing
                        break;
                    case "define":
                        variableName = command.Item3[0].ToString();
                        value = command.Item3[1].ToString();

                        if ( value.StartsWith( "&" ) ) {
                            value = value.Substring( 1 );
                            if ( ( layer = findVariableInLists( value, local ) ) != -1 ) {
                                value = gameVariableLists[layer][value].ToString();
                            }
                            else {
                                foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                                    img.material = matRedHighLight;
                                }
                                Debug.LogWarning( "variable \"" + value + "\" doesn't exsit!" );
                                gameBreakTrigger = true;
                                break;
                            }
                        }

                        if ( !gameVariableLists[local].ContainsKey( variableName ) ) {
                            if ( double.TryParse( value, out num ) ) {
                                gameVariableLists[local].Add( variableName, num );
                            }
                            else {
                                gameVariableLists[local].Add( variableName, value );
                            }
                        }
                        else {
                            if ( double.TryParse( value, out num ) ) {
                                gameVariableLists[local][variableName] = num;
                            }
                            else {
                                gameVariableLists[local][variableName] = value;
                            }
                        }
                        break;
                    case "set":

                        variableName = command.Item3[0].ToString();

                        value = command.Item3[1].ToString();

                        if ( value.StartsWith( "&" ) ) {
                            value = value.Substring( 1 );
                            if ( ( layer = findVariableInLists( value, local ) ) != -1 ) {
                                value = gameVariableLists[layer][value].ToString();
                            }
                            else {
                                foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                                    img.material = matRedHighLight;
                                }
                                Debug.LogWarning( "variable \"" + value + "\" doesn't exsit!" );
                                gameBreakTrigger = true;
                                break;
                            }
                        }

                        if (( layer = findVariableInLists( variableName, local ) ) != -1 ) {
                            if ( double.TryParse( value, out num ) ) {
                                gameVariableLists[layer][variableName] = num;
                            }
                            else {
                                gameVariableLists[layer][variableName] = value;
                            }
                            Debug.Log( "variable \"" + variableName + "\" exsit!" );
                        }
                        else {
                            foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                                img.material = matRedHighLight;
                            }
                            Debug.LogWarning( "variable \"" + variableName + "\" doesn't exsit!" );
                            gameBreakTrigger = true;
                            break;
                        }

                        break;
                    case "for":
                        yield return StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[0] ) );
                        break;
                    case "if":

                        foreach ( Image img in ((Transform)command.Item3[5]).GetComponentsInChildren<Image>() ) {
                            img.material = null;
                        }
                        foreach ( Image img in ( (Transform)command.Item3[6] ).GetComponentsInChildren<Image>() ) {
                            img.material = null;
                        }

                        value = command.Item3[0].ToString();

                        if ( value.StartsWith( "&" ) ) {
                            value = value.Substring( 1 );
                            if ( ( layer = findVariableInLists( value, local ) ) != -1 ) {
                                value = gameVariableLists[layer][value].ToString();
                            }
                            else {
                                foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                                    img.material = matRedHighLight;
                                }
                                Debug.LogWarning( "variable \"" + value + "\" doesn't exsit!" );
                                gameBreakTrigger = true;
                                break;
                            }
                        }
                        string value2 = command.Item3[1].ToString();

                        if ( value2.StartsWith( "&" ) ) {
                            value2 = value2.Substring( 1 );
                            if ( ( layer = findVariableInLists( value2, local ) ) != -1 ) {
                                value2 = gameVariableLists[layer][value2].ToString();
                            }
                            else {
                                foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                                    img.material = matRedHighLight;
                                }
                                Debug.LogWarning( "variable \"" + value2 + "\" doesn't exsit!" );
                                gameBreakTrigger = true;
                                break;
                            }
                        }
                        Debug.Log( value + value2 );
                        bool enter = false;

                        if ( command.Item3[2].ToString() == "==" ) {
                            enter = value == value2;
                        }
                        else if ( command.Item3[2].ToString() == "!=" ) {
                            enter = value != value2;
                        }
                        else {

                            if ( double.TryParse( value, out num ) && double.TryParse( value2, out double num2 ) ) {
                                if ( command.Item3[2].ToString() == ">" ) {
                                    enter = num > num2;
                                }
                                else if ( command.Item3[2].ToString() == "<" ) {
                                    enter = num < num2;
                                }
                                else if ( command.Item3[2].ToString() == ">=" ) {
                                    enter = num >= num2;
                                }
                                else if ( command.Item3[2].ToString() == "<=" ) {
                                    enter = num <= num2;
                                }
                            }
                            else if ( GameUtility.OptionTextToNumber( value, out int num3 )  && GameUtility.OptionTextToNumber( value2, out int num4 ) ) {
                                Debug.Log( num3 + num4 );
                                foreach ( MiniGameObject mini in GameVariable.gamePiece[num3] ) {
                                    if      ( command.Item3[2].ToString() == "面向" )     enter = mini.IsFacing( num4 );
                                    else if ( command.Item3[2].ToString() == "沒有面向" ) enter = !mini.IsFacing( num4 );
                                    else if ( command.Item3[2].ToString() == "腳下有" )   enter = mini.IsOn( num4 );
                                    else if ( command.Item3[2].ToString() == "腳下沒有" ) enter = !mini.IsOn( num4 );
                                }
                            }
                        }

                        if ( enter ) {
                            yield return StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[3] ) );
                        }
                        else {
                            yield return StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[4] ) );
                        }

                        //Transform t = (Transform)command.Item3[2];
                        //StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[0] ) );
                        //StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[1] ) );
                        break;
                    case "repeat":

                        foreach ( Image img in ( (Transform)command.Item3[2] ).GetComponentsInChildren<Image>() ) {
                            img.material = null;
                        }

                        int time = 0;
                        if ( command.Item3[0].GetType() == typeof( int ) ) {
                            time = (int)command.Item3[0];
                        }
                        else {
                            variableName = command.Item3[0].ToString();
                            if ( variableName.StartsWith( "&" ) ) {
                                variableName = variableName.Substring( 1 );

                                Debug.Log( variableName );

                                if ( ( layer = findVariableInLists( variableName, local ) ) != -1 ) {
                                    time = (int)(double)gameVariableLists[layer][variableName];
                                    Debug.Log( time );
                                }
                                else {
                                    Debug.LogWarning( "variable \"" + variableName + "\" doesn't exsit!" );
                                }
                            }
                        }
                        int x = 0;
                        if ( variableName == "infinity" ) {
                            while ( true ) {
                                score_blocks += -100;
                                gameLoopCoroutines.Add( StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[1] ) ) );
                                x = gameLoopCoroutines.Count - 1;
                                yield return gameLoopCoroutines[x];
                                if ( loopBreakTrigger || gameBreakTrigger ) {
                                    loopBreakTrigger = false;
                                    break;
                                }
                            }
                        }
                        else {
                            for ( int i = 0; i < time; i++ ) {
                                score_blocks += -100;
                                gameLoopCoroutines.Add( StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[1] ) ) );
                                x = gameLoopCoroutines.Count - 1;
                                yield return gameLoopCoroutines[x];
                                if ( loopBreakTrigger || gameBreakTrigger ) {
                                    loopBreakTrigger = false;
                                    break;
                                }
                            }
                        }

                        //StartCoroutine( ExecuteCommand( (List<Tuple<string, Transform, List<object>>>)command.Item3[0] ) );
                        break;
                    case "move":
                        score_blocks += -50;
                        if ( double.TryParse( command.Item3[0].ToString(), out num ) ) {
                            MiniGameObject playerMini = player.GetComponent<MiniGameObject>();
                            playerMini.Move( (int)num );
                        }
                        break;
                    case "turn":
                        score_blocks += -50;
                        if ( double.TryParse( command.Item3[0].ToString(), out num ) ) {
                            MiniGameObject playerMini = player.GetComponent<MiniGameObject>();
                            playerMini.Turn( (int)num );
                        }
                        break;
                    case "break":
                        loopBreakTrigger = true;
                        break;
                }
            }

            score_time += 1;
            yield return wait;

            foreach ( List< MiniGameObject > list in gameEnv2d ) {
                foreach ( MiniGameObject mgo in list ) {
                    //if ( mgo.containObject( mgo.posInEnv.x, mgo.posInEnv.y, 5 ).Count >= 0 ) {
                    //    mgo.fallToDead = true;
                    //    mgo.moveAnimationType = 2;
                    //    mgo.moveAnimationStart = Time.time;
                    //}
                    if ( mgo.objectType == 2 || mgo.objectType == 3 ) {
                        if ( mgo.containObject( mgo.posInEnv.x, mgo.posInEnv.y, 5 ).Count > 0 ) {
                            mgo.fallToDead = true;
                            mgo.moveAnimationType = 2;
                            mgo.moveAnimationStart = Time.time;
                            if ( mgo.objectType == 2 ) {
                                gameBreakTrigger = true;
                            }
                        }
                    }
                }
            }

            foreach ( Image img in command.Item2.GetComponentsInChildren<Image>() ) {
                img.material = null;
            }

            if ( gameBreakTrigger ) {
                StopGame();
                yield break;
            }
        }



        if ( !child ) {
            StopGame( player.GetComponent<MiniGameObject>().IsOnFlag() );
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
                        score_amount += 1;
                        score_blocks += 1000;
                        break;

                    case BlockType.DefineBlock:
                        type = "define";
                        score_amount += 1;
                        score_blocks += 200;
                        if ( blockInfo.refField[0].GetComponent<TMP_InputField>().text == "" ) {
                            infos.Add( null );
                            infos.Add( "You need a name to define variable!" );
                        }
                        else {
                            infos.Add( blockInfo.refField[0].GetComponent<TMP_InputField>().text );
                            if ( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount > 0 ) {
                                score_amount += 1;
                                score_blocks += 50;
                                infos.Add( "&" + blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                            }
                            else {
                                score_blocks += 100;
                                infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text );
                            }
                        }
                        break;

                    case BlockType.SetBlock:
                        type = "set";
                        score_amount += 2;
                        score_blocks += 300;
                        if ( blockInfo.refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount == 0 ) {
                            infos.Add( null );
                            infos.Add( "You need to have a variable to set to!" );
                        }
                        else {
                            infos.Add( blockInfo.refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                            if ( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount > 0 ) {
                                score_amount += 1;
                                score_blocks += 300;
                                infos.Add( "&" + blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                            }
                            else {
                                score_blocks += 50;
                                infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text );
                            }
                        }
                        break;

                    case BlockType.ForBlock:
                        type = "for";
                        score_amount += 1;
                        score_blocks += 800;
                        infos.Add( CreateCommand( blockInfo.refField[0] ) );
                        break;

                    case BlockType.IfBlock:
                        type = "if";
                        score_amount += 2;
                        score_blocks += 800;
                        if ( blockInfo.refField[2].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount == 0 ) {
                            infos.Add( null );
                            infos.Add( "You need to have a login!" );
                            break;
                        }
                        int k = 3;

                        if ( blockInfo.refField[2].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<ValueBlockSwap>() == null ) {
                            k = 1;
                        }
                        else {
                            for ( int j = 1; j < 3; j++ ) {
                                Transform vc = blockInfo.refField[2].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( j );
                                if ( vc.GetComponent<ValueBlockSwap>().valueBlockGrid.childCount > 0 ) {
                                    score_amount += 1;
                                    score_blocks += 300;
                                    infos.Add( "&" + vc.GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                                    //Debug.Log( vc.GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                                }
                                else {
                                    score_amount += 1;
                                    score_blocks += 50;
                                    infos.Add( vc.GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text );
                                    //Debug.Log( vc.GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text );
                                }
                            }
                        }

                        for (int j = k; j <= 3; j++ ) {
                            TMP_Dropdown dp = blockInfo.refField[2].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( j ).GetComponent<TMP_Dropdown>();
                            infos.Add( dp.options[dp.value].text );
                            //Debug.Log( dp.options[dp.value].text );
                        }


                        infos.Add( CreateCommand( blockInfo.refField[0] ) );
                        infos.Add( CreateCommand( blockInfo.refField[1] ) );

                        infos.Add( blockInfo.refField[0] );
                        infos.Add( blockInfo.refField[1] );

                        break;

                    case BlockType.MoveBlock:
                        type = "move";
                        score_amount += 1;
                        score_blocks += -50;
                        infos.Add( 1 );
                        break;


                    case BlockType.TurnBlock:
                        type = "turn";
                        score_amount += 1;
                        score_blocks += -50;
                        infos.Add( blockInfo.refField[0].GetComponent<TMP_Dropdown>().value );
                        break;

                    case BlockType.RepeatBlock:
                        type = "repeat";
                        score_amount += 1;
                        score_blocks += 800;
                        //infos.Add( CreateCommand( blockInfo.refField[0] ) );

                        if ( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount > 0 ) {
                            score_blocks += 100;
                            infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                        }
                        else {
                            score_amount += 1;
                            score_blocks += 200;
                            int num = 0;
                            if ( int.TryParse( blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text, out num ) ) {
                                infos.Add( num );
                            }
                            else if ( blockInfo.refField[1].GetComponent<ValueBlockSwap>().inputField.GetComponent<TMP_InputField>().text == "infinity" ) {
                                infos.Add( "infinity" );
                            }
                            else {
                                infos.Add( null );
                                infos.Add( "Repeat Block need a number in order to function!" );
                            }
                        }
                        infos.Add( CreateCommand( blockInfo.refField[0] ) );
                        infos.Add( blockInfo.refField[0] );

                        //if ( blockInfo.refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid.childCount == 0 ) {
                        //    infos.Add( null );
                        //    infos.Add( "You need to have a variable to set to!" );
                        //}
                        //else {
                        //    infos.Add( blockInfo.refField[0].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );

                        //}
                        //infos.Add( blockInfo.refField[1].GetComponent<ValueBlockSwap>().valueBlockGrid.GetChild( 0 ).GetChild( 1 ).GetComponent<TMP_Text>().text );
                        break;
                    case BlockType.BreakBlock:
                        type = "break";
                        score_amount += 1;
                        break;
                }

                commands.Add( new Tuple<string, Transform, List<object>>( type, target.GetChild( i ), infos ) );
            }
        }

        return commands;
    }
    public void StartUpload() {
        StartCoroutine( Upload() );
    }

    public IEnumerator Upload() {
        string stmt = "UPDATE play_record SET " +
            "score_time = " +   ( ( VariablesStorage.levelTime == -1 || VariablesStorage.levelTime > score_time ) ? score_time : VariablesStorage.levelTime ).ToString() + "," +
            "score_amount = " + ( ( VariablesStorage.levelAmount == -1 || VariablesStorage.levelAmount > score_amount ) ? score_amount : VariablesStorage.levelAmount ).ToString() + "," +
            "score_blocks = " + ( ( VariablesStorage.levelBlocks == -1 || VariablesStorage.levelBlocks < score_blocks ) ? score_blocks : VariablesStorage.levelBlocks ).ToString() + " " +
            "WHERE member_id = '" + VariablesStorage.memberId + "' AND course_id = '" + VariablesStorage.courseId + "';";

        if ( VariablesStorage.levelTime > score_time || VariablesStorage.levelAmount > score_amount || VariablesStorage.levelBlocks < score_blocks ) {
            yield return StartCoroutine( UploadScore( stmt ) );
            winPanel.back.interactable = true;
        }
        
    }

    private IEnumerator UploadScore( string stmt_u ) {

        string stmt = "";
        string jsonString = null;

        Debug.Log( "upload score" );
        stmt = "SELECT * FROM play_record WHERE member_id = '" + VariablesStorage.memberId + "' AND course_id = '" + VariablesStorage.courseId + "';";
        Debug.Log( stmt );

        yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
            jsonString = returnValue;
        } ) );

        if ( jsonString == null ) {
            Debug.Log( "sql Error" );
            //infoText.text = "SQL Error : Please contact us the error!";
            //infoText.color = new Color( 1, 0, 0 );
            yield break;
        }
        else if ( jsonString.Trim() == "[]" || jsonString.Trim() == "" ) {
            Debug.Log( "no record" );


            stmt = "INSERT INTO play_record VALUES ('" + VariablesStorage.memberId + "','" + VariablesStorage.courseId + "', -1, -1, -1 );";

            yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
                jsonString = returnValue;
            } ) );

            Debug.Log( stmt );
            Debug.Log( jsonString );

        }

        Debug.Log( stmt_u );
        yield return StartCoroutine( NetworkManager.GetRequest( stmt_u, returnValue => {
            Debug.Log( returnValue );
        } ) );


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
            if ( bg != null && bg.parent.name.StartsWith( "Content" ) ) {
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
            case BlockType.RepeatBlock:
                blockPrefab = repeatBlockPrefab;
                break;
            case BlockType.TurnBlock:
                blockPrefab = turnBlockPrefab;
                break;
            case BlockType.BreakBlock:
                blockPrefab = breakBlockPrefab;
                break;
            case BlockType.LogicBlock:
                blockPrefab = logicBlockPrefab;
                break;
            case BlockType.LogicSelectorBlock:
                blockPrefab = logicSelectorBlockPrefab;
                break;
        }
        return blockPrefab;
    }

    public void GoBack() {
        SceneManager.LoadScene( "GameListScene" );
    }
}
