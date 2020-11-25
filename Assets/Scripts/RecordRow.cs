using UnityEngine;
using System.Collections;
using TMPro;
using System;

public class RecordRow : MonoBehaviour {
    public TMP_Text nameText = null;
    public TMP_Text numText = null;
    public TMP_Text scoreTimeText = null;
    public TMP_Text scoreAmountText = null;
    public TMP_Text scoreBlocksText = null;
    public int order = -1;
    public float timeStart = -1f;
    private float duration = 0.5f;
    private Vector3 lastPos;
    private Vector3 newPos;

    private void Update() {
        if ( timeStart != -1f ) {
            float t = Mathf.Min( 1f, ( Time.time - timeStart ) / duration );

            transform.localPosition = Vector3.Lerp( lastPos, newPos, Mathf.Pow( t, 2.0f ) );

            if ( t == 1f ) {
                timeStart = -1f;
            }
        }
    }

    public void ChangeOrder( int o ) {
        timeStart = Time.time;

        var rect = transform.GetComponent<RectTransform>();

        Vector3 pos = new Vector3( transform.parent.GetComponent<RectTransform>().sizeDelta.x / 2, -(o + 0.5f) * rect.sizeDelta.y, 0 );

        if ( order == -1 ) {
            transform.localPosition = pos;
        }

        order = o;

        lastPos = transform.localPosition;
        newPos = pos;
    }
}
