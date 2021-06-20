using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Terrarium
{
	public class Player
	{
		World _world;
		Camera _cam;

		int _currTileId = 1;
		int _currWallId = 0;

		public Player()
		{
			_world = Main.World;
			_cam = Main.Camera;
		}

		public void Update()
		{
			Vector2 mousePos = _cam.ScreenToWorldSpace(Input.MousePosition);
			Point tilePos = new Point((int)(mousePos.X / TileData.TILE_SIZE), (int)(mousePos.Y / TileData.TILE_SIZE));

			// Place/remove tiles
			if (Input.IsMouseButtonHeld(MouseButton.Left) && _world.GetTile(tilePos.X, tilePos.Y) == -1)
				_world.SetTile(tilePos.X, tilePos.Y, _currTileId);
			else if (Input.IsMouseButtonHeld(MouseButton.Right) && _world.GetWall(tilePos.X, tilePos.Y) == -1)
				_world.SetWall(tilePos.X, tilePos.Y, _currWallId);
			else if(Input.IsMouseButtonDown(MouseButton.Middle))
			{
				if (_world.GetTile(tilePos.X, tilePos.Y) != -1)
					_world.SetTile(tilePos.X, tilePos.Y, -1);
				else
					_world.SetWall(tilePos.X, tilePos.Y, -1);
			}

			// Select current tile
			if (Input.IsKeyDown(Keys.D1))
				_currTileId = GameData.GetTileIdFromStrId("tile.dirt");
			else if (Input.IsKeyDown(Keys.D2))
				_currTileId = GameData.GetTileIdFromStrId("tile.stone");
			else if (Input.IsKeyDown(Keys.D3))
				_currTileId = GameData.GetTileIdFromStrId("tile.wood");

			// Select current wall
			if (Input.IsKeyDown(Keys.D4))
				_currWallId = GameData.GetWallIdFromStrId("wall.dirt");
			else if (Input.IsKeyDown(Keys.D5))
				_currWallId = GameData.GetWallIdFromStrId("wall.wood");
		}
	}
}
