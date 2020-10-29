using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class LeaderBoard : MonoBehaviour {

    public NetworkManager networkManager = null;
    public TMP_InputField PINInput = null;
    public Transform content = null;
    public GameObject recordRowPrefab = null;
    private string orderBy = "class_member.member_id";
    private string roomId = null;
    private Coroutine currentCoroutine = null;

    public void OnEnable() {
        startFetching();
    }

    public void OnDisable() {
        if ( currentCoroutine != null ) {
            StopCoroutine( currentCoroutine );
        }
    }
    public void SetOrderBy( int num ) {
        if      ( num == 0 ) orderBy = "class_member.member_id";
        else if ( num == 1 ) orderBy = "num";
        else if ( num == 2 ) orderBy = "sum_score_time";
        else if ( num == 3 ) orderBy = "sum_score_amount";
        else if ( num == 4 ) orderBy = "sum_score_blocks";

        startFetching();
    }

    public void OpenLeaderBoard( bool roomSeted ) {
        roomId = "";
        if ( roomSeted ) {
            roomId = VariablesStorage.roomId;
        }
        else {
            roomId = PINInput.text;
        }

        gameObject.SetActive( true );
    }

    private void startFetching() {
        if ( currentCoroutine != null ) {
            StopCoroutine( currentCoroutine );
        }

        foreach ( Transform child in content ) {
            GameObject.Destroy( child.gameObject );
        }
        RecordRow record = Instantiate( recordRowPrefab, content ).GetComponent<RecordRow>();
        record.nameText.text = "";
        record.numText.text = "";
        record.scoreTimeText.text = "讀取中...";
        record.scoreAmountText.text = "";
        record.scoreBlocksText.text = "";

        currentCoroutine = StartCoroutine( DataFetch() );
    }

    IEnumerator DataFetch() {
        WaitForSeconds wait = new WaitForSeconds( 1f );
        string stmt = "";
        string jsonString = null;
        while ( true ) {
            Debug.Log( "leaderBoard fetching" );
            stmt = "SELECT * FROM class WHERE class_id = '" + roomId + "';";

            yield return StartCoroutine( networkManager.GetRequest( stmt, returnValue => {
                jsonString = returnValue;
            } ) );

            print( jsonString );

            if ( jsonString == null ) {
                Debug.Log( "sql Error" );
                //infoText.text = "SQL Error : Please contact us the error!";
                //infoText.color = new Color( 1, 0, 0 );
                yield break;
            }
            else if ( jsonString.Trim() == "[]" || jsonString.Trim() == "" ) {
                Debug.Log( "table empty" );
                //infoText.text = "找不到此房間!";
                //infoText.color = new Color( 1, 0, 0 );
                yield break;
            }

            stmt = "SELECT member_name, COUNT(*) AS num, SUM(score_time) AS sum_score_time, SUM(score_amount) AS sum_score_amount, SUM(score_blocks) AS sum_score_blocks FROM class_member LEFT JOIN play_record ON class_member.member_id = class_member.member_id GROUP BY class_member.member_id ORDER BY " + orderBy + ";";

            yield return StartCoroutine( networkManager.GetRequest( stmt, returnValue => {
                jsonString = returnValue;
            } ) );

            if ( jsonString == null ) {
                Debug.Log( "sql Error" );
                //infoText.text = "SQL Error : Please contact us the error!";
                //infoText.color = new Color( 1, 0, 0 );
                yield break;
            }
            else if ( jsonString.Trim() == "[]" || jsonString.Trim() == "" ) {
                Debug.Log( "table empty" );
                //infoText.text = "找不到此房間!";
                //infoText.color = new Color( 1, 0, 0 );
                yield break;
            }

            var jsonO = MiniJSON.Json.Deserialize( jsonString ) as List<object>;

            foreach ( Transform child in content ) {
                GameObject.Destroy( child.gameObject );
            }

            foreach ( Dictionary<string, object> item in jsonO ) {
                Dictionary<string, object> it = item as Dictionary<string, object>;
                string member_name = it["member_name"] as string;
                int num = 0;
                int sum_score_time = 0;
                int sum_score_amount = 0;
                int sum_score_blocks = 0;
                if ( it["sum_score_time"] != null ) {
                    num = int.Parse( it["num"] as string );
                    sum_score_time = (int)(long)it["sum_score_time"];
                    sum_score_amount = (int)(long)it["sum_score_amount"];
                    sum_score_blocks = (int)(long)it["sum_score_blocks"];
                }
                RecordRow record = Instantiate( recordRowPrefab, content ).GetComponent<RecordRow>();

                record.nameText.text = member_name;
                record.numText.text = num.ToString();
                record.scoreTimeText.text = sum_score_time.ToString();
                record.scoreAmountText.text = sum_score_amount.ToString();
                record.scoreBlocksText.text = sum_score_blocks.ToString();
            }

            yield return wait;
        }
    }
}
