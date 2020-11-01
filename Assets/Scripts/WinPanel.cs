using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour {
    public TMP_Text time = null;
    public TMP_Text amount = null;
    public TMP_Text blocks = null;
    public Transform newScoreText = null;
    public Button upload = null;

    void OnDisable() {
        upload.interactable = false;
        newScoreText.gameObject.SetActive( false );
    }
}
