using UnityEngine;

namespace ReverseTD.Defense
{
    /// <summary>
    /// White sprites made in code, one world unit across with a centered pivot.
    /// Tint them with <see cref="SpriteRenderer.color"/> and size them with the transform's scale.
    /// Use only at runtime: sprites made in Edit mode would be saved into scenes as missing references.
    /// </summary>
    public static class ShapeSprites
    {
        private const int SquareSize = 4;
        private const int CircleSize = 64;
        private const int RingSize = 256;
        private const float RingThickness = 4f; // in texture pixels

        private static Sprite square;
        private static Sprite circle;
        private static Sprite ring;

        // Domain reload is disabled, so statics survive between Play sessions unless cleared here.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            square = null;
            circle = null;
            ring = null;
        }

        public static Sprite Square
        {
            get
            {
                if (square == null)
                {
                    square = CreateSprite("RK Square", SquareSize, SquareAlpha);
                }
                return square;
            }
        }

        public static Sprite Circle
        {
            get
            {
                if (circle == null)
                {
                    circle = CreateSprite("RK Circle", CircleSize, CircleAlpha);
                }
                return circle;
            }
        }

        public static Sprite Ring
        {
            get
            {
                if (ring == null)
                {
                    ring = CreateSprite("RK Ring", RingSize, RingAlpha);
                }
                return ring;
            }
        }

        private static float SquareAlpha(float distance, float radius)
        {
            return 1f;
        }

        // The +0.5 terms fade each edge over one pixel, for anti-aliasing.
        private static float CircleAlpha(float distance, float radius)
        {
            return radius - distance + 0.5f;
        }

        private static float RingAlpha(float distance, float radius)
        {
            return Mathf.Min(radius - distance + 0.5f, distance - (radius - RingThickness) + 0.5f);
        }

        // alphaAt(distance from center, radius) gives each pixel's opacity; both are in pixels.
        private static Sprite CreateSprite(string spriteName, int size, System.Func<float, float, float> alphaAt)
        {
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                name = spriteName,
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            float radius = size * 0.5f;
            var center = new Vector2(radius, radius);
            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    float alpha = Mathf.Clamp01(alphaAt(distance, radius));
                    pixels[y * size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);

            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size, 0, SpriteMeshType.FullRect);
            sprite.name = spriteName;
            return sprite;
        }
    }
}
