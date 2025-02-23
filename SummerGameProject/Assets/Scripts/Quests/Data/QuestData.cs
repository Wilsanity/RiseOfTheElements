using Kibo.Data;
using System;
using System.Linq;
using UnityEngine;

namespace Kibo.Quests.Data
{
    [Serializable]
    public sealed class QuestData : IGlobalIdentityData
    {
        [field: SerializeField] public string GUID { get; set; }
        [field: SerializeField] public string NPCGUID { get; set; }
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public Objective[] Objectives { get; set; }

        public bool IsComplete => Objectives.All(objectiveData => objectiveData.IsComplete);
    }
}