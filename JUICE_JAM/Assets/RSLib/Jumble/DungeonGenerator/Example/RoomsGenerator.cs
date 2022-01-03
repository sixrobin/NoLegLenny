namespace RSLib.Jumble.DungeonGenerator
{
    using UnityEngine;

    /// <summary>
    /// Object that actually instantiates the dungeon in the game.
    /// Should probably have methods to get the right world coordinates, and some event to notify the generation has been done.
    /// </summary>
    public class RoomsGenerator : MonoBehaviour
    {
        public event System.Action<MapDatas> MapGenerated;

        private MapDatas _datas;

        [SerializeField] private int _w = 2;
        [SerializeField] private int _h = 7;
        [SerializeField] private int _seed = 0;
        [SerializeField] private RoomsFactory _roomsFactory = null;
        [SerializeField] private GameObject _firstRoomMark = null;
        [SerializeField] private GameObject _lastRoomMark = null;

        private Vector3 RoomWorldCoordinates(int x, int y)
        {
            return new Vector3(x * 17, -y * 9);
        }

        private void GenerateMap()
        {
            MapDatasGenerator generator = new MapDatasGenerator(_seed, _w, _h);
            _datas = generator.ComputeMapDatas();
            _seed = generator.Seed;
            SpawnRooms();
            MapGenerated?.Invoke(_datas);
        }

        private void SpawnRooms()
        {
            for (int x = 0; x < _datas.Size.W; ++x)
            {
                for (int y = 0; y < _datas.Size.H; ++y)
                {
                    if (_datas.Rooms[x, y].RoomType == RoomType.NA)
                    {
                        Debug.LogError("MapGeneratorGO ERROR: Trying to instantiate a room of type RoomType.NA!");
                        continue;
                    }

                    Instantiate(_roomsFactory.GetRandomRoomByType(_datas.Rooms[x, y].RoomType), RoomWorldCoordinates(x, y), Quaternion.identity);

                    if (x == _datas.Start.X && y == _datas.Start.Y)
                        Instantiate(_firstRoomMark, RoomWorldCoordinates(x, y), _firstRoomMark.transform.rotation);
                    else if (x == _datas.End.X && y == _datas.End.Y)
                        Instantiate(_lastRoomMark, RoomWorldCoordinates(x, y), _lastRoomMark.transform.rotation);
                }
            }
        }

        private void Start()
        {
            GenerateMap();
        }
    }
}