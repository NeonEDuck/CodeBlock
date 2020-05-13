using UnityEngine;
using System.Collections;
using TMPro;

public class DefineBlockTrigger : MonoBehaviour {

    public BlockInfo blockInfo = null;
    public BlockSpawner blockSpawner = null;
    private TMP_InputField inputField = null;
    public string pre_name;

    // Use this for initialization
    void Start() {
        inputField = blockInfo.refField[0].GetComponent<TMP_InputField>();
    }

    // Update is called once per frame
    void Update() {
        if ( inputField.text.Length == 0 || transform.parent.parent.name.StartsWith( "BlockSpawner" ) ) {
            blockSpawner.trigger = false;
        }
        else {
            blockSpawner.ModifyArgs( "value_name", pre_name );
            blockSpawner.trigger = true;
            pre_name = inputField.text;
        }
    }
}
