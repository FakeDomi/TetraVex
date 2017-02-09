using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TetraVex
{
    internal class ControlArea
    {
        private const int ButtonSize = 50;

        private static Texture2D squareButtonBase, squareButtonDown;

        internal static void LoadTextures(ContentManager content)
        {
            squareButtonBase = content.Load<Texture2D>("buttonSquare");
            squareButtonDown = content.Load<Texture2D>("buttonSquareDown");
        }

        private readonly int gridSize;

        private int selectedButton = -1;
        private bool selectedButtonDown;

        internal ControlArea(int gridSize)
        {
            this.gridSize = gridSize;
        }

        internal void Update(GameState state, bool currentLeftMouseButtonState, TetraVex tetraVexInstance)
        {
            int baseX = this.gridSize * TetraVex.TileSize;
            int baseY = TetraVex.TileSize + this.gridSize * TetraVex.TileSize;
            int hoverButton = -1;

            if (state.MouseY >= baseY && state.MouseY < baseY + ButtonSize && state.MouseX >= baseX && state.MouseX < baseX + 4 * ButtonSize)
            {
                hoverButton = (state.MouseX - baseX) / ButtonSize;
            }

            if (!state.LeftMouseButtonDown && currentLeftMouseButtonState)
            {
                this.selectedButton = hoverButton >= 0 ? hoverButton : -1;
                this.selectedButtonDown = true;
            }
            else if (state.LeftMouseButtonDown && currentLeftMouseButtonState)
            {
                this.selectedButtonDown = hoverButton == this.selectedButton;
            }
            else if (state.LeftMouseButtonDown && !currentLeftMouseButtonState)
            {
                if (hoverButton == this.selectedButton && this.selectedButton >= 0)
                {
                    tetraVexInstance.SetSize(3 + this.selectedButton);
                }

                this.selectedButton = -1;
                this.selectedButtonDown = false;
            }
        }

        internal void Draw(GameState state, TetraVex tetraVexInstance)
        {
            int baseX = this.gridSize * TetraVex.TileSize;
            int baseY = TetraVex.TileSize + this.gridSize * TetraVex.TileSize;

            int offsetX = 0, offsetY = 0;

            if (tetraVexInstance.State.DiscoMode)
            {
                offsetX = tetraVexInstance.Random.Next(10) - 5;
                offsetY = tetraVexInstance.Random.Next(10) - 5;
            }

            state.SpriteBatch.Draw(squareButtonBase, new Rectangle(baseX + offsetX, baseY + offsetY, ButtonSize, ButtonSize), Color.White);

            if (tetraVexInstance.State.DiscoMode)
            {
                offsetX = tetraVexInstance.Random.Next(10) - 5;
                offsetY = tetraVexInstance.Random.Next(10) - 5;
            }

            state.SpriteBatch.Draw(squareButtonBase, new Rectangle(baseX + ButtonSize + offsetX, baseY + offsetY, ButtonSize, ButtonSize), Color.White);

            if (tetraVexInstance.State.DiscoMode)
            {
                offsetX = tetraVexInstance.Random.Next(10) - 5;
                offsetY = tetraVexInstance.Random.Next(10) - 5;
            }

            state.SpriteBatch.Draw(squareButtonBase, new Rectangle(baseX + 2 * ButtonSize + offsetX, baseY + offsetY, ButtonSize, ButtonSize), Color.White);

            if (tetraVexInstance.State.DiscoMode)
            {
                offsetX = tetraVexInstance.Random.Next(10) - 5;
                offsetY = tetraVexInstance.Random.Next(10) - 5;
            }

            state.SpriteBatch.Draw(squareButtonBase, new Rectangle(baseX + 3 * ButtonSize + offsetX, baseY + offsetY, ButtonSize, ButtonSize), Color.White);

            if (tetraVexInstance.State.DiscoMode)
            {
                offsetX = tetraVexInstance.Random.Next(10) - 5;
                offsetY = tetraVexInstance.Random.Next(10) - 5;
            }

            if (this.selectedButtonDown && this.selectedButton >= 0)
            {
                state.SpriteBatch.Draw(squareButtonDown, new Rectangle(baseX + this.selectedButton * ButtonSize + offsetX, baseY + offsetY, ButtonSize, ButtonSize), Color.White);
            }
        }
    }
}
