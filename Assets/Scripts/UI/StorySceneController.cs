using CoinsOfHope;
using CoinsOfHope.Audio;
using CoinsOfHope.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CoinsOfHope.UI
{
    public class StorySceneController : MonoBehaviour
    {
        [Header("Story Data")]
        public StoryCharacter[] storyCharacters;

        [Header("UI Elements")]
        public Text titleText;
        public Transform storyListContainer;
        public Button backButton;

        private int _currentStoryIndex = 0;

        void Start()
        {
            LoadStoryCharacters();
            SetupUI();
            DisplayStoryList();
        }

        void LoadStoryCharacters()
        {
            if (storyCharacters == null || storyCharacters.Length == 0)
            {
                storyCharacters = Resources.LoadAll<StoryCharacter>("");
            }

            if (storyCharacters == null || storyCharacters.Length == 0)
            {
                Debug.LogWarning("No story characters found. Creating defaults.");
                CreateDefaultStories();
            }
        }

        void CreateDefaultStories()
        {
            storyCharacters = new StoryCharacter[2];

            var amina = ScriptableObject.CreateInstance<StoryCharacter>();
            amina.storyId = "amina";
            amina.characterName = "Amina's Journey";
            amina.introText = "Meet Amina, a single mother of three who works tirelessly to provide for her family. She needs help with essential items.";
            amina.resolutionText = "Thanks to your generosity, Amina's family now has what they need. You've made a real difference!";
            amina.needs = new System.Collections.Generic.List<CharacterNeed>
            {
                new CharacterNeed { needId = "food", needName = "Food", description = "Basic groceries", cost = 50 },
                new CharacterNeed { needId = "books", needName = "Books", description = "School supplies", cost = 30 },
                new CharacterNeed { needId = "clothes", needName = "Clothes", description = "Warm clothing", cost = 40 }
            };
            storyCharacters[0] = amina;

            var rahim = ScriptableObject.CreateInstance<StoryCharacter>();
            rahim.storyId = "rahim";
            rahim.characterName = "Rahim's Hope";
            rahim.introText = "Rahim is an elderly man who lives alone. He needs assistance with medical care and basic necessities.";
            rahim.resolutionText = "Your kindness has brought hope to Rahim's life. He can now face each day with dignity.";
            rahim.needs = new System.Collections.Generic.List<CharacterNeed>
            {
                new CharacterNeed { needId = "medical", needName = "Medical Help", description = "Medicine and care", cost = 60 },
                new CharacterNeed { needId = "clothes", needName = "Warm Clothes", description = "Winter clothing", cost = 40 },
                new CharacterNeed { needId = "food", needName = "Food", description = "Nutritious meals", cost = 50 }
            };
            storyCharacters[1] = rahim;
        }

        void SetupUI()
        {
            if (backButton != null)
            {
                backButton.onClick.AddListener(OnBackClicked);
            }

            if (titleText != null)
            {
                titleText.text = "Stories";
            }
        }

        void DisplayStoryList()
        {
            if (storyListContainer == null)
            {
                Debug.LogError("Story list container not assigned!");
                return;
            }

            for (int i = 0; i < storyCharacters.Length; i++)
            {
                var story = storyCharacters[i];
                if (story == null) continue;

                int index = i;
                bool story1Completed = IsStoryCompleted(0);
                bool unlocked = GameManager.Instance != null && GameManager.Instance.IsStoryUnlocked(story.storyId, story1Completed);

                var btn = UIFactory.CreateButton(storyListContainer, $"Story{i}Button", story.characterName);
                btn.interactable = unlocked;

                var rt = btn.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, -200 * i);

                if (!unlocked)
                {
                    var img = btn.GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = Color.gray;
                    }
                }

                btn.onClick.AddListener(() => OnStorySelected(index));
            }
        }

        bool IsStoryCompleted(int storyIndex)
        {
            if (storyIndex < 0 || storyIndex >= storyCharacters.Length) return false;
            if (GameManager.Instance == null) return false;

            var story = storyCharacters[storyIndex];
            var storyData = GameManager.Instance.GetOrCreateStory(story.storyId);

            int fulfilledCount = 0;
            foreach (var need in story.needs)
            {
                var needData = storyData.GetOrCreateNeed(need.needId);
                if (needData.fulfilled) fulfilledCount++;
            }

            return fulfilledCount == story.needs.Count;
        }

        void OnStorySelected(int index)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            _currentStoryIndex = index;
            ShowStoryDetail(index);
        }

        void ShowStoryDetail(int index)
        {
            if (index < 0 || index >= storyCharacters.Length) return;

            ClearStoryList();

            var story = storyCharacters[index];
            if (story == null) return;

            var introText = UIFactory.CreateText(storyListContainer, "IntroText", story.introText, 32, TextAnchor.UpperLeft, UITheme.Text);
            var introRt = introText.GetComponent<RectTransform>();
            introRt.anchoredPosition = new Vector2(0, 0);
            introRt.sizeDelta = new Vector2(800, 200);

            float yPos = -250;
            foreach (var need in story.needs)
            {
                CreateNeedUI(story.storyId, need, yPos);
                yPos -= 180;
            }

            yPos -= 50;
            var backToListBtn = UIFactory.CreateButton(storyListContainer, "BackToListButton", "Back to Stories");
            var backRt = backToListBtn.GetComponent<RectTransform>();
            backRt.anchoredPosition = new Vector2(0, yPos);
            backToListBtn.onClick.AddListener(() => RefreshStoryList());
        }

        void CreateNeedUI(string storyId, CharacterNeed need, float yPos)
        {
            var needGo = new GameObject($"Need_{need.needId}");
            needGo.transform.SetParent(storyListContainer, false);
            var needRt = needGo.AddComponent<RectTransform>();
            needRt.anchoredPosition = new Vector2(0, yPos);
            needRt.sizeDelta = new Vector2(800, 150);

            var needData = GameManager.Instance.GetOrCreateNeedProgress(storyId, need.needId);

            var nameText = UIFactory.CreateText(needGo.transform, "Name", $"{need.needName} ({needData.donated}/{need.cost})", 36, TextAnchor.MiddleLeft, UITheme.Text);
            var nameRt = nameText.GetComponent<RectTransform>();
            nameRt.anchoredPosition = new Vector2(0, 40);
            nameRt.sizeDelta = new Vector2(800, 50);

            var descText = UIFactory.CreateText(needGo.transform, "Description", need.description, 28, TextAnchor.MiddleLeft, UITheme.MutedText);
            var descRt = descText.GetComponent<RectTransform>();
            descRt.anchoredPosition = new Vector2(0, -10);
            descRt.sizeDelta = new Vector2(800, 50);

            if (needData.fulfilled)
            {
                var fulfilledText = UIFactory.CreateText(needGo.transform, "Fulfilled", "✓ Fulfilled!", 32, TextAnchor.MiddleLeft, UITheme.Primary);
                var fulfilledRt = fulfilledText.GetComponent<RectTransform>();
                fulfilledRt.anchoredPosition = new Vector2(0, -60);
                fulfilledRt.sizeDelta = new Vector2(800, 50);
            }
            else
            {
                var donateBtn = UIFactory.CreateButton(needGo.transform, "DonateButton", "Donate 10 Coins");
                var donateRt = donateBtn.GetComponent<RectTransform>();
                donateRt.anchoredPosition = new Vector2(0, -60);
                donateRt.sizeDelta = new Vector2(400, 80);
                donateBtn.onClick.AddListener(() => OnDonateClicked(storyId, need));
            }
        }

        void OnDonateClicked(string storyId, CharacterNeed need)
        {
            if (GameManager.Instance == null) return;

            if (GameManager.Instance.TryDonateToNeed(storyId, need.needId, need.cost, 10, out int donated, out bool fulfilled))
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayStoryHelp();
                }

                RefreshStoryList();
                ShowStoryDetail(_currentStoryIndex);
            }
            else
            {
                Debug.Log("Not enough coins or need already fulfilled!");
            }
        }

        void ClearStoryList()
        {
            if (storyListContainer == null) return;

            foreach (Transform child in storyListContainer)
            {
                Destroy(child.gameObject);
            }
        }

        void RefreshStoryList()
        {
            ClearStoryList();
            DisplayStoryList();
        }

        void OnBackClicked()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }

            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}
