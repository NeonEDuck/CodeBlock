using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.IO;

public class DebugManager : MonoBehaviour {

    public Transform content = null;
    public GameObject gameViewTemplate = null;
    [Header( "MiniPrefab" )]
    public GameObject obstaclePrefab;
    public GameObject playerPrefab;
    public GameObject boxPrefab;
    public GameObject flagPrefab;
    public GameObject holePrefab;
    public GameObject buttonPrefab;
    public GameObject doorPrefab;

    void Start() {

        StartCoroutine( LoadLevels() );
    }


    IEnumerator LoadLevels() {
        string stmt = "SELECT course.*, topic_name FROM course LEFT JOIN topic ON course.topic_id = topic.topic_id order by course.course_id";

        yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
            var jsonO = MiniJSON.Json.Deserialize( returnValue ) as List<object>;


            foreach ( Dictionary<string, object> item in jsonO ) {
                Transform player = null;
                string gameEnv = "";
                Direction dir = Direction.DOWN;


                var jsonO2 = MiniJSON.Json.Deserialize( item["course_json"] as string ) as Dictionary<string, object>;

                Transform gameView = Instantiate( gameViewTemplate, content ).transform;

                gameView.GetComponentInChildren<TMP_Text>().text = item["topic_name"] as string + ',' + item["course_name"] as string;

                Vector3 origin = gameView.localPosition;

                gameEnv = jsonO2["gameEnv"] as string;
                gameEnv = gameEnv.Replace( "\n", "" );
                dir = Direction.DOWN;

                if ( jsonO2.ContainsKey( "playerDir" ) ) {
                    dir = (Direction)(long)jsonO2["playerDir"];
                }

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
                                    spawn.GetComponent<MiniGameObject>().direction = dir;
                                    player = spawn;
                                }
                                break;
                            case '3':
                            case 'b':
                                spawn = Instantiate( boxPrefab, gameView ).transform;
                                break;
                            case '4':
                            case 'f':
                                spawn = Instantiate( flagPrefab, gameView ).transform;
                                break;
                            case '5':
                            case 'h':
                                spawn = Instantiate( holePrefab, gameView ).transform;
                                break;
                            case '6':
                            case 'j':
                                spawn = Instantiate( buttonPrefab, gameView ).transform;
                                break;
                            case '7':
                            case 'd':
                                spawn = Instantiate( doorPrefab, gameView ).transform;
                                break;
                        }
                        if ( spawn != null ) {
                            int x = i % 7;
                            int y = (int)Mathf.Floor( i / 7 );
                            spawn.GetComponent<MiniGameObject>().debug = true;
                            spawn.localPosition = origin + new Vector3( ( x + 0.5f ) * 50f, -( y + 0.5f ) * 50f, 0f );
                        }
                    }
                }

            }

        } ) );

        //ScreenCapture.CaptureScreenshot( Application.dataPath + "ScreenShot.png" );
    }
}
