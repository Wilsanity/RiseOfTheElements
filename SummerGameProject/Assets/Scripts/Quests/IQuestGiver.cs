using Kibo.Data;

namespace Kibo.Quests
{
    public interface IQuestGiver : IGlobalIdentityData
    {
        public string Name { get; }
    }
}