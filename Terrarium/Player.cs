using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Terrarium
{
	public class Player
	{
		World _world;
		Camera _cam;

		int _currBlockId = 1;
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
			if (Input.IsMouseButtonHeld(MouseButton.Left) && _world.GetBlock(tilePos.X, tilePos.Y) == -1)
				_world.SetBlock(tilePos.X, tilePos.Y, _currBlockId);
			else if (Input.IsMouseButtonHeld(MouseButton.Right) && _world.GetWall(tilePos.X, tilePos.Y) == -1)
				_world.SetWall(tilePos.X, tilePos.Y, _currWallId);
			else if(Input.IsMouseButtonDown(MouseButton.Middle))
			{
				if (_world.GetBlock(tilePos.X, tilePos.Y) != -1)
					_world.SetBlock(tilePos.X, tilePos.Y, -1);
				else
					_world.SetWall(tilePos.X, tilePos.Y, -1);
			}

			// Select current block
			if (Input.IsKeyDown(Keys.D1))
				_currBlockId = GameData.GetBlockIdFromStrId("block.dirt");
			else if (Input.IsKeyDown(Keys.D2))
				_currBlockId = GameData.GetBlockIdFromStrId("block.stone");
			else if (Input.IsKeyDown(Keys.D3))
				_currBlockId = GameData.GetBlockIdFromStrId("block.wood");

			// Select current wall
			if (Input.IsKeyDown(Keys.D4))
				_currWallId = GameData.GetWallIdFromStrId("wall.dirt");
			else if (Input.IsKeyDown(Keys.D5))
				_currWallId = GameData.GetWallIdFromStrId("wall.wood");
		}
	}
}
