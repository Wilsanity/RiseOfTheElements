using Kibo.Data;
using System;
using UnityEngine;

namespace Kibo.NPCs.Data
{
    [Serializable]
    public class NPCData : IGlobalIdentityData
    {
        [field: SerializeField] public string GUID { get; set; }
        [field: SerializeField] public string Name { get; set; }
    }
}