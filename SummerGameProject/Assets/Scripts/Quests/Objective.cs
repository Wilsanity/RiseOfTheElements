using System;
using UnityEngine;

namespace Kibo.Quests
{
    [Serializable]
    public sealed class Objective
    {
        [field: SerializeField] public string Description { get; set; }
        [field: SerializeField] public int Progress { get; set; }
        [field: SerializeField] public int Total { get; set; }

        public bool IsComplete => Progress >= Total;

        public override string ToString()
        {
            return string.Format(Description, Progress, Total);
        }
    }
}