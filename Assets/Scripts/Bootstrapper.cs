using CoinsOfHope.Audio;
using UnityEngine;

namespace CoinsOfHope
{
    public static class Bootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            EnsureManager<GameManager>("GameManager");
            EnsureManager<AudioManager>("AudioManager");
            EnsureManager<AdManager>("AdManager");
        }

        private static void EnsureManager<T>(string name) where T : Component
        {
            if (Object.FindFirstObjectByType<T>() != null) return;

            var go = new GameObject(name);
            go.AddComponent<T>();
        }
    }
}
