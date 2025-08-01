using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class PixelLine : MonoBehaviour
{
    public Vector2Int start;
    public Vector2Int end;
    public Vector2Int resolution = new Vector2Int(128, 128); // size of pixel canvas

    private Texture2D lineTexture;

    void Start()
    {
        lineTexture = new Texture2D(resolution.x, resolution.y, TextureFormat.R8, false);
        lineTexture.filterMode = FilterMode.Point;
        ClearTexture();

        DrawBresenhamLine(start.x, start.y, end.x, end.y);
        lineTexture.Apply();

        GetComponent<Renderer>().material.SetTexture("_LineTex", lineTexture);
    }

    void ClearTexture()
    {
        Color32[] clearPixels = new Color32[resolution.x * resolution.y];
        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = new Color32(0, 0, 0, 0);
        lineTexture.SetPixels32(clearPixels);
    }

    void DrawBresenhamLine(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            lineTexture.SetPixel(x0, y0, Color.white);
            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx) { err += dx; y0 += sy; }
        }
    }
}