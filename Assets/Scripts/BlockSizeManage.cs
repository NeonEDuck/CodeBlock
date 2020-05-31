using TMPro;
using UnityEngine;

public class BlockSizeManage : MonoBehaviour {
    public BlockInfo blockInfo = null;

    public void ChangeText( string valueName ) {
        blockInfo.refField[0].GetComponent<TMP_Text>().text = valueName;
        //Vector2 size = new Vector2( blockInfo.refField[0].GetComponent<RectTransform>().sizeDelta.x + 36f, GetComponent<RectTransform>().sizeDelta.y );
        //GetComponent<RectTransform>().sizeDelta = size;
        //if ( transform.parent.parent.name.StartsWith( "Value" ) ) {
        //    transform.parent.parent.GetComponent<RectTransform>().sizeDelta = size;
        //}
    }

    //public void Update() {
    //    float offset = 70f;
    //    Vector3 pos = transform.position - new Vector3( transform.GetComponent<RectTransform>().sizeDelta.x / 2, 0, 0 );
    //    for ( int i = 0; i < transform.childCount; i++ ) {
    //        Transform child = transform.GetChild( i );
    //        if ( child.name.StartsWith( "Image" ) ) continue;
    //        float w = child.GetComponent<RectTransform>().sizeDelta.x;
    //        child.position = pos + new Vector3( (offset + w / 2), 0, 0 );
    //        offset += w;
    //    }
    //    transform.GetComponent<RectTransform>().sizeDelta = new Vector2( offset, transform.GetComponent<RectTransform>().sizeDelta.y );
    //    transform.position = new Vector3( (pos.x + offset / 2f) , transform.position.y, transform.position.z );
    //}
}
