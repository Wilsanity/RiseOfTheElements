using Kibo.NPCs;
using Kibo.NPCs.Data;
using Kibo.Quests;
using Kibo.Quests.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kibo.Data
{
    public class DataManager : SingletonBehaviour<DataManager>
    {
        [field: SerializeField] public GameData GameData { get; private set; }

        public IDataService<GameData> DataService => dataService ??= new FileDataService(new JsonSerializer());

        private IDataService<GameData> dataService;

        #region Unity Messages
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;

            CheckDuplicateGUIDs<NPC, NPCData>();
            CheckDuplicateGUIDs<Quest, QuestData>();

            string[] saves = DataService.Saves.ToArray();
            if (saves.Length == 0) NewGame();
            else LoadGame(saves[0]);

            NPC.Spawned.AddListener(GameData.Load);
            NPC.Destroyed.AddListener(GameData.Save);
            Quest.Spawned.AddListener(GameData.Load);
            Quest.Destroyed.AddListener(GameData.Save);
        }

        protected override void OnDestroy()
        {
            if (Instance != this) return;

            NPC.Spawned.RemoveListener(GameData.Load);
            NPC.Destroyed.RemoveListener(GameData.Save);
            Quest.Spawned.RemoveListener(GameData.Load);
            Quest.Destroyed.RemoveListener(GameData.Save);

            base.OnDestroy();
        }
        #endregion

        #region Global Identity Behaviour
        private void CheckDuplicateGUIDs<GlobalIdentityT, DataT>() 
            where GlobalIdentityT : GlobalIdentityBehaviour<GlobalIdentityT, DataT> 
            where DataT : IGlobalIdentityData
        {
            Dictionary<string, string> guids = new();
            foreach (GlobalIdentityT globalIdentity in Resources.FindObjectsOfTypeAll(typeof(GlobalIdentityT)).Cast<GlobalIdentityT>())
            {
                if (guids.TryAdd(globalIdentity.GUID, globalIdentity.name)) continue;

                Debug.LogError($"Duplicate GUID between {typeof(GlobalIdentityT).Name}s '{guids[globalIdentity.GUID]}' and '{globalIdentity.name}': '{globalIdentity.GUID}'");
            }
        }
        #endregion

        #region File IO
        public void NewGame()
        {
            GameData = new GameData();

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void SaveGame()
        {
            GameData.Save();

            DataService.Save(GameData);
        }

        public void LoadGame(string gameName)
        {
            GameData = DataService.Load(gameName);

            GameData.Load();
        }

        public void DeleteGame(string gameName)
        {
            DataService.Delete(gameName);
        }
        #endregion

        #region Data Access
        public NPCData GetNPCData(string guid) => Get<NPCData>(guid);

        public QuestData GetQuestData(string guid) => Get<QuestData>(guid);

        private DataT Get<DataT>(string guid) where DataT : class, IGlobalIdentityData
        {
            if (typeof(NPCData).IsAssignableFrom(typeof(DataT))) return GameData.NPCDatas.FirstOrDefault(npc => npc.GUID == guid) as DataT;
            else if (typeof(QuestData).IsAssignableFrom(typeof(DataT))) return GameData.QuestDatas.FirstOrDefault(quest => quest.GUID == guid) as DataT;
            else return null;
        }
        #endregion
    }
}