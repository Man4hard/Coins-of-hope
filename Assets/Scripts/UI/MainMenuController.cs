using CoinsOfHope;
using CoinsOfHope.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CoinsOfHope.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Elements")]
        public Text coinDisplay;
        public Button playButton;
        public Button storyButton;
        public Button settingsButton;

        void Start()
        {
            if (GameManager.Instance == null)
            {
                CreateGameManager();
            }

            if (AudioManager.Instance == null)
            {
                CreateAudioManager();
            }

            if (AudioManager.Instance != null && AudioManager.Instance.mainMenuMusic != null)
            {
                AudioManager.Instance.PlayMusic(AudioManager.Instance.mainMenuMusic);
            }

            SetupUI();
            UpdateCoinDisplay();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.CoinsChanged += OnCoinsChanged;
            }
        }

        void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CoinsChanged -= OnCoinsChanged;
            }
        }

        void SetupUI()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayClicked);
            }

            if (storyButton != null)
            {
                storyButton.onClick.AddListener(OnStoryClicked);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsClicked);
            }
        }

        void UpdateCoinDisplay()
        {
            if (coinDisplay != null && GameManager.Instance != null)
            {
                coinDisplay.text = $"Coins: {GameManager.Instance.Coins}";
            }
        }

        void OnCoinsChanged(int newAmount)
        {
            UpdateCoinDisplay();
        }

        void OnPlayClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            SceneManager.LoadScene(SceneNames.CoinRunnerGameplay);
        }

        void OnStoryClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            SceneManager.LoadScene(SceneNames.StoryScene);
        }

        void OnSettingsClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
                AudioManager.Instance.ToggleMute();
            }

            UpdateCoinDisplay();
        }

        void CreateGameManager()
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }

        void CreateAudioManager()
        {
            var go = new GameObject("AudioManager");
            go.AddComponent<AudioManager>();
        }
    }
}
