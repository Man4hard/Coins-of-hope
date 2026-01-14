using System;
using UnityEngine;

namespace CoinsOfHope.Data
{
    public static class DailyRewardService
    {
        public const int DailyRewardCoins = 10;

        public static bool TryClaimDailyReward(PlayerData playerData, out int rewardedCoins)
        {
            rewardedCoins = 0;

            var lastClaimStr = PlayerPrefs.GetString(GameConstants.DailyRewardLastClaimUtcTicksKey, "");
            var nowUtc = DateTime.UtcNow;

            DateTime lastClaimUtc;
            if (string.IsNullOrEmpty(lastClaimStr) || !DateTime.TryParse(lastClaimStr, out lastClaimUtc))
            {
                lastClaimUtc = DateTime.MinValue;
            }

            if (lastClaimUtc.Date >= nowUtc.Date) return false;

            rewardedCoins = DailyRewardCoins;
            playerData.coins += rewardedCoins;

            PlayerPrefs.SetString(GameConstants.DailyRewardLastClaimUtcTicksKey, nowUtc.ToString());
            PlayerPrefs.Save();

            return true;
        }
    }
}
