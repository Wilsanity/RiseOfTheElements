using Kibo.Attributes;
using Kibo.Data;
using Kibo.Quests.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Kibo.Quests
{
    public abstract class Quest : GlobalIdentityBehaviour<Quest, QuestData>
    {
        public static UnityEvent<Quest> QuestStarted { get; private set; } = new();
        public static UnityEvent<Quest> QuestCompleted { get; private set; } = new();

        [field: SerializeField] public string NPCGUID { get; private set; }
        [field: SerializeField, ReadOnly] public Objective[] Objectives { get; protected set; }

        public bool HasStarted => Objectives != null && Objectives.Length > 0;
        public bool IsComplete => Objectives.All(objective => objective.IsComplete);

        public virtual void StartQuest()
        {
            if (HasStarted) return;

            QuestStarted.Invoke(this);
        }

        protected virtual void CompleteQuest()
        {
            if (!IsComplete) return;

            QuestCompleted.Invoke(this);

            Debug.Log("Quest Complete");
        }

        #region Save/Load Overrides
        public override void SaveTo(ref QuestData questData)
        {
            questData.GUID = GUID;
            questData.NPCGUID = NPCGUID;
            questData.Name = name;
            questData.Objectives = Objectives;
        }

        public override void LoadFrom(QuestData questData)
        {
            NPCGUID = questData.NPCGUID;
            name = questData.Name;
            Objectives = questData.Objectives.ToArray();
        } 
        #endregion
    }
}