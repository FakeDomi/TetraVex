using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace TetraVex
{
    internal class GameState
    {
        internal SpriteBatch SpriteBatch { get; set; }

        internal bool LeftMouseButtonDown { get; set; }
        internal bool RightMouseButtonDown { get; set; }

        internal int MouseX { get; set; }
        internal int MouseY { get; set; }

        internal Tile HoverTile { get; set; }
        internal Tile FloatingTile { get; set; }

        internal int FloatingTileX { get; set; }
        internal int FloatingTileY { get; set; }
        internal int FloatingTileDx { get; set; }
        internal int FloatingTileDy { get; set; }

        internal int DropBoard { get; set; }
        internal int DropColumn { get; set; }
        internal int DropRow { get; set; }

        internal int DropStartBoard { get; set; }
        internal int DropStartColumn { get; set; }
        internal int DropStartRow { get; set; }

        internal Stopwatch Stopwatch { get; set; }

        internal bool Locked { get; set; }
        internal bool DevMode { get; set; }
        internal bool DiscoMode { get; set; }
    }
}
