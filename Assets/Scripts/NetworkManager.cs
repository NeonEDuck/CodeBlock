using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour {
    public VariablesStorage variablesStorage = null;
    public LevelInfoPanelManager levelInfoPanelManager = null;

    void Start() {

#if !UNITY_EDITOR
        Application.ExternalCall( "UnityStartup" );
#endif
    }

    public void SetVariables( string json ) {
        var jsonO = MiniJSON.Json.Deserialize( json ) as Dictionary<string, object>;
        variablesStorage.userId = jsonO["userId"] as string;
        variablesStorage.hostname = jsonO["hostname"] as string;
        if ( !variablesStorage.hostname.StartsWith("http:") ) {
            variablesStorage.hostname = "http://" + variablesStorage.hostname;
        }
        Debug.Log( variablesStorage.userId );
        Debug.Log( variablesStorage.hostname );
        levelInfoPanelManager.ReloadLevels();
    }


    public void GetJson( string jsonresponse ) {

        Debug.Log( jsonresponse );

    }

    //public void SendData( string jsonstring ) {
    //    jsonstring = Random.Range( 1, 1000 ).ToString();
    //}

    public IEnumerator GetRequest( string stmt, System.Action<string> callback = null ) {
        WWWForm form = new WWWForm();
        form.AddField( "stmt", stmt );

        using ( UnityWebRequest webRequest = UnityWebRequest.Post( variablesStorage.hostname + "/sql", form ) ) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if ( webRequest.isNetworkError ) {
                Debug.Log( ": Error: " + webRequest.error );
            }
            else {
                Debug.Log( ":Received: " + webRequest.downloadHandler.text );
                callback( webRequest.downloadHandler.text );
            }
        }
    }

}