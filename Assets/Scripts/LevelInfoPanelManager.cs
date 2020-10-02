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
    public NetworkManager networkManager = null;
    public Transform levelButtonHolder = null;
    public GameObject buttonPrefab = null;
    public Transform loadingIcon = null;

    public Dictionary<int, (string, string, string, string)> levelsInfo = new Dictionary<int, (string, string, string, string)>();

    void Start() {
        //levelsInfo.Add( 1, ("First Level", "Lorem ipsum dolor sit amet, orci erat morbi interdum erat, nibh wisi erat. Sed nulla urna, at vel, vitae aliquam imperdiet placerat scelerisque.", "Creator1", "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":4, \"DefineBlock\":2, \"MoveBlock\":0}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}") );
        //levelsInfo.Add( 2, ("Second Level", "Quis nullam massa eleifend egestas donec massa, velit dui accumsan, augue vivamus.", "Creator2", "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":1}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}") );

        ReloadLevels();
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
        stmt = "SELECT * FROM course LEFT JOIN player ON course.player_id = player.player_id";
        //stmt = "INSERT INTO course_type ( type_name ) VALUES ( 'type_test' )";

        yield return StartCoroutine( networkManager.GetRequest( stmt, returnValue => {
            jsonString = returnValue;
        }));
#if UNITY_EDITOR
        jsonString = "[{\"course_name\" : \"Course Test1\",\"player_name\" : \"PlayerAAA\",\"course_json\" : \"{\\\"blocksList\\\":{\\\"StartBlock\\\":1,\\\"SetBlock\\\":0,\\\"DefineBlock\\\":0,\\\"MoveBlock\\\":0,\\\"ForBlock\\\":0},\\\"gameEnv\\\":\\\"001001010010001000100013120000400001000100\\\"}\"},{\"course_name\" : \"Course Test2\",\"player_name\" : \"PlayerAAA\",\"course_json\" : \"{\\\"blocksList\\\":{\\\"StartBlock\\\":2,\\\"SetBlock\\\":2,\\\"DefineBlock\\\":1,\\\"MoveBlock\\\":0},\\\"gameEnv\\\":\\\"001001010010001000100013120000000001000100\\\"}\"},{\"course_name\" : \"Course Test3\",\"player_name\" : \"PlayerBBB\",\"course_json\" : \"{\\\"blocksList\\\":{\\\"StartBlock\\\":3,\\\"SetBlock\\\":4,\\\"DefineBlock\\\":3,\\\"MoveBlock\\\":0},\\\"gameEnv\\\":\\\"001001010010001000100013120000000001000100\\\"}\"}]";
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
                string course_name = it["course_name"] as string;
                string player_name = it["player_name"] as string;
                string course_json = it["course_json"] as string;
                levelsInfo.Add( i, (course_name, "Lorem ipsum dolor sit amet, orci erat morbi interdum erat, nibh wisi erat. Sed nulla urna, at vel, vitae aliquam imperdiet placerat scelerisque.", player_name, course_json) );
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
