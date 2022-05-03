using UnityEngine;

namespace BLD
{
    public class DataStore_Player
    {
        // NOTE: set when character is teleported (BLDCharacterController - Teleport)
        public readonly BaseVariable<Vector3> lastTeleportPosition = new BaseVariable<Vector3>(Vector3.zero);
        public readonly BaseDictionary<string, Player> otherPlayers = new BaseDictionary<string, Player>();
        public readonly BaseVariable<Player> ownPlayer = new BaseVariable<Player>();
    }
}