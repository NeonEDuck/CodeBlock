using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GameVariable {
    public static MiniGameObject player = null;
    public static Dictionary<int, List<MiniGameObject>> gamePiece = new Dictionary<int, List<MiniGameObject>>();
    public static bool mouseInWindow = false;
}
