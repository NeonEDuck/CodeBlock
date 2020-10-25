using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelInfoPanel : MonoBehaviour {

    public int levelId = 0;
    public LevelInfoPanelManager levelInfoPanelManager = null;

    public TMP_Text TMP_Name = null;
    public TMP_Text TMP_Desc = null;
    public TMP_Text TMP_Creator = null;

    public string levelName = "";
    public string levelDesc = "";
    public string levelCreator = "";
    public string levelJson = "";

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
            toPos = new Vector3( Screen.width / 2, 0, 0 );
        }
        else {
            fromPos = transform.localPosition;
            toPos = new Vector3( Screen.width, transform.localPosition.y );
        }

        delta = 0f;
        pullAnimationStart = Time.time;
    }

    public void AddOrRemovePanel( bool add ) {
        levelInfoPanelManager.AddOrRemovePanel( ( add ) ? levelId : -1 );
    }

    public void SetInfo( (string, string, string) info ) {
        levelName = info.Item1;
        levelDesc = info.Item2;
        levelJson = info.Item3;
        DisplayInfo();
    }
    public void DisplayInfo() {
        TMP_Name.text = levelName;
        TMP_Desc.text = levelDesc;
        TMP_Creator.text = levelCreator;
    }

    public void EnterLevel() {
        VariablesStorage.levelId = levelId;
        VariablesStorage.levelJson = levelJson;
        SceneManager.LoadScene( "SampleScene" );
    }
}
