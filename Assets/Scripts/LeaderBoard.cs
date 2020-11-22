using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class LeaderBoard : MonoBehaviour {
    public TMP_InputField PINInput = null;
    public Transform content = null;
    public GameObject recordRowPrefab = null;
    public TMP_Text courseName = null;
    private string orderBy = "class_member.member_id";
    private string roomId = null;
    private string courseId = null;
    private Coroutine currentCoroutine = null;

    public void OnEnable() {
        startFetching();
    }

    public void OnDisable() {
        if ( currentCoroutine != null ) {
            StopCoroutine( currentCoroutine );
        }
        courseId = null;
        courseName.text = "全部總分";
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
        courseId = null;
        courseName.text = roomId + courseName.text;
        gameObject.SetActive( true );
    }

    public void OpenLeaderBoard( bool roomSeted, string courseId, string courseName ) {
        roomId = "";
        if ( roomSeted ) {
            roomId = VariablesStorage.roomId;
        }
        else {
            roomId = PINInput.text;
        }
        this.courseId = courseId;
        this.courseName.text = courseName;
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

            yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
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

            if ( courseId == null )
                stmt = "SELECT member_name, COUNT(*) AS num, SUM(score_time) AS sum_score_time, SUM(score_amount) AS sum_score_amount, SUM(score_blocks) AS sum_score_blocks FROM class_member LEFT JOIN play_record ON class_member.member_id = play_record.member_id WHERE class_id = '" + roomId + "' GROUP BY class_member.member_id ORDER BY " + orderBy + ";";
            else
                stmt = "SELECT member_name, COUNT(*) AS num, SUM(score_time) AS sum_score_time, SUM(score_amount) AS sum_score_amount, SUM(score_blocks) AS sum_score_blocks FROM class_member LEFT JOIN play_record ON class_member.member_id = play_record.member_id WHERE class_id = '" + roomId + "' AND course_id = '" + courseId + "' GROUP BY class_member.member_id ORDER BY " + orderBy + ";";

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
                foreach ( Transform child in content ) {
                    GameObject.Destroy( child.gameObject );
                }
                Debug.Log( "table empty" );
                RecordRow record = Instantiate( recordRowPrefab, content ).GetComponent<RecordRow>();
                record.nameText.text = "";
                record.numText.text = "";
                record.scoreTimeText.text = "尚未有遊玩紀錄";
                record.scoreAmountText.text = "";
                record.scoreBlocksText.text = "";
                yield break;
            }

            var jsonO = MiniJSON.Json.Deserialize( jsonString ) as List<object>;

            foreach ( Transform child in content ) {
                GameObject.Destroy( child.gameObject );
            }
            int num = 0;
            int sum_score_time = 0;
            int sum_score_amount = 0;
            int sum_score_blocks = 0;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2( content.GetComponent<RectTransform>().sizeDelta.x, 50 * jsonO.Count );

            foreach ( Dictionary<string, object> item in jsonO ) {
                Dictionary<string, object> it = item as Dictionary<string, object>;
                string member_name = it["member_name"] as string;
                num = 0;
                sum_score_time = 0;
                sum_score_amount = 0;
                sum_score_blocks = 0;
                if ( it["sum_score_time"] != null ) {
                    num = int.Parse( it["num"] as string );
                    sum_score_time = int.Parse( it["sum_score_time"] as string );
                    sum_score_amount = int.Parse( it["sum_score_amount"] as string );
                    sum_score_blocks = int.Parse( it["sum_score_blocks"] as string );
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
