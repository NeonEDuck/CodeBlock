using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    public Transform loadingIcon = null;
    public TMP_InputField nameInput = null;
    public TMP_InputField PINInput = null;
    public TMP_Text infoText = null;
    public TMP_Text debugText = null;
    public TMP_Text welcomeText = null;
    public LevelInfoPanelManager lipm = null;
    public Transform blockPanel = null;
    public Transform namePanel = null;
    public TMP_Text nameText = null;

    void Awake() {
        if ( VariablesStorage.roomOK ) {
            gameObject.SetActive( false );
        }
        else {
            gameObject.SetActive( true );
            if ( VariablesStorage.last_played != "" ) {
                switch ( VariablesStorage.error ) {
                    case 0:
                        infoText.text = "已在其他裝置登入!";
                        break;
                    case 1:
                        infoText.text = "資料遺失，請嘗試登入一次!";
                        break;
                }
                infoText.color = new Color( 1, 0, 0 );
            }
            VariablesStorage.last_played = "";
        }
    }

    public void EnterRoom() {

        //string p = "abctetet";
        //Debug.Log( $"testing this {p} aaaa" );

        if ( nameInput.text.Trim() != "" ) {
            string name = nameInput.text.Trim();

            if ( name.Contains("#") ) {
                VariablesStorage.memberName = name.Split( "#".ToCharArray(), 2 )[0].Trim();
                VariablesStorage.memberPin = name.Split( "#".ToCharArray(), 2 )[1].Trim();
            }
            else {
                VariablesStorage.memberName = name;
                VariablesStorage.memberPin = null;
            }
        }
        else {
            infoText.text = "請輸入你的名字!";
            infoText.color = new Color( 1, 0, 0 );
            return;
        }

        if ( PINInput.text.Trim() != "" ) {
            VariablesStorage.roomId = PINInput.text.Trim();
        }
        else {
            infoText.text = "請輸入房間PIN!";
            infoText.color = new Color( 1, 0, 0 );
            return;
        }

        StartCoroutine( GetClass() );
    }

    IEnumerator GetClass() {
        loadingIcon.gameObject.SetActive( true );
        blockPanel.gameObject.SetActive( true );
        yield return StartCoroutine( CheckClass() );
        loadingIcon.gameObject.SetActive( false );
        blockPanel.gameObject.SetActive( false );
    }

    IEnumerator CheckClass() {

        string jsonString = null;
        string stmt = "SELECT * FROM class WHERE class_id = '" + VariablesStorage.roomId + "';";

        yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
            jsonString = returnValue;
        } ) );

        print( jsonString );

        infoText.text = "";
        infoText.color = new Color( 1, 1, 1 );

        if ( jsonString == null ) {
            Debug.Log( "sql Error" );
            infoText.text = "SQL Error : Please contact us the error!";
            infoText.color = new Color( 1, 0, 0 );
            yield break;
        }
        else if ( jsonString.Trim() == "[]" || jsonString.Trim() == "" ) {
            Debug.Log( "table empty" );
            infoText.text = "找不到此房間!";
            infoText.color = new Color( 1, 0, 0 );
            yield break;
        }


        var jsonO = MiniJSON.Json.Deserialize( jsonString ) as List<object>;

        Dictionary<string, object> it = jsonO[0] as Dictionary<string, object>;
        string school = it["school"] as string;
        string name = it["name"] as string;
        string email = it["email"] as string;
        int max_number = (int)(long)it["max_number"];
        string topics = it["topics"] as string;

        debugText.text = "class_id : " + VariablesStorage.roomId + "\n" +
            "school : " + school + "\n" +
            "name : " + name + "\n" +
            "email : " + email + "\n" +
            "max_number : " + max_number + "\n" +
            "topics : " + topics;


        if ( VariablesStorage.memberPin != null ) {
            stmt = $"SELECT * FROM class_member WHERE member_name = '{VariablesStorage.memberName}' AND class_id = '{VariablesStorage.roomId}' AND pin = '{VariablesStorage.memberPin}';";

            yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
                jsonString = returnValue;
            } ) );


            if ( jsonString == null ) {
                Debug.Log( "sql Error" );
                infoText.text = "SQL Error : Please contact us the error!";
                infoText.color = new Color( 1, 0, 0 );
                yield break;
            }
            else if ( jsonString.Trim() == "[]" || jsonString.Trim() == "" ) {
                Debug.Log( "memeber not found" );

                stmt = $"SELECT COUNT(*) FROM class_member WHERE member_name = '{VariablesStorage.memberName}' AND class_id = '{VariablesStorage.roomId}';";

                yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
                    jsonO = MiniJSON.Json.Deserialize( returnValue ) as List<object>;
                } ) );

                if ( int.Parse( ( jsonO[0] as Dictionary<string, object> )["count"] as string ) > 0 ) {
                    Debug.Log( "wrong pin" );
                    infoText.text = "Pin碼不對!請確認一下#後4碼是否正確!\n如有問題,可以請老師幫您確認";
                    infoText.color = new Color( 1, 0, 0 );
                    yield break;
                }
                else {
                    Debug.Log( "No member" );
                    infoText.text = "沒有此成員在房間裡!\n如果是初次進入的話,請將名字內的#刪除掉!";
                    infoText.color = new Color( 1, 0, 0 );
                    yield break;
                }
            }
            else {
                jsonO = MiniJSON.Json.Deserialize( jsonString ) as List<object>;

                it = jsonO[0] as Dictionary<string, object>;
                VariablesStorage.memberId = it["member_id"] as string;

                yield return StartCoroutine( NetworkManager.GetRequest( $"select updatetime('{VariablesStorage.memberId}');", returnValue => {
                    jsonO = MiniJSON.Json.Deserialize( returnValue ) as List<object>;

                    it = jsonO[0] as Dictionary<string, object>;
                    VariablesStorage.last_played = it["updatetime"] as string;
                } ) );
            }
        }
        else {


            stmt = $"SELECT COUNT(*) FROM class_member WHERE class_id = '{VariablesStorage.roomId}';";

            yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
                jsonO = MiniJSON.Json.Deserialize( returnValue ) as List<object>;
            } ) );

            if ( int.Parse( ( jsonO[0] as Dictionary<string, object> )["count"] as string ) < max_number ) {


                stmt = $"select member_id, pin from AddNewMember('{VariablesStorage.memberName}','{VariablesStorage.roomId}') as ( member_id char(8), pin char(4) )";

                yield return StartCoroutine( NetworkManager.GetRequest( stmt, returnValue => {
                    jsonO = MiniJSON.Json.Deserialize( returnValue ) as List<object>;

                    it = jsonO[0] as Dictionary<string, object>;
                    VariablesStorage.memberId = it["member_id"] as string;
                    VariablesStorage.memberPin = it["pin"] as string;
                } ) );


                yield return StartCoroutine( NetworkManager.GetRequest( $"select updatetime('{VariablesStorage.memberId}');", returnValue => {
                    jsonO = MiniJSON.Json.Deserialize( returnValue ) as List<object>;

                    it = jsonO[0] as Dictionary<string, object>;
                    VariablesStorage.last_played = it["updatetime"] as string;
                } ) );

                nameText.text = $"{VariablesStorage.memberName}#{VariablesStorage.memberPin}";
                namePanel.gameObject.SetActive( true );
            }
            else {
                Debug.Log( "Room is full" );
                infoText.text = "房間滿了!\n請通知老師擴大數量或進行人員管制!";
                infoText.color = new Color( 1, 0, 0 );
                yield break;
            }
        }

        VariablesStorage.roomOK = true;
        gameObject.SetActive( false );
        welcomeText.text = "歡迎" + VariablesStorage.memberName + "!";
        lipm.ReloadLevels();

        //var jsonO = MiniJSON.Json.Deserialize( jsonString ) as List<object>;
        //int i = 1;
        //foreach ( Dictionary<string, object> item in jsonO ) {
        //    Dictionary<string, object> it = item as Dictionary<string, object>;
        //    string course_name = it["course_name"] as string;
        //    string description = it["description"] as string;
        //    string course_json = it["course_json"] as string;
        //    course_json = course_json.Replace( "\n", "" ).Replace( "\t", "" );
        //    levelsInfo.Add( i, (course_name, description, course_json) );
        //    Transform btn = Instantiate( buttonPrefab, levelButtonHolder ).transform;
        //    int temp = i++;
        //    btn.GetComponent<Button>().onClick.AddListener( delegate { AddOrRemovePanel( temp ); } );
        //    btn.GetChild( 0 ).GetComponent<TMP_Text>().text = course_name;
        //}
    }
}
