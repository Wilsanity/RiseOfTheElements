using Kibo.Data;
using Kibo.NPCs;
using Kibo.NPCs.Data;
using Kibo.Quests.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kibo.Quests
{
    public class SpeakQuest : Quest
    {
        [SerializeField] private string[] npcGUIDS;

        private readonly Dictionary<string, (NPC, UnityAction)> listeners = new();

        public override void StartQuest()
        {
            if (HasStarted) return;

            Objectives = new Objective[npcGUIDS.Length];
            for (int i = 0; i < npcGUIDS.Length; i++)
            {
                NPCData npcData = DataManager.Instance.GetNPCData(npcGUIDS[i]); // TODO: Find a way to ensure NPCs across all scenes are registered by this point
                if (npcData == null) Debug.LogError($"No {nameof(NPCData)} registered for GUID '{npcGUIDS[i]}'");

                Objectives[i] = new Objective
                {
                    Description = $"Speak to {npcData.Name}",
                    Total = 1
                };
            }
            AddListeners();

            base.StartQuest();
        }

        #region Speaking
        private void AddListeners()
        {
            for (int i = 0; i < npcGUIDS.Length; i++)
            {
                NPC npc = NPC.Get(npcGUIDS[i]);
                Listen(npc);
            }

            NPC.Spawned.AddListener(Listen);
        }

        private void Listen(NPC npc)
        {
            if (npc == null) return;

            int index = Array.IndexOf(npcGUIDS, npc.GUID);
            if (index == -1) return;

            Ignore(npc.GUID);

            UnityAction speakAction = () => SpeakTo(index);
            npc.WasInteractedWith.AddListener(speakAction);
            listeners[npc.GUID] = (npc, speakAction);
        }

        private void RemoveListeners()
        {
            for (int i = 0; i < npcGUIDS.Length; i++)
            {
                NPC npc = NPC.Get(npcGUIDS[i]);
                Ignore(npc.GUID);
            }

            NPC.Spawned.RemoveListener(Listen);
        }

        private void Ignore(string npcGUID)
        {
            if (!listeners.TryGetValue(npcGUID, out (NPC, UnityAction) npcAction)) return;

            listeners.Remove(npcGUID);

            (NPC npc, UnityAction speakAction) = npcAction;
            if (npc == null) return;

            npc.WasInteractedWith.RemoveListener(speakAction);
        }

        private void SpeakTo(int npcIndex)
        {
            Objectives[npcIndex].Progress = 1;

            CompleteQuest();
        }
        #endregion

        protected override void CompleteQuest()
        {
            if (!IsComplete) return;

            RemoveListeners();

            base.CompleteQuest();
        }

        public override void LoadFrom(QuestData questData)
        {
            base.LoadFrom(questData);

            if (HasStarted && !IsComplete) AddListeners();
        }
    }
}