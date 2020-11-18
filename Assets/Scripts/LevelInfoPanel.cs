﻿using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelInfoPanel : MonoBehaviour {

    public int levelId = 0;
    public LevelInfoPanelManager levelInfoPanelManager = null;

    public TMP_Text TMP_Name = null;
    public TMP_Text TMP_Desc = null;
    public TMP_Text TMP_Creator = null;
    public Transform T_img = null;

    public string courseId = "";
    public string levelName = "";
    public string levelDesc = "";
    public string levelCreator = "";
    public string levelJson = "";
    public int levelScoreTime = 0;
    public int levelScoreAmount = 0;
    public int levelScoreBlocks = 0;
    public Sprite levelPreviewImg = null;

    public bool pullOutState = false;
    private float pullAnimationStart = -1f;
    private float pullAnimationTime = 0.5f;
    private float delta = 0f;

    private Vector3 fromPos = Vector3.zero;
    private Vector3 toPos = Vector3.zero;

    void Update() {
        if ( pullAnimationStart != -1f && delta != 1f ) {
            delta = Mathf.Min( 1f, ( Time.time - pullAnimationStart ) / pullAnimationTime );
            if ( !pullOutState && delta == 1f ) {
                Destroy( gameObject );
            }
            transform.localPosition = Vector3.Lerp( fromPos, toPos, Mathf.Pow( delta, 2.0f ) );
        }
    }

    public void PullTrigger( bool pullOut ) {
        pullOutState = pullOut;
        if ( pullOut ) {
            fromPos = transform.localPosition;
            toPos = new Vector3( ( Screen.width / transform.lossyScale.x ) / 2, 0, 0 );
        }
        else {
            fromPos = transform.localPosition;
            toPos = new Vector3( ( Screen.width / transform.lossyScale.x ), transform.localPosition.y );
        }

        delta = 0f;
        pullAnimationStart = Time.time;
    }

    public void AddOrRemovePanel( bool add ) {
        levelInfoPanelManager.AddOrRemovePanel( ( add ) ? levelId : -1 );
    }
    public void GetLeaderBoard() {
        levelInfoPanelManager.FetchLevelLeaderBoard( levelId );
    }

    public void SetInfo( (string, string, string, string, int, int, int, Sprite) info ) {
        courseId = info.Item1;
        levelName = info.Item2;
        levelDesc = info.Item3;
        levelJson = info.Item4;
        levelScoreTime = info.Item5;
        levelScoreAmount = info.Item6;
        levelScoreBlocks = info.Item7;
        levelPreviewImg = info.Item8;
        DisplayInfo();
    }
    public void DisplayInfo() {
        TMP_Name.text = levelName;
        TMP_Desc.text = levelDesc;
        TMP_Creator.text = levelCreator;
        T_img.GetComponent<Image>().sprite = levelPreviewImg;
    }

    public void EnterLevel() {
        VariablesStorage.levelId = levelId;
        VariablesStorage.courseId = courseId;
        VariablesStorage.levelJson = levelJson;
        VariablesStorage.levelTime = levelScoreTime;
        VariablesStorage.levelAmount = levelScoreAmount;
        VariablesStorage.levelBlocks = levelScoreBlocks;
        SceneManager.LoadScene( "SampleScene" );
    }
}
