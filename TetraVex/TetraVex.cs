using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TetraVex
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    internal class TetraVex : Game
    {
        internal const int DefaultGridSize = 3;
        internal const int BorderSize = 50;
        internal const int TileSize = 100;
        internal const int HalfTileSize = 50;
        internal const int BottomAreaSize = 100;

        internal int GridSize { get; set; }
        internal Random Random { get; set; }
        internal GameState State { get; set; }

        private readonly GraphicsDeviceManager graphics;
        private Color backColor = new Color(230, 230, 230);

        private SpriteBatch spriteBatch;
        private ControlArea controlArea;
        private Board board;

        private int tFrames;
        private int vFrames;
        private int currentSecond;

        internal TetraVex()
        {
            this.State = new GameState { DropBoard = -1, Stopwatch = new Stopwatch() };

            this.Random = new Random();

            this.graphics = new GraphicsDeviceManager(this);
            this.Content.RootDirectory = "Content";

            this.Content = new ResourceContentManager(this.Services, Resources.ResourceManager);

            this.SetSize(DefaultGridSize);

            this.IsMouseVisible = true;

            this.TargetElapsedTime = new TimeSpan(50000);
            this.graphics.SynchronizeWithVerticalRetrace = false;
            this.IsFixedTimeStep = true;
        }

        internal void SetSize(int gridSize)
        {
            if (this.GridSize != gridSize)
            {
                int diff = this.GridSize - gridSize;

                this.GridSize = gridSize;

                this.board = new Board(this.GridSize);
                this.controlArea = new ControlArea(this.GridSize);

                this.graphics.PreferredBackBufferHeight = (this.GridSize + 1) * TileSize + BottomAreaSize;
                this.graphics.PreferredBackBufferWidth = (this.GridSize + 1) * 2 * TileSize;
                this.graphics.ApplyChanges();

                this.Window.Position = new Point(this.Window.Position.X + diff * TileSize, this.Window.Position.Y + diff * HalfTileSize);
            }

            this.PopulateBoard();
        }

        internal void PopulateBoard()
        {
            Tile[] tiles = new Tile[this.GridSize * this.GridSize];

            for (int y = 0; y < this.GridSize; y++)
            {
                for (int x = 0; x < this.GridSize; x++)
                {
                    int index = x + this.GridSize * y;

                    tiles[index] = new Tile(this.Random.Next(10), this.Random.Next(10), this.Random.Next(10), this.Random.Next(10));

                    if (x > 0)
                    {
                        tiles[index].LeftValue = tiles[index - 1].RightValue;
                    }

                    if (y > 0)
                    {
                        tiles[index].TopValue = tiles[index - this.GridSize].BottomValue;
                    }
                }
            }

            for (int y = 0; y < this.GridSize; y++)
            {
                for (int x = 0; x < this.GridSize; x++)
                {
                    int tileIndex = this.Random.Next(tiles.Length - (x + this.GridSize * y));

                    this.board[0, x, y] = null;
                    this.board[1, x, y] = tiles[tileIndex];
                    tiles[tileIndex] = tiles[tiles.Length - (x + this.GridSize * y) - 1];
                }
            }

            this.State.Stopwatch.Restart();
            this.State.Locked = false;
        }

        private bool CheckBoard()
        {
            return this.CheckBoard(0) || this.CheckBoard(1);
        }

        private bool CheckBoard(int boardId)
        {
            for (int y = 0; y < this.GridSize; y++)
            {
                for (int x = 0; x < this.GridSize; x++)
                {
                    Tile thisTile = this.board[boardId, x, y];

                    if (thisTile == null || x > 0 && this.board[boardId, x - 1, y].RightValue != thisTile.LeftValue || y > 0 && this.board[boardId, x, y - 1].BottomValue != thisTile.TopValue)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.State.SpriteBatch = this.spriteBatch;

            Board.LoadTextures(this.Content);
            Tile.LoadTextures(this.Content);
            ControlArea.LoadTextures(this.Content);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            MouseState mouseState = Mouse.GetState();

            this.controlArea.Update(this.State, mouseState.LeftButton == ButtonState.Pressed, this);

            this.State.MouseX = mouseState.X;
            this.State.MouseY = mouseState.Y;

            int boardId, column, row;

            this.board.GetSlotAt(this.State.MouseX, this.State.MouseY, out boardId, out column, out row);

            Tile tile = this.board[boardId, column, row];

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (this.State.LeftMouseButtonDown)
                {
                    // LMB is being held.

                    if (this.State.FloatingTile != null)
                    {
                        this.State.FloatingTileX = this.State.MouseX - this.State.FloatingTileDx;
                        this.State.FloatingTileY = this.State.MouseY - this.State.FloatingTileDy;

                        this.board.GetSlotAt(this.State.FloatingTileX + HalfTileSize, this.State.FloatingTileY + HalfTileSize, out boardId, out column, out row);

                        this.State.DropBoard = boardId;
                        this.State.DropColumn = column;
                        this.State.DropRow = row;
                    }
                }
                else
                {
                    // LMB went down.

                    if (tile == null && column == 2 && row == 2 && Keyboard.GetState().IsKeyDown(Keys.LeftControl))
                    {
                        this.State.DevMode = true;
                        this.backColor = new Color(60, 60, 64);
                    }

                    if (tile != null && !this.State.Locked)
                    {
                        int x, y;

                        this.board.GetSlotCoords(boardId, column, row, out x, out y);

                        this.State.FloatingTile = tile;
                        this.State.FloatingTileX = x;
                        this.State.FloatingTileY = y;
                        this.State.FloatingTileDx = this.State.MouseX - x;
                        this.State.FloatingTileDy = this.State.MouseY - y;

                        this.board[boardId, column, row] = null;

                        this.board.GetSlotAt(this.State.FloatingTileX + HalfTileSize, this.State.FloatingTileY + HalfTileSize, out boardId, out column, out row);

                        this.State.DropBoard = boardId;
                        this.State.DropColumn = column;
                        this.State.DropRow = row;

                        this.State.DropStartBoard = boardId;
                        this.State.DropStartColumn = column;
                        this.State.DropStartRow = row;
                    }
                }

                this.State.LeftMouseButtonDown = true;
            }
            else if (this.State.LeftMouseButtonDown)
            {
                // LMB was released.

                this.State.LeftMouseButtonDown = false;

                if (this.State.FloatingTile != null)
                {
                    if (this.State.DropBoard >= 0)
                    {
                        this.board[this.State.DropStartBoard, this.State.DropStartColumn, this.State.DropStartRow] = this.board[this.State.DropBoard, this.State.DropColumn, this.State.DropRow];
                        this.board[this.State.DropBoard, this.State.DropColumn, this.State.DropRow] = this.State.FloatingTile;

                        if (this.State.DevMode && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                        {
                            Tile oldTile = this.State.FloatingTile;

                            Tile topTile = this.board[this.State.DropBoard, this.State.DropColumn, this.State.DropRow - 1];
                            Tile leftTile = this.board[this.State.DropBoard, this.State.DropColumn - 1, this.State.DropRow];
                            Tile bottomTile = this.board[this.State.DropBoard, this.State.DropColumn, this.State.DropRow + 1];
                            Tile rightTile = this.board[this.State.DropBoard, this.State.DropColumn + 1, this.State.DropRow];

                            this.board[this.State.DropBoard, this.State.DropColumn, this.State.DropRow] = new Tile(topTile != null ? topTile.BottomValue : oldTile.TopValue, leftTile != null ? leftTile.RightValue : oldTile.LeftValue, bottomTile != null ? bottomTile.TopValue : oldTile.BottomValue, rightTile != null ? rightTile.LeftValue : oldTile.RightValue);
                        }
                    }
                    else
                    {
                        this.board[this.State.DropStartBoard, this.State.DropStartColumn, this.State.DropStartRow] = this.State.FloatingTile;
                    }

                    this.State.FloatingTile = null;
                    this.State.DropBoard = -1;

                    if (this.CheckBoard())
                    {
                        this.State.Stopwatch.Stop();
                        this.State.Locked = true;
                    }
                }
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                if (!this.State.RightMouseButtonDown)
                {
                    // RMB went down.

                    this.State.RightMouseButtonDown = true;
                }
            }
            else if (this.State.RightMouseButtonDown)
            {
                // RMB went up.

                this.State.RightMouseButtonDown = false;

                if (this.board[boardId, column, row] != null && !this.State.Locked)
                {
                    int otherBoard = 1 - boardId;

                    for (int checkY = 0; checkY < this.GridSize; checkY++)
                    {
                        for (int checkX = 0; checkX < this.GridSize; checkX++)
                        {
                            if (this.board[otherBoard, checkX, checkY] == null)
                            {
                                this.board[otherBoard, checkX, checkY] = this.board[boardId, column, row];
                                this.board[boardId, column, row] = null;
                            }
                        }
                    }

                    if (this.CheckBoard())
                    {
                        this.State.Stopwatch.Stop();
                        this.State.Locked = true;
                    }
                }
            }

            if (this.State.DevMode && this.tFrames % 10 == 0)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    this.backColor = new Color(this.Random.Next(255), this.Random.Next(255), this.Random.Next(255));
                    this.State.DiscoMode = true;
                }
                else
                {
                    this.backColor = new Color(60, 60, 64);
                    this.State.DiscoMode = false;
                }
            }

            if (tile != null && this.State.FloatingTile == null)
            {
                this.State.HoverTile = tile;
            }
            else
            {
                this.State.HoverTile = null;
            }

            base.Update(gameTime);

            if (DateTime.Now.Second == this.currentSecond)
            {
                this.tFrames++;
            }
            else
            {
                TimeSpan timestamp = this.State.Stopwatch.Elapsed;
                int totalHous = (int)timestamp.TotalHours;

                if (this.State.Locked)
                {
                    this.Window.Title = "TetraVex | Your Time: " + (totalHous > 0 ? totalHous + ":" : "") + (timestamp.Minutes < 10 ? "0" : "") + timestamp.Minutes + (timestamp.Seconds < 10 ? ":0" : ":") + timestamp.Seconds + "." + timestamp.Milliseconds.ToString().PadLeft(3, '0');
                }
                else
                {
                    this.Window.Title = "TetraVex | " + (totalHous > 0 ? totalHous + ":" : "") + (timestamp.Minutes < 10 ? "0" : "") + timestamp.Minutes + (timestamp.Seconds < 10 ? ":0" : ":") + timestamp.Seconds + " | " + this.tFrames + " TPS, " + this.vFrames + " FPS";
                }

                this.tFrames = 1;
                this.vFrames = 0;
                this.currentSecond = DateTime.Now.Second;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(this.backColor);

            this.spriteBatch.Begin();
            this.controlArea.Draw(this.State, this);
            this.board.Draw(this.State, this);
            this.spriteBatch.End();

            base.Draw(gameTime);

            this.vFrames++;
        }
    }
}
