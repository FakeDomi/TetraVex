using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TetraVex
{
    internal class Tile
    {
        private static Texture2D topTexture, leftTexture, bottomTexture, rightTexture;
        private static Texture2D backgroundTexture, hoverTexture;

        private static readonly Color[] colors = new Color[10];

        internal static void LoadTextures(ContentManager content)
        {
            topTexture = content.Load<Texture2D>("tileTop");
            leftTexture = content.Load<Texture2D>("tileLeft");
            bottomTexture = content.Load<Texture2D>("tileBottom");
            rightTexture = content.Load<Texture2D>("tileRight");
            backgroundTexture = content.Load<Texture2D>("tileBackground");
            hoverTexture = content.Load<Texture2D>("tileHover");

            colors[0] = new Color(200, 0, 15);
            colors[1] = new Color(5, 200, 10);
            colors[2] = new Color(70, 70, 220);
            colors[3] = new Color(200, 200, 200);
            colors[4] = new Color(80, 80, 85);
            colors[5] = new Color(250, 130, 10);
            colors[6] = new Color(230, 220, 10);
            colors[7] = new Color(150, 0, 150);
            colors[8] = new Color(10, 200, 240);
            colors[9] = new Color(255, 0, 200);
        }

        internal int TopValue { get; set; }
        internal int LeftValue { get; set; }
        internal int BottomValue { get; set; }
        internal int RightValue { get; set; }

        internal Tile(int top, int left, int bottom, int right)
        {
            this.TopValue = top;
            this.LeftValue = left;
            this.BottomValue = bottom;
            this.RightValue = right;
        }

        internal void Draw(SpriteBatch spriteBatch, int x, int y, bool hoverEffect, TetraVex tetraVexInstance)
        {
            int offsetX = 0, offsetY = 0;

            if (tetraVexInstance.State.DiscoMode)
            {
                offsetX = tetraVexInstance.Random.Next(10) - 5;
                offsetY = tetraVexInstance.Random.Next(10) - 5;
            }

            spriteBatch.Draw(backgroundTexture, new Rectangle(x + offsetX, y + offsetY, TetraVex.TileSize, TetraVex.TileSize), Color.White);
            spriteBatch.Draw(topTexture, new Rectangle(x + offsetX, y + offsetY, TetraVex.TileSize, TetraVex.TileSize), colors[this.TopValue]);
            spriteBatch.Draw(leftTexture, new Rectangle(x + offsetX, y + offsetY, TetraVex.TileSize, TetraVex.TileSize), colors[this.LeftValue]);
            spriteBatch.Draw(bottomTexture, new Rectangle(x + offsetX, y + offsetY, TetraVex.TileSize, TetraVex.TileSize), colors[this.BottomValue]);
            spriteBatch.Draw(rightTexture, new Rectangle(x + offsetX, y + offsetY, TetraVex.TileSize, TetraVex.TileSize), colors[this.RightValue]);

            if (hoverEffect)
            {
                spriteBatch.Draw(hoverTexture, new Rectangle(x + offsetX, y + offsetY, TetraVex.TileSize, TetraVex.TileSize), Color.White);
            }
        }
    }
}
