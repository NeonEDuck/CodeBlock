using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour {
    public VariablesStorage variablesStorage = null;

    void Start() {
        Application.ExternalCall( "UnityStartup" );
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
    }


    public void GetJson( string jsonresponse ) {

        Debug.Log( jsonresponse );

    }

    public void SendData( string jsonstring ) {
        jsonstring = Random.Range( 1, 1000 ).ToString();
        Debug.Log( variablesStorage.hostname + "/sql?json=" + jsonstring );
        StartCoroutine( GetRequest( variablesStorage.hostname + "/sql?json=" + jsonstring ) );
    }

    IEnumerator GetRequest( string uri ) {
        Debug.Log( uri );
        using ( UnityWebRequest webRequest = UnityWebRequest.Get( uri ) ) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if ( webRequest.isNetworkError ) {
                Debug.Log( ": Error: " + webRequest.error );
            }
            else {
                Debug.Log( ":Received: " + webRequest.downloadHandler.text );
            }

        }
    }

}