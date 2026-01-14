using CoinsOfHope;
using CoinsOfHope.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CoinsOfHope.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        public Text resultText;
        public Text coinsEarnedText;
        public Button retryButton;
        public Button menuButton;

        private int _coinsEarned;

        public void Show(int coinsEarned)
        {
            _coinsEarned = coinsEarned;

            if (resultText != null)
            {
                resultText.text = "Game Over!";
            }

            if (coinsEarnedText != null)
            {
                coinsEarnedText.text = $"Coins Earned: {_coinsEarned}";
            }

            if (retryButton != null)
            {
                retryButton.onClick.AddListener(OnRetryClicked);
            }

            if (menuButton != null)
            {
                menuButton.onClick.AddListener(OnMenuClicked);
            }

            gameObject.SetActive(true);
        }

        void OnRetryClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            SceneManager.LoadScene(SceneNames.CoinRunnerGameplay);
        }

        void OnMenuClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}
