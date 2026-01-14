using System;
using CoinsOfHope.Data;
using UnityEngine;

namespace CoinsOfHope
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public event Action<int> CoinsChanged;
        public event Action PlayerDataChanged;

        public int Coins => _data.coins;
        public int HighestUnlockedLevel => _data.highestUnlockedLevel;

        public int SelectedLevel { get; private set; } = 1;

        public int LastRunCoinsEarned => _data.lastRunCoinsEarned;
        public bool LastRunDoubleClaimed => _data.lastRunDoubleClaimed;

        private PlayerData _data;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;

            _data = PersistenceService.LoadPlayerData();

            if (_data.highestUnlockedLevel < 1) _data.highestUnlockedLevel = 1;
            if (_data.highestUnlockedLevel > GameConstants.MaxLevels) _data.highestUnlockedLevel = GameConstants.MaxLevels;

            if (DailyRewardService.TryClaimDailyReward(_data, out var rewarded))
            {
                Save();
                CoinsChanged?.Invoke(_data.coins);
                Debug.Log($"Daily reward claimed: {rewarded} coins");
            }
        }

        public void Save()
        {
            PersistenceService.SavePlayerData(_data);
            PlayerDataChanged?.Invoke();
        }

        public void SetSelectedLevel(int level)
        {
            SelectedLevel = Mathf.Clamp(level, 1, GameConstants.MaxLevels);
        }

        public void AddCoins(int amount)
        {
            if (amount <= 0) return;
            _data.coins += amount;
            Save();
            CoinsChanged?.Invoke(_data.coins);
        }

        public bool SpendCoins(int amount)
        {
            if (amount <= 0) return true;
            if (_data.coins < amount) return false;

            _data.coins -= amount;
            Save();
            CoinsChanged?.Invoke(_data.coins);
            return true;
        }

        public bool IsLevelUnlocked(int level)
        {
            return level <= _data.highestUnlockedLevel;
        }

        public void CompleteLevel(int levelCompleted, int coinsEarned)
        {
            levelCompleted = Mathf.Clamp(levelCompleted, 1, GameConstants.MaxLevels);

            _data.MarkLevelCompleted(levelCompleted);

            var newHighest = Mathf.Max(_data.highestUnlockedLevel, Mathf.Min(GameConstants.MaxLevels, levelCompleted + 1));
            _data.highestUnlockedLevel = newHighest;

            _data.lastRunCoinsEarned = Mathf.Max(0, coinsEarned);
            _data.lastRunDoubleClaimed = false;

            if (coinsEarned > 0) _data.coins += coinsEarned;

            Save();
            CoinsChanged?.Invoke(_data.coins);
        }

        public bool CanDoubleLastRunCoins()
        {
            return _data.lastRunCoinsEarned > 0 && !_data.lastRunDoubleClaimed;
        }

        public bool TryClaimDoubleLastRunCoins()
        {
            if (!CanDoubleLastRunCoins()) return false;

            _data.coins += _data.lastRunCoinsEarned;
            _data.lastRunDoubleClaimed = true;

            Save();
            CoinsChanged?.Invoke(_data.coins);
            return true;
        }

        public StoryProgressData GetOrCreateStory(string storyId)
        {
            if (_data.stories == null) _data.stories = new System.Collections.Generic.List<StoryProgressData>();

            var existing = _data.stories.Find(s => s.storyId == storyId);
            if (existing != null) return existing;

            var created = new StoryProgressData { storyId = storyId };
            _data.stories.Add(created);
            return created;
        }

        public NeedProgressData GetOrCreateNeedProgress(string storyId, string needId)
        {
            return GetOrCreateStory(storyId).GetOrCreateNeed(needId);
        }

        public bool TryDonateToNeed(string storyId, string needId, int needCost, int donationStep, out int donatedNow, out bool fulfilledNow)
        {
            donatedNow = 0;
            fulfilledNow = false;

            if (needCost <= 0) return false;
            donationStep = Mathf.Max(1, donationStep);

            var need = GetOrCreateNeedProgress(storyId, needId);
            if (need.fulfilled) return false;

            var remaining = Mathf.Max(0, needCost - need.donated);
            if (remaining == 0)
            {
                need.fulfilled = true;
                Save();
                return false;
            }

            var canDonate = Mathf.Min(donationStep, remaining, _data.coins);
            if (canDonate <= 0) return false;

            _data.coins -= canDonate;
            need.donated += canDonate;
            if (need.donated >= needCost) need.fulfilled = true;

            donatedNow = canDonate;
            fulfilledNow = need.fulfilled;

            Save();
            CoinsChanged?.Invoke(_data.coins);
            return true;
        }

        public bool IsStoryUnlocked(string storyId, bool story1Completed)
        {
            if (string.IsNullOrEmpty(storyId)) return false;

            // Story 1 unlock: complete Level 5
            if (storyId == "amina") return _data.IsLevelCompleted(GameConstants.MaxLevels);

            // Story 2 unlock: after Story 1 completed (or Level 5 complete as a fallback)
            if (storyId == "rahim") return story1Completed || _data.IsLevelCompleted(GameConstants.MaxLevels);

            return false;
        }
    }
}
