using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LevelInfoPanelManager : MonoBehaviour {

    Stack levelList = new Stack();
    public LevelInfoPanel gameInfoPanelPrefab = null;
    public Transform parentCanvas = null;

    public Dictionary<int, (string, string, string, string)> levelsInfo = new Dictionary<int, (string, string, string, string)>();

    void Start() {
        levelsInfo.Add( 1, ( "First Level", "Lorem ipsum dolor sit amet, orci erat morbi interdum erat, nibh wisi erat. Sed nulla urna, at vel, vitae aliquam imperdiet placerat scelerisque.", "Creator1", "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":2, \"MoveBlock\":0}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}") );
        levelsInfo.Add( 2, ( "Second Level", "Quis nullam massa eleifend egestas donec massa, velit dui accumsan, augue vivamus.", "Creator2", "{\"blocksList\":{\"StartBlock\":1, \"SetBlock\":1}, \"gameEnv\":\"001001010010001000100013120000000001000100\"}") );
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
