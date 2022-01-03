namespace RSLib.Jumble.DungeonGenerator
{
    /// <summary>
    /// Class containing basic informations about a room.
    /// Can be inherited to add specific datas.
    /// </summary>
    public class Room
    {
        public Room(RoomType roomType)
        {
            RoomType = roomType;
        }

        public RoomType RoomType { get; private set; }
    }
}