using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TetraVex
{
    internal class Board
    {
        private static Texture2D slotTexture;
        private static Texture2D dropTexture;

        public static void LoadTextures(ContentManager content)
        {
            slotTexture = content.Load<Texture2D>("tileSlot");
            dropTexture = content.Load<Texture2D>("tileHover");
        }

        private readonly int gridSize;

        private readonly Tile[] leftGrid;
        private readonly Tile[] rightGrid;

        internal Board(int gridSize)
        {
            this.leftGrid = new Tile[gridSize * gridSize];
            this.rightGrid = new Tile[gridSize * gridSize];
            this.gridSize = gridSize;
        }

        internal Tile this[int board, int x, int y]
        {
            get { return x < 0 || x >= this.gridSize || y < 0 || y >= this.gridSize ? null : board == 0 ? this.leftGrid[y * this.gridSize + x] : board == 1 ? this.rightGrid[y * this.gridSize + x] : null; }
            set
            {
                if (board == 0)
                {
                    this.leftGrid[y * this.gridSize + x] = value;
                }
                else if (board == 1)
                {
                    this.rightGrid[y * this.gridSize + x] = value;
                }
            }
        }

        internal void GetSlotAt(int x, int y, out int board, out int column, out int row)
        {
            if (y >= TetraVex.BorderSize && y < TetraVex.BorderSize + this.gridSize * TetraVex.TileSize)
            {
                if (x >= TetraVex.BorderSize && x < TetraVex.BorderSize + this.gridSize * TetraVex.TileSize)
                {
                    board = 0;
                }
                else if (x >= TetraVex.BorderSize + TetraVex.TileSize + this.gridSize * TetraVex.TileSize && x < TetraVex.BorderSize + TetraVex.TileSize + 2 * this.gridSize * TetraVex.TileSize)
                {
                    board = 1;
                }
                else
                {
                    goto returnDefault;
                }

                x = x % ((this.gridSize + 1) * TetraVex.TileSize);

                column = (x - TetraVex.BorderSize) / TetraVex.TileSize;
                row = (y - TetraVex.BorderSize) / TetraVex.TileSize;

                return;
            }

            returnDefault:

            board = -1;
            column = -1;
            row = -1;
        }

        internal void GetSlotCoords(int board, int column, int row, out int slotX, out int slotY)
        {
            if (board >= 0)
            {
                slotX = TetraVex.BorderSize + board * (this.gridSize + 1) * TetraVex.TileSize + column * TetraVex.TileSize;
                slotY = TetraVex.BorderSize + row * TetraVex.TileSize;

                return;
            }

            slotX = -1;
            slotY = -1;
        }

        internal void Draw(GameState gameState, TetraVex tetraVexInstance)
        {
            for (int row = 0; row < this.gridSize; row++)
            {
                for (int column = 0; column < this.gridSize; column++)
                {
                    int index = column + row * this.gridSize;
                    Tile leftTile = this.leftGrid[index];
                    Tile rightTile = this.rightGrid[index];


                    int offsetX = 0, offsetY = 0;

                    if (tetraVexInstance.State.DiscoMode)
                    {
                        offsetX = tetraVexInstance.Random.Next(10) - 5;
                        offsetY = tetraVexInstance.Random.Next(10) - 5;
                    }

                    gameState.SpriteBatch.Draw(slotTexture, new Rectangle(TetraVex.BorderSize + column * TetraVex.TileSize + offsetX, TetraVex.BorderSize + row * TetraVex.TileSize + offsetY, TetraVex.TileSize, TetraVex.TileSize), Color.White);
                    gameState.SpriteBatch.Draw(slotTexture, new Rectangle(TetraVex.BorderSize + TetraVex.TileSize + (this.gridSize + column) * TetraVex.TileSize + offsetX, TetraVex.BorderSize + row * TetraVex.TileSize + offsetY, TetraVex.TileSize, TetraVex.TileSize), Color.White);

                    if (leftTile != null)
                    {
                        leftTile.Draw(gameState.SpriteBatch, TetraVex.BorderSize + column * TetraVex.TileSize, TetraVex.BorderSize + row * TetraVex.TileSize, leftTile == gameState.HoverTile && !gameState.Locked, tetraVexInstance);
                    }

                    if (rightTile != null)
                    {
                        rightTile.Draw(gameState.SpriteBatch, TetraVex.BorderSize + TetraVex.TileSize + (this.gridSize + column) * TetraVex.TileSize, TetraVex.BorderSize + row * TetraVex.TileSize, rightTile == gameState.HoverTile && !gameState.Locked, tetraVexInstance);
                    }
                }
            }

            int offsetX2 = 0, offsetY2 = 0;

            if (tetraVexInstance.State.DiscoMode)
            {
                offsetX2 = tetraVexInstance.Random.Next(10) - 5;
                offsetY2 = tetraVexInstance.Random.Next(10) - 5;
            }

            if (gameState.DropBoard >= 0)
            {
                gameState.SpriteBatch.Draw(dropTexture, new Rectangle(TetraVex.BorderSize + gameState.DropColumn * TetraVex.TileSize + gameState.DropBoard * (this.gridSize + 1) * TetraVex.TileSize + offsetX2, TetraVex.BorderSize + gameState.DropRow * TetraVex.TileSize + offsetY2, TetraVex.TileSize, TetraVex.TileSize), Color.White);
            }

            if (gameState.FloatingTile != null)
            {
                gameState.FloatingTile.Draw(gameState.SpriteBatch, gameState.FloatingTileX, gameState.FloatingTileY, true, tetraVexInstance);
            }
        }
    }
}
