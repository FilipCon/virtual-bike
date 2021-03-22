using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapToTexture2D : MonoBehaviour
{
    public static Texture2D Convert(Tilemap tm)
    {
        Sprite sprite = null;
        int minX = 0, maxX = 0, minY = 0, maxY = 0;

        for (int x = 0; x < tm.size.x; ++x)
        {
            for (int y = 0; y < tm.size.y; ++y)
            {
                var pos = new Vector3Int(-x, -y, 0);
                if (tm.GetSprite(pos) != null)
                {
                    sprite = tm.GetSprite(pos); // select any sprite to later know the dimensions of the sprites
                    if (minX > pos.x) minX = pos.x;
                    if (minY > pos.y) minY = pos.y;
                }

                pos = new Vector3Int(x, y, 0);
                if (tm.GetSprite(pos) != null)
                {
                    if (maxX < pos.x) maxX = pos.x;
                    if (maxY < pos.y) maxY = pos.y;
                }
            }
        }

        // find the size of the sprite in pixels
        float width = sprite.rect.width;
        float height = sprite.rect.height;

        // create a texture with the size multiplied by the number of cells
        Texture2D texture2D = new Texture2D((int)width * tm.size.x, (int)height * tm.size.y);

        // assign the entire invisible image
        Color[] color = new Color[texture2D.width * texture2D.height];
        for (int i = 0; i < color.Length; ++i)
            color[i] = new Color(0f, 0f, 0f, 0f);

        texture2D.SetPixels(0, 0, texture2D.width, texture2D.height, color);

        // assign each block its respective pixels
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                if (tm.GetSprite(new Vector3Int(x, y, 0)) != null)
                    // lower the pixels so that minX = 0 and minY = 0
                    texture2D.SetPixels((x - minX) * (int)width, (y - minY) * (int)height, (int)width, (int)height, GetCurrentSprite(tm.GetSprite(new Vector3Int(x, y, 0))).GetPixels());

            }
        }
        texture2D.Apply();

        return texture2D;
    }


    private static Texture2D GetCurrentSprite(Sprite sprite)
    {
        var pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                              (int)sprite.textureRect.y,
                                              (int)sprite.textureRect.width,
                                              (int)sprite.textureRect.height);

        Texture2D texture = new Texture2D((int)sprite.textureRect.width,
                                          (int)sprite.textureRect.height);

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
