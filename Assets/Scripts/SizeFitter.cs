using UnityEngine;
using System.Collections;

public static class SizeFitter {
    public static void CheckForChanges( Transform transform ) {
        RectTransform children = transform.GetComponentInChildren<RectTransform>();

        float min_x, max_x, min_y, max_y;
        min_x = max_x = transform.localPosition.x;
        min_y = max_y = transform.localPosition.y;

        foreach ( RectTransform child in children ) {
            Vector2 scale = child.sizeDelta;
            float temp_min_x, temp_max_x, temp_min_y, temp_max_y;

            temp_min_x = child.localPosition.x - ( scale.x / 2 );
            temp_max_x = child.localPosition.x + ( scale.x / 2 );
            temp_min_y = child.localPosition.y - ( scale.y / 2 );
            temp_max_y = child.localPosition.y + ( scale.y / 2 );

            if ( temp_min_x < min_x )
                min_x = temp_min_x;
            if ( temp_max_x > max_x )
                max_x = temp_max_x;

            if ( temp_min_y < min_y )
                min_y = temp_min_y;
            if ( temp_max_y > max_y )
                max_y = temp_max_y;
        }

        transform.GetComponent<RectTransform>().sizeDelta = new Vector2( max_x - min_x, max_y - min_y );
    }
}