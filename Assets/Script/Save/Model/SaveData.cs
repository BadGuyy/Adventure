using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class SaveData
{
    [JsonProperty]
    internal Dictionary<string, int> _npcDialoguePhase = new();
    [JsonProperty]
    internal float _playerPositionX, _playerPositionY, _playerPositionZ;
    [JsonProperty]
    internal float _playerRotationX, _playerRotationY, _playerRotationZ, _playerRotationW;
}