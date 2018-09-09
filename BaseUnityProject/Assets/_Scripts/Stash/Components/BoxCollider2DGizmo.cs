using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class BoxCollider2DGizmo : MonoBehaviour {

    public Color color = Color.cyan;
    public Style style = Style.FULL_BOX;

    public enum Style {
        FULL_BOX,
        CORNERS,
    }

#if UNITY_EDITOR

    void OnDrawGizmos() {

        if (!enabled)
            return;

        // don't draw if selected (then Unity drawing takes over)
        if (UnityEditor.Selection.Contains(gameObject))
            return;

        switch (style) {
        case Style.FULL_BOX:
            drawFullBox();
            break;
        case Style.CORNERS:
            drawCorners();
            break;
        }

    }

#endif

    void drawFullBox() {

        BoxCollider2D bc2d = GetComponent<BoxCollider2D>();

        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(bc2d.offset.x - bc2d.size.x / 2, bc2d.offset.y - bc2d.size.y / 2);
        points[1] = new Vector2(bc2d.offset.x + bc2d.size.x / 2, bc2d.offset.y - bc2d.size.y / 2);
        points[2] = new Vector2(bc2d.offset.x + bc2d.size.x / 2, bc2d.offset.y + bc2d.size.y / 2);
        points[3] = new Vector2(bc2d.offset.x - bc2d.size.x / 2, bc2d.offset.y + bc2d.size.y / 2);
        for (int i = 0; i < points.Length; i++) {
            points[i] = transform.TransformPoint(points[i]);
        }

        Gizmos.color = color;
        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[1], points[2]);
        Gizmos.DrawLine(points[2], points[3]);
        Gizmos.DrawLine(points[3], points[0]);

    }

    void drawCorners() {

        BoxCollider2D bc2d = GetComponent<BoxCollider2D>();

        //float sizeMult = .1f;
        //float cornerW = bc2d.size.x * sizeMult;
        //float cornerH = bc2d.size.y * sizeMult;
        float size = 1;
        float cornerW = Mathf.Min(size, bc2d.size.x / 2);
        float cornerH = Mathf.Min(size, bc2d.size.y / 2);

        Vector2[] points = new Vector2[12];

        points[0] = new Vector2(bc2d.offset.x - bc2d.size.x / 2, bc2d.offset.y - bc2d.size.y / 2);
        points[1] = points[0] + new Vector2(cornerW, 0);
        points[2] = points[0] + new Vector2(0, cornerH);
        
        points[3] = new Vector2(bc2d.offset.x + bc2d.size.x / 2, bc2d.offset.y - bc2d.size.y / 2);
        points[4] = points[3] - new Vector2(cornerW, 0);
        points[5] = points[3] + new Vector2(0, cornerH);
        
        points[6] = new Vector2(bc2d.offset.x + bc2d.size.x / 2, bc2d.offset.y + bc2d.size.y / 2);
        points[7] = points[6] - new Vector2(cornerW, 0);
        points[8] = points[6] - new Vector2(0, cornerH);
        
        points[9] = new Vector2(bc2d.offset.x - bc2d.size.x / 2, bc2d.offset.y + bc2d.size.y / 2);
        points[10] = points[9] + new Vector2(cornerW, 0);
        points[11] = points[9] - new Vector2(0, cornerH);
        
        for (int i = 0; i < points.Length; i++) {
            points[i] = transform.TransformPoint(points[i]);
        }

        Gizmos.color = color;
        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[0], points[2]);
        
        Gizmos.DrawLine(points[3], points[4]);
        Gizmos.DrawLine(points[3], points[5]);

        Gizmos.DrawLine(points[6], points[7]);
        Gizmos.DrawLine(points[6], points[8]);

        Gizmos.DrawLine(points[9], points[10]);
        Gizmos.DrawLine(points[9], points[11]);

    }

    

}
