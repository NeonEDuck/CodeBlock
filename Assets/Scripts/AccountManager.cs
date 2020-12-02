using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class AccountManager : MonoBehaviour {

    public float time = 0f;
    public float interval = 30f;
    private Coroutine coroutine = null;

    private void Awake() {
        DontDestroyOnLoad( this );
    }

    private void Update() {
        time += Time.deltaTime;

        if ( VariablesStorage.roomOK ) {
            if ( time >= interval && coroutine == null ) {
                StartCoroutine( accountCheck() );
            }
        }

        while ( time >= interval )
            time -= interval;
    }


    IEnumerator accountCheck() {

        string stmt = $"select to_char(last_played, 'YYYY-MM-DD HH24:MI:SS:MS') as last_played from class_member where member_id = '{VariablesStorage.memberId}';";

        yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {

            if ( returnValue == null ) {
                Debug.Log( "sql Error" );
            }
            else if ( returnValue.Trim() == "[]" || returnValue.Trim() == "" ) {
                VariablesStorage.error = 1;
                VariablesStorage.roomOK = false;
                SceneManager.LoadScene( "GameListScene" );
            }
            else {
                var jsonO = MiniJSON.Json.Deserialize( returnValue ) as List<object>;

                var it = jsonO[0] as Dictionary<string, object>;
                string last_played = it["last_played"] as string;
                Debug.Log( last_played );

                if ( last_played != VariablesStorage.last_played ) {
                    VariablesStorage.error = 0;
                    VariablesStorage.roomOK = false;
                    SceneManager.LoadScene( "GameListScene" );
                }
            }
        }));
        Debug.Log( "ok" );
    }
}
