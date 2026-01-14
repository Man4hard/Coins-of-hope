using UnityEngine;

namespace CoinsOfHope.Gameplay
{
    public static class SpriteFactory
    {
        private static Sprite _whiteSquare;

        public static Sprite WhiteSquare
        {
            get
            {
                if (_whiteSquare != null) return _whiteSquare;

                var texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
                _whiteSquare = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100f);
                return _whiteSquare;
            }
        }
    }
}
