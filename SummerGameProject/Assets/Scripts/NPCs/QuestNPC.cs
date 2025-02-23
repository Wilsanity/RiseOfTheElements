using Kibo.Quests;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kibo.NPCs
{
    public class QuestNPC : NPC, IQuestGiver
    {
        private List<Quest> quests = new();

        public string Name => name;

        protected override void Awake()
        {
            base.Awake();

            CheckQuests();
            Quest.Spawned.AddListener(CheckQuest);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Quest.Spawned.RemoveListener(CheckQuest);
        }

        private void CheckQuests()
        {
            foreach (Quest quest in Quest.GetAll()) CheckQuest(quest);
        }

        private void CheckQuest(Quest quest)
        {
            if (quest == null || GUID != quest.NPCGUID) return;

            quests.Add(quest);
        }

        public override bool Interact()
        {
            Quest quest = quests.FirstOrDefault(q => !q.HasStarted);
            if (quest == null)
            {
                Debug.Log($"No quests left"); // TODO: Integrate with upcoming dialog system
                return true;
            }

            quest.StartQuest();
            Debug.Log($"Started new quest '{quest.name}'");

            return true;
        }
    }
}