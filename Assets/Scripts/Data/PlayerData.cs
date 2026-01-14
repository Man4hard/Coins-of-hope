using System;
using System.Collections.Generic;

namespace CoinsOfHope.Data
{
    [Serializable]
    public class PlayerData
    {
        public int coins = 0;
        public int highestUnlockedLevel = 1;
        public int lastRunCoinsEarned = 0;
        public bool lastRunDoubleClaimed = false;

        public List<int> completedLevels = new List<int>();
        public List<StoryProgressData> stories = new List<StoryProgressData>();

        public bool IsLevelCompleted(int level)
        {
            return completedLevels.Contains(level);
        }

        public void MarkLevelCompleted(int level)
        {
            if (!completedLevels.Contains(level)) completedLevels.Add(level);
        }
    }

    [Serializable]
    public class StoryProgressData
    {
        public string storyId;
        public List<NeedProgressData> needs = new List<NeedProgressData>();

        public NeedProgressData GetOrCreateNeed(string needId)
        {
            var existing = needs.Find(n => n.needId == needId);
            if (existing != null) return existing;

            var created = new NeedProgressData { needId = needId };
            needs.Add(created);
            return created;
        }
    }

    [Serializable]
    public class NeedProgressData
    {
        public string needId;
        public int donated = 0;
        public bool fulfilled = false;
    }
}
