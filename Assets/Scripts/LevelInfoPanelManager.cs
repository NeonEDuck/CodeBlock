﻿using UnityEngine;
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
    public List<string> previewImgKey = new List<string>();
    public List<Sprite> previewImgValue = new List<Sprite>();
    public LeaderBoard leaderBoard = null;
    public Transform topicPanel = null;
    public Transform blockPanel = null;
    public Transform topicLabel = null;

    public Dictionary<int, ( string, string, string, string, int, int, int, string)> levelsInfo = new Dictionary<int, ( string, string, string, string, int, int, int, string)>();
    public List<( string, Transform )> levelsBtns = new List<(string, Transform)>();
    public Dictionary<string, (string, Color, Color)> topics = new Dictionary<string, (string, Color, Color)>();

    void Start() {
        //levelsInfo.Add( 1, ("First Level", "Lorem ipsum dolor sit amet, orci erat morbi interdum erat, nibh wisi erat. Sed nulla urna, at vel, vitae aliquam imperdiet placerat scelerisque.", "Creator1", "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":4, \"DefineBlock\":2, \"MoveBlock\":0}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}") );
        //levelsInfo.Add( 2, ("Second Level", "Quis nullam massa eleifend egestas donec massa, velit dui accumsan, augue vivamus.", "Creator2", "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":1}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}") );

        if ( VariablesStorage.roomOK ) {
            ReloadLevels();
        }
    }
    public void FetchLevelLeaderBoard( int levelId ) {
        leaderBoard.OpenLeaderBoard( true, levelsInfo[levelId].Item1, levelsInfo[levelId].Item2 );
    }

    public void ReloadLevels() {
        StartCoroutine( ReloadLevelsCorotine() );
    }

    IEnumerator ReloadLevelsCorotine() {

        print( "start" );
        loadingIcon.gameObject.SetActive( true );
        blockPanel.gameObject.SetActive( true );
        AddOrRemovePanel(-1);

        levelsInfo.Clear();

        for ( int i = levelButtonHolder.childCount - 1; i >= 0; i-- ) {
            Destroy( levelButtonHolder.GetChild( i ).gameObject );
        }

        yield return StartCoroutine( NetworkManager.GetRequest( "SELECT * FROM topic ORDER BY topic_id ASC", returnValue => {
            var jsonO = MiniJSON.Json.Deserialize( returnValue ) as List<object>;
            int i = 0;
            foreach ( Dictionary<string, object> item in jsonO ) {
                Color HColor = topicPanel.GetComponent<TopicPanel>().buttons[i].GetChild( 2 ).GetComponent<Image>().color;
                Color BColor = topicPanel.GetComponent<TopicPanel>().buttons[i].GetChild( 3 ).GetComponent<Image>().color;

                topics.Add( item["topic_id"] as string, ( item["topic_name"] as string, HColor, BColor ) );
                Debug.Log( item["topic_id"] as string + ", " + item["topic_name"] as string );
                if ( i < topicPanel.GetComponent<TopicPanel>().buttons.Count ) {
                    topicPanel.GetComponent<TopicPanel>().buttons[i].GetComponent<TopicButton>().text.text = item["topic_name"] as string;
                    topicPanel.GetComponent<TopicPanel>().buttons[i].GetComponent<Button>().onClick.AddListener( () => { ChangeType( item["topic_id"] as string ); } );
                    i++;
                }
            }
        } ) );

        string jsonString = null;
        string stmt = "";
        //string cjson = "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":4, \"DefineBlock\":2, \"MoveBlock\":0}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}";
        //stmt = "INSERT INTO course ( course_name, type_id, player_id, course_json ) VALUES ( 'test', 'T1000000', 'P1000000', '" + cjson + "' )";
        //stmt = "DELETE FROM course WHERE course_name = 'test'";
        // {"course_id":"C1000003","course_name":"test","type_id":"T1000000","hint":null,"player_id":"P1000000","course_json":null}
        stmt = $"SELECT course.*, play_record.score_time, play_record.score_amount, play_record.score_blocks FROM course left join (	select course_id, play_record.score_time, play_record.score_amount, play_record.score_blocks FROM play_record where member_id = '{VariablesStorage.memberId}') as play_record on course.course_id = play_record.course_id order by course.course_id";
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

            //List<object> jsonO2 = new List<object>();

            //yield return StartCoroutine( NetworkManager.GetRequest( "SELECT * FROM play_record WHERE member_id = '" + VariablesStorage.memberId + "';", returnValue => {
            //    if ( !( returnValue.Trim() == "[]" || returnValue.Trim() == "" ) ) {
            //        Debug.LogWarning( "returnValue:" + returnValue );
            //        jsonO2 = MiniJSON.Json.Deserialize( returnValue ) as List<object>;
            //    }
            //} ) );



            //it = jsonO[0] as Dictionary<string, object>;

            //foreach ( var k in it.Keys ) {
            //    Debug.Log( k );
            //}
            //score_time = (int)(long)it["score_time"];
            //score_amount = (int)(long)it["score_amount"];
            //score_blocks = (int)(long)it["score_blocks"];

            var jsonO = MiniJSON.Json.Deserialize( jsonString ) as List<object>;
            int i = 1;
            foreach ( Dictionary<string, object> item in jsonO ) {
                string course_id = item["course_id"] as string;
                string course_name = item["course_name"] as string;
                string description = item["description"] as string;
                string course_json = item["course_json"] as string;
                course_json = course_json.Replace( "\n", "" ).Replace( "\t", "" );
                int score_time = ( item["score_time"] is null ) ? -1 : (int)(long)item["score_time"]; ;
                int score_amount = ( item["score_amount"] is null ) ? -1 : (int)(long)item["score_amount"];
                int score_blocks = ( item["score_blocks"] is null ) ? -1 : (int)(long)item["score_blocks"];
                string topic_id = item["topic_id"] as string;

                //Debug.LogWarning( "SELECT * FROM play_record WHERE member_id = '" + VariablesStorage.memberId + "' AND course_id = '" + course_id + "';" );

                //foreach ( Dictionary<string, object> item2 in jsonO2 ) {
                //    if ( item["course_id"] as string == course_id ) {
                //        score_time = (int)(long)item2["score_time"];
                //        score_amount = (int)(long)item2["score_amount"];
                //        score_blocks = (int)(long)item2["score_blocks"];
                //    }
                //}

                levelsInfo.Add( i, ( course_id, course_name, description, course_json, score_time, score_amount, score_blocks, topic_id) );
                Transform btn = Instantiate( buttonPrefab, levelButtonHolder ).transform;
                btn.SetParent( transform );
                int temp = i++;
                btn.GetComponent<Button>().onClick.AddListener( () => { AddOrRemovePanel( temp ); } );
                btn.GetChild( 0 ).GetComponent<TMP_Text>().text = course_name;
                levelsBtns.Add( ( topic_id, btn ) );
            }
        }

        ChangeType( VariablesStorage.levelTopic );
        loadingIcon.gameObject.SetActive( false );
        blockPanel.gameObject.SetActive( false );
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
            gip.levelInfoPanelManager = this;
            gip.SetInfo( levelsInfo[id] );
            gip.PullTrigger( true );
        }
    }

    public void ChangeType(string type) {

        if ( type == "" ) {
            topicPanel.gameObject.SetActive( true );
            AddOrRemovePanel( -1 );
        }
        else {
            topicLabel.GetComponentInChildren<TMP_Text>().text = topics[type].Item1;
            topicLabel.GetChild( 2 ).GetComponent<Image>().color = topics[type].Item2;
            topicLabel.GetChild( 3 ).GetComponent<Image>().color = topics[type].Item3;
            topicPanel.gameObject.SetActive( false );
        }

        foreach ( (string, Transform) d in levelsBtns ) {
            if ( d.Item1 == type ) {
                d.Item2.SetParent( levelButtonHolder );
                d.Item2.localScale = Vector3.one;
            }
            else {
                d.Item2.SetParent( transform );
            }
        }
    }
}
