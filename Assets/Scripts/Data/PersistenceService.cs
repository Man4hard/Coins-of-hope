using UnityEngine;

namespace CoinsOfHope.Data
{
    public static class PersistenceService
    {
        public static PlayerData LoadPlayerData()
        {
            var json = PlayerPrefs.GetString(GameConstants.PlayerDataKey, string.Empty);
            if (string.IsNullOrWhiteSpace(json)) return new PlayerData();

            try
            {
                var data = JsonUtility.FromJson<PlayerData>(json);
                return data ?? new PlayerData();
            }
            catch
            {
                return new PlayerData();
            }
        }

        public static void SavePlayerData(PlayerData data)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(GameConstants.PlayerDataKey, json);
            PlayerPrefs.Save();
        }
    }
}
