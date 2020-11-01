using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MiniJSON;
using UnityEngine.UI;
using TMPro;

public class LevelInfoPanelManager : MonoBehaviour {

    Stack levelList = new Stack();
    public LevelInfoPanel gameInfoPanelPrefab = null;
    public Transform parentCanvas = null;
    public Transform levelButtonHolder = null;
    public GameObject buttonPrefab = null;
    public Transform loadingIcon = null;

    public Dictionary<int, ( string, string, string, string, int, int, int)> levelsInfo = new Dictionary<int, ( string, string, string, string, int, int, int)>();

    void Start() {
        //levelsInfo.Add( 1, ("First Level", "Lorem ipsum dolor sit amet, orci erat morbi interdum erat, nibh wisi erat. Sed nulla urna, at vel, vitae aliquam imperdiet placerat scelerisque.", "Creator1", "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":4, \"DefineBlock\":2, \"MoveBlock\":0}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}") );
        //levelsInfo.Add( 2, ("Second Level", "Quis nullam massa eleifend egestas donec massa, velit dui accumsan, augue vivamus.", "Creator2", "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":1}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}") );

        if ( VariablesStorage.roomOK ) {
            ReloadLevels();
        }
    }

    public void ReloadLevels() {
        StartCoroutine( ReloadLevelsCorotine() );
    }

    IEnumerator ReloadLevelsCorotine() {

        print( "start" );
        loadingIcon.gameObject.SetActive( true );
        AddOrRemovePanel(-1);

        levelsInfo.Clear();

        for ( int i = levelButtonHolder.childCount - 1; i >= 0; i-- ) {
            Destroy( levelButtonHolder.GetChild( i ).gameObject );
        }

        string jsonString = null;
        string stmt = "";
        //string cjson = "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":4, \"DefineBlock\":2, \"MoveBlock\":0}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}";
        //stmt = "INSERT INTO course ( course_name, type_id, player_id, course_json ) VALUES ( 'test', 'T1000000', 'P1000000', '" + cjson + "' )";
        //stmt = "DELETE FROM course WHERE course_name = 'test'";
        // {"course_id":"C1000003","course_name":"test","type_id":"T1000000","hint":null,"player_id":"P1000000","course_json":null}
        stmt = "SELECT * FROM course;";
        //stmt = "INSERT INTO course_type ( type_name ) VALUES ( 'type_test' )";

        yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
            jsonString = returnValue;
        }));
#if UNITY_EDITOR
        if ( jsonString == null || jsonString == "" ) {
            jsonString = "[{\"course_name\" : \"Course Test1\",\"player_name\" : \"PlayerAAA\",\"course_json\" : \"{\\\"blocksList\\\":{\\\"StartBlock\\\":1,\\\"SetBlock\\\":0,\\\"DefineBlock\\\":0,\\\"MoveBlock\\\":0,\\\"ForBlock\\\":0,\\\"IfBlock\\\":0,\\\"RepeatBlock\\\":0},\\\"gameEnv\\\":\\\"001001010010001000100013120000400001000100\\\"}\"},{\"course_name\" : \"Course Test2\",\"player_name\" : \"PlayerAAA\",\"course_json\" : \"{\\\"blocksList\\\":{\\\"StartBlock\\\":2,\\\"SetBlock\\\":2,\\\"DefineBlock\\\":1,\\\"MoveBlock\\\":0},\\\"gameEnv\\\":\\\"001001010010001000100013120000000001000100\\\"}\"},{\"course_name\" : \"Course Test3\",\"player_name\" : \"PlayerBBB\",\"course_json\" : \"{\\\"blocksList\\\":{\\\"StartBlock\\\":3,\\\"SetBlock\\\":4,\\\"DefineBlock\\\":3,\\\"MoveBlock\\\":0},\\\"gameEnv\\\":\\\"001001010010001000100013120000000001000100\\\"}\"}]";
        }
#endif

        if ( jsonString == null ) {
            Debug.Log( "sql Error" );
        }
        else if ( jsonString.Trim() == "[]" || jsonString.Trim() == "" ) {
            Debug.Log( "table empty" );
        }
        else {
            Debug.Log( jsonString );

            var jsonO = MiniJSON.Json.Deserialize( jsonString ) as List<object>;
            int i = 1;
            foreach ( Dictionary<string, object> item in jsonO ) {
                Dictionary<string, object> it = item as Dictionary<string, object>;
                string course_id = it["course_id"] as string;
                string course_name = it["course_name"] as string;
                string description = it["description"] as string;
                string course_json = it["course_json"] as string;
                course_json = course_json.Replace( "\n", "" ).Replace( "\t", "" );
                int score_time = -1;
                int score_amount = -1;
                int score_blocks = -1;
                //Debug.LogWarning( "SELECT * FROM play_record WHERE member_id = '" + VariablesStorage.memberId + "' AND course_id = '" + course_id + "';" );
                yield return StartCoroutine( NetworkManager.GetRequest( "SELECT * FROM play_record WHERE member_id = '" + VariablesStorage.memberId + "' AND course_id = '" + course_id + "';", returnValue => {
                    if (!( returnValue.Trim() == "[]" || returnValue.Trim() == "" )) {
                        Debug.LogWarning( "returnValue:" + returnValue );
                        jsonO = MiniJSON.Json.Deserialize( returnValue ) as List<object>;

                        it = jsonO[0] as Dictionary<string, object>;

                        foreach ( var k in it.Keys ) {
                            Debug.Log( k );
                        }
                        score_time = (int)(long)it["score_time"];
                        score_amount = (int)(long)it["score_amount"];
                        score_blocks = (int)(long)it["score_blocks"];
                    }
                }));

                levelsInfo.Add( i, ( course_id, course_name, description, course_json, score_time, score_amount, score_blocks ) );
                Transform btn = Instantiate( buttonPrefab, levelButtonHolder ).transform;
                int temp = i++;
                btn.GetComponent<Button>().onClick.AddListener( delegate { AddOrRemovePanel( temp ); } );
                btn.GetChild( 0 ).GetComponent<TMP_Text>().text = course_name;
            }
        }

        Transform btnReload = Instantiate( buttonPrefab, levelButtonHolder ).transform;
        btnReload.GetComponent<Button>().onClick.AddListener( () => ReloadLevels() );
        btnReload.GetChild( 0 ).GetComponent<TMP_Text>().text = "Reload";

        loadingIcon.gameObject.SetActive( false );
        print( "end" );
    }

    public void AddOrRemovePanel( int id ) {

        if ( levelList.Count > 0 ) {
            LevelInfoPanel panel = (LevelInfoPanel)levelList.Peek();

            Debug.Log( panel.levelId );
            Debug.Log( id );

            if ( panel.levelId == id ) return;

            levelList.Pop();

            panel.PullTrigger( false );
        }

        if ( id >= 0 ) {
            LevelInfoPanel gip = Instantiate( gameInfoPanelPrefab, parentCanvas ).transform.GetComponent<LevelInfoPanel>();
            levelList.Push( gip );
            gip.levelId = id;
            gip.SetInfo( levelsInfo[id] );
            gip.levelInfoPanelManager = this;
            gip.PullTrigger( true );
        }
    }
}
