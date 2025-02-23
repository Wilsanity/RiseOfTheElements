using Kibo.Attributes;
using Kibo.NPCs;
using Kibo.NPCs.Data;
using Kibo.Quests;
using Kibo.Quests.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Kibo.Data
{
    [Serializable]
    public sealed class GameData
    {
        [field: SerializeField] public string Name { get; private set; }
        [SerializeField, ReadOnly] private List<NPCData> npcDatas = new();
        [SerializeField, ReadOnly] private List<QuestData> questDatas = new();

        public IEnumerable<NPCData> NPCDatas => npcDatas;
        public IEnumerable<QuestData> QuestDatas => questDatas;
        public IEnumerable<QuestData> ActiveQuests => questDatas.Where(questData => !questData.IsComplete);
        public IEnumerable<QuestData> CompleteQuests => questDatas.Where(questData => questData.IsComplete);

        public GameData() : this("New Game") { }

        public GameData(string name)
        {
            Name = name;
        }

        public void Save()
        {
            foreach (NPC npc in NPC.GetAll()) Save(npc);
            foreach (Quest quest in Quest.GetAll()) Save(quest);
        }

        public void Load()
        {
            foreach (NPC npc in NPC.GetAll()) Load(npc);
            foreach (Quest quest in Quest.GetAll()) Load(quest);
        }

        #region NPC
        public void Save(NPC npc) => Save(npc, npcDatas);
        public void Load(NPC npc) => Load(npc, npcDatas);
        public void Delete(NPC npc) => Delete(npc, npcDatas);
        #endregion

        #region Quests
        public void Save(Quest quest) => Save(quest, questDatas);
        public void Load(Quest quest) => Load(quest, questDatas);
        public void Delete(Quest quest) => Delete(quest, questDatas);
        #endregion

        #region Generics
        private void Save<GlobalIdentityT, DataT>(GlobalIdentityT globalIdentity, List<DataT> tDatas)
            where GlobalIdentityT : GlobalIdentityBehaviour<GlobalIdentityT, DataT>
            where DataT : class, IGlobalIdentityData, new()
        {
            if (Register(globalIdentity, tDatas, out DataT tData)) return;

            globalIdentity.SaveTo(ref tData);
        }

        private void Load<GlobalIdentityT, DataT>(GlobalIdentityT globalIdentity, List<DataT> tDatas)
            where GlobalIdentityT : GlobalIdentityBehaviour<GlobalIdentityT, DataT>
            where DataT : class, IGlobalIdentityData, new()
        {
            if (Register(globalIdentity, tDatas, out DataT tData)) return;
            
            globalIdentity.LoadFrom(tData);
        }

        private bool Register<GlobalIdentityT, DataT>(GlobalIdentityT globalIdentity, List<DataT> tDatas, out DataT tData)
            where GlobalIdentityT : GlobalIdentityBehaviour<GlobalIdentityT, DataT>
            where DataT : class, IGlobalIdentityData, new()
        {
            tData = tDatas.FirstOrDefault(data => data.GUID == globalIdentity.GUID);
            if (tData != null) return false;

            tData = new();
            globalIdentity.SaveTo(ref tData);
            tDatas.Add(tData);
#if LOG
            Debug.Log($"Registered '{globalIdentity.name}' as a new '{typeof(DataT).Name}' with GUID: '{tData.GUID}'");
#endif

            return true;
        }

        private void Delete<GlobalIdentityT, DataT>(GlobalIdentityT globalIdentity, List<DataT> tDatas)
            where GlobalIdentityT : GlobalIdentityBehaviour<GlobalIdentityT, DataT>
            where DataT : class, IGlobalIdentityData, new()
        {
            DataT tData = tDatas.FirstOrDefault(data => data.GUID == globalIdentity.GUID);
            if (tData == null) return;

            tDatas.Remove(tData);
        } 
        #endregion
    }
}