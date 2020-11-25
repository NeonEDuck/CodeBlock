using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class LeaderBoard : MonoBehaviour {
    public TMP_InputField PINInput = null;
    public Transform title = null;
    public Transform content = null;
    public GameObject recordRowPrefab = null;
    public TMP_Text courseName = null;
    private string[] orderBy = { "class_member.member_id" , "num", "sum_score_time", "sum_score_amount", "sum_score_blocks" };
    private int orderId = 0;
    private bool ascending = true;
    private string roomId = null;
    private string courseId = null;
    private Coroutine currentCoroutine = null;
    private string validRoom = "";
    private Dictionary<string, RecordRow> recordList = new Dictionary<string, RecordRow>();

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

        if ( orderId == num ) {
            ascending = !ascending;
        }
        else {
            ascending = true;
        }

        orderId = num;

        foreach ( Transform child in title ) {
            child.GetChild( 1 ).gameObject.SetActive( false );
        }

        title.GetChild( orderId ).GetChild( 1 ).gameObject.SetActive( true );
        title.GetChild( orderId ).GetChild( 1 ).localRotation = Quaternion.Euler( new Vector3( 0, 0, ( ascending ) ? 0 : 180 ) );

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
        recordList.Clear();
        foreach ( Transform child in content ) {
            Destroy( child.gameObject );
        }

        stmt = $"SELECT * FROM class WHERE class_id = '{roomId}';";

        yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
            jsonString = returnValue;
        } ) );

        print( jsonString );

        if ( jsonString == null ) {
            Debug.Log( "sql Error" );
            yield break;
        }
        else if ( jsonString.Trim() == "[]" || jsonString.Trim() == "" ) {
            Debug.Log( "table empty" );
            content.GetChild( 0 ).GetComponent<RecordRow>().scoreTimeText.text = "找不到此房間!";
            yield break;
        }

        while ( true ) {
            Debug.Log( "leaderBoard fetching" );
            
            stmt = $"SELECT class_member.member_id, member_name, COUNT(*) AS num, coalesce(SUM(score_time),0) AS sum_score_time, coalesce(SUM(score_amount),0) AS sum_score_amount, coalesce(SUM(score_blocks),0) AS sum_score_blocks FROM class_member LEFT JOIN play_record ON class_member.member_id = play_record.member_id WHERE class_id = '{roomId}'{ ( ( courseId == null ) ? "" : $" AND course_id = '{courseId}'" ) } GROUP BY class_member.member_id ORDER BY {orderBy[orderId]} {((ascending)?"ASC":"DESC")};";

            yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
                jsonString = returnValue;
            } ) );

            if ( jsonString == null ) {
                Debug.Log( "sql Error" );
                yield break;
            }
            else if ( jsonString.Trim() == "[]" || jsonString.Trim() == "" ) {
                recordList.Clear();
                foreach ( Transform child in content ) {
                    Destroy( child.gameObject );
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

            int num = 0;
            int sum_score_time = 0;
            int sum_score_amount = 0;
            int sum_score_blocks = 0;

            float height = recordRowPrefab.GetComponent<RectTransform>().sizeDelta.y * jsonO.Count;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2( content.GetComponent<RectTransform>().sizeDelta.x, height );

            int i = 0;
            foreach ( Dictionary<string, object> item in jsonO ) {
                Dictionary<string, object> it = item as Dictionary<string, object>;
                string member_name = it["member_name"] as string;
                string member_id = it["member_id"] as string;

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

                RecordRow record;

                if ( recordList.ContainsKey( member_id ) ) {
                    record = recordList[member_id];
                }
                else {
                    record = Instantiate( recordRowPrefab, content ).GetComponent<RecordRow>();
                    recordList.Add( member_id, record );
                }

                record.nameText.text = member_name;
                record.numText.text = num.ToString();
                record.scoreTimeText.text = sum_score_time.ToString();
                record.scoreAmountText.text = sum_score_amount.ToString();
                record.scoreBlocksText.text = sum_score_blocks.ToString();
                record.ChangeOrder( i++ );
            }

            yield return wait;
        }
    }
}
