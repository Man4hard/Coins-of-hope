using System;
using UnityEngine;

namespace CoinsOfHope
{
    public class AdManager : MonoBehaviour
    {
        public static AdManager Instance { get; private set; }

        public event Action<bool> RewardedAdCompleted;
        public event Action InterstitialAdClosed;

        [Header("Ad Settings")]
        [SerializeField] private bool useTestAds = true;
        
        private const string TestRewardedAdUnitId = "ca-app-pub-3940256099942544/5224354917";
        private const string TestInterstitialAdUnitId = "ca-app-pub-3940256099942544/1033173712";

        private bool _isInitialized = false;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAds();
        }

        void InitializeAds()
        {
#if GOOGLE_MOBILE_ADS
            Debug.Log("Google Mobile Ads SDK detected. Initializing...");
            
            MobileAds.Initialize(initStatus =>
            {
                _isInitialized = true;
                Debug.Log("Google Mobile Ads initialized.");
            });
#else
            Debug.LogWarning("Google Mobile Ads SDK not found. Using stub implementation.");
            _isInitialized = true;
#endif
        }

        public void ShowRewardedAd(Action<bool> onComplete)
        {
#if GOOGLE_MOBILE_ADS
            if (!_isInitialized)
            {
                Debug.LogWarning("Ad system not initialized yet.");
                onComplete?.Invoke(false);
                return;
            }

            LoadRewardedAd((success) =>
            {
                if (success)
                {
                    ShowLoadedRewardedAd(onComplete);
                }
                else
                {
                    onComplete?.Invoke(false);
                }
            });
#else
            Debug.Log("Stub: Showing rewarded ad (simulated success).");
            onComplete?.Invoke(true);
            RewardedAdCompleted?.Invoke(true);
#endif
        }

        public void ShowInterstitialAd()
        {
#if GOOGLE_MOBILE_ADS
            if (!_isInitialized)
            {
                Debug.LogWarning("Ad system not initialized yet.");
                return;
            }

            LoadInterstitialAd((success) =>
            {
                if (success)
                {
                    ShowLoadedInterstitialAd();
                }
            });
#else
            Debug.Log("Stub: Showing interstitial ad (simulated).");
            InterstitialAdClosed?.Invoke();
#endif
        }

#if GOOGLE_MOBILE_ADS
        private void LoadRewardedAd(Action<bool> onLoaded)
        {
            var adRequest = new AdRequest.Builder().Build();
            RewardedAd.Load(TestRewardedAdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"Failed to load rewarded ad: {error}");
                    onLoaded?.Invoke(false);
                    return;
                }

                Debug.Log("Rewarded ad loaded successfully.");
                onLoaded?.Invoke(true);
            });
        }

        private void ShowLoadedRewardedAd(Action<bool> onComplete)
        {
            // Actual implementation would show the ad via the RewardedAd instance
            // For now this is a placeholder
            Debug.Log("Showing rewarded ad...");
            onComplete?.Invoke(true);
            RewardedAdCompleted?.Invoke(true);
        }

        private void LoadInterstitialAd(Action<bool> onLoaded)
        {
            var adRequest = new AdRequest.Builder().Build();
            InterstitialAd.Load(TestInterstitialAdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError($"Failed to load interstitial ad: {error}");
                    onLoaded?.Invoke(false);
                    return;
                }

                Debug.Log("Interstitial ad loaded successfully.");
                onLoaded?.Invoke(true);
            });
        }

        private void ShowLoadedInterstitialAd()
        {
            // Actual implementation would show the ad via the InterstitialAd instance
            Debug.Log("Showing interstitial ad...");
            InterstitialAdClosed?.Invoke();
        }
#endif
    }
}
