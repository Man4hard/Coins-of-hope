using CoinsOfHope.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CoinsOfHope
{
    public class SceneBuilder : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoaded()
        {
            var sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            switch (sceneName)
            {
                case SceneNames.SplashScreen:
                    BuildSplashScreen();
                    break;
                case SceneNames.MainMenu:
                    BuildMainMenu();
                    break;
                case SceneNames.CoinRunnerGameplay:
                    BuildCoinRunnerGameplay();
                    break;
                case SceneNames.StoryScene:
                    BuildStoryScene();
                    break;
            }
        }

        private static void BuildSplashScreen()
        {
            var canvas = UIFactory.CreateRootCanvas("Canvas");

            var bgPanel = UIFactory.CreatePanel(canvas.transform, "Background", UITheme.Background);

            var titleText = UIFactory.CreateText(canvas.transform, "TitleText", "Coins of Hope", 80, TextAnchor.MiddleCenter, UITheme.Primary);
            var titleRt = titleText.GetComponent<RectTransform>();
            UIFactory.SetAnchored(titleRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(900, 200));

            var controller = canvas.gameObject.AddComponent<SplashScreenController>();
        }

        private static void BuildMainMenu()
        {
            var canvas = UIFactory.CreateRootCanvas("Canvas");

            var bgPanel = UIFactory.CreatePanel(canvas.transform, "Background", UITheme.Background);

            var titleText = UIFactory.CreateText(canvas.transform, "TitleText", "Coins of Hope", 80, TextAnchor.MiddleCenter, UITheme.Primary);
            var titleRt = titleText.GetComponent<RectTransform>();
            UIFactory.SetAnchored(titleRt, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -200), new Vector2(900, 150));

            var coinDisplay = UIFactory.CreateText(canvas.transform, "CoinDisplay", "Coins: 0", 48, TextAnchor.MiddleCenter, UITheme.Text);
            var coinRt = coinDisplay.GetComponent<RectTransform>();
            UIFactory.SetAnchored(coinRt, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -400), new Vector2(700, 100));

            var playButton = UIFactory.CreateButton(canvas.transform, "PlayButton", "Play Coin Runner");
            var playRt = playButton.GetComponent<RectTransform>();
            UIFactory.SetAnchored(playRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(700, 140));

            var storyButton = UIFactory.CreateButton(canvas.transform, "StoryButton", "View Stories");
            var storyRt = storyButton.GetComponent<RectTransform>();
            UIFactory.SetAnchored(storyRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -80), new Vector2(700, 140));

            var settingsButton = UIFactory.CreateButton(canvas.transform, "SettingsButton", "Toggle Mute");
            var settingsRt = settingsButton.GetComponent<RectTransform>();
            UIFactory.SetAnchored(settingsRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -260), new Vector2(700, 140));

            var levelTitle = UIFactory.CreateText(canvas.transform, "LevelTitle", "Select Level", 44, TextAnchor.MiddleCenter, UITheme.Text);
            var levelTitleRt = levelTitle.GetComponent<RectTransform>();
            UIFactory.SetAnchored(levelTitleRt, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 420), new Vector2(700, 80));

            var selectedLevelText = UIFactory.CreateText(canvas.transform, "SelectedLevelText", "Selected: 1", 40, TextAnchor.MiddleCenter, UITheme.MutedText);
            var selectedLevelRt = selectedLevelText.GetComponent<RectTransform>();
            UIFactory.SetAnchored(selectedLevelRt, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 350), new Vector2(700, 80));

            int selectedLevel = GameManager.Instance != null ? GameManager.Instance.SelectedLevel : 1;
            selectedLevelText.text = $"Selected: {selectedLevel}";

            var levelButtons = new UnityEngine.UI.Button[5];
            for (int i = 1; i <= 5; i++)
            {
                int level = i;
                var btn = UIFactory.CreateButton(canvas.transform, $"Level{level}Btn", level.ToString());
                levelButtons[i - 1] = btn;

                var rt = btn.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(160, 110);

                float startX = -340f;
                float spacing = 170f;
                UIFactory.SetAnchored(rt, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(startX + (i - 1) * spacing, 240), new Vector2(160, 110));

                bool unlocked = GameManager.Instance != null && GameManager.Instance.IsLevelUnlocked(level);
                btn.interactable = unlocked;

                var img = btn.GetComponent<UnityEngine.UI.Image>();
                img.color = unlocked ? UITheme.Secondary : UnityEngine.Color.gray;

                btn.onClick.AddListener(() =>
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.SetSelectedLevel(level);
                        selectedLevelText.text = $"Selected: {level}";

                        for (int j = 0; j < levelButtons.Length; j++)
                        {
                            var b = levelButtons[j];
                            if (b == null) continue;

                            var bi = b.GetComponent<UnityEngine.UI.Image>();
                            if (!b.interactable)
                            {
                                bi.color = UnityEngine.Color.gray;
                            }
                            else
                            {
                                bi.color = (j + 1) == level ? UITheme.Primary : UITheme.Secondary;
                            }
                        }
                    }
                });

                if (img != null && unlocked && level == selectedLevel)
                {
                    img.color = UITheme.Primary;
                }
            }

            var controller = canvas.gameObject.AddComponent<MainMenuController>();
            controller.coinDisplay = coinDisplay;
            controller.playButton = playButton;
            controller.storyButton = storyButton;
            controller.settingsButton = settingsButton;
        }

        private static void BuildCoinRunnerGameplay()
        {
            CreateCamera();
            CreateGround();
            var player = CreatePlayer();
            CreateGameManager(player);
            CreateGameplayUI();
        }

        private static void CreateCamera()
        {
            var cameraGo = new GameObject("Main Camera");
            var cam = cameraGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = UITheme.Background;
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cameraGo.tag = "MainCamera";
            cameraGo.transform.position = new Vector3(0, 0, -10);
        }

        private static void CreateGround()
        {
            var groundGo = new GameObject("Ground");
            groundGo.layer = LayerMask.NameToLayer("Default");
            groundGo.tag = "Ground";
            groundGo.transform.position = new Vector3(0, -3f, 0);

            var spriteRenderer = groundGo.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(0.5f, 0.3f, 0.2f);
            spriteRenderer.sprite = CreateSquareSprite();
            groundGo.transform.localScale = new Vector3(100f, 1f, 1f);

            var boxCollider = groundGo.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1f, 1f);
        }

        private static Gameplay.PlayerController CreatePlayer()
        {
            var playerGo = new GameObject("Player");
            playerGo.tag = "Player";
            playerGo.transform.position = new Vector3(-3f, 0f, 0f);

            var spriteRenderer = playerGo.AddComponent<SpriteRenderer>();
            spriteRenderer.color = UITheme.Primary;
            spriteRenderer.sprite = CreateSquareSprite();
            playerGo.transform.localScale = new Vector3(0.5f, 1f, 1f);

            var rb = playerGo.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;

            var boxCollider = playerGo.AddComponent<BoxCollider2D>();
            boxCollider.size = new Vector2(1f, 1f);

            var groundCheckGo = new GameObject("GroundCheck");
            groundCheckGo.transform.SetParent(playerGo.transform);
            groundCheckGo.transform.localPosition = new Vector3(0, -0.5f, 0);

            var playerController = playerGo.AddComponent<Gameplay.PlayerController>();

            return playerController;
        }

        private static void CreateGameManager(Gameplay.PlayerController player)
        {
            var managerGo = new GameObject("CoinRunnerManager");
            var manager = managerGo.AddComponent<Gameplay.CoinRunnerManager>();
            manager.player = player;
        }

        private static void CreateGameplayUI()
        {
            var canvas = UIFactory.CreateRootCanvas("GameplayCanvas");

            var coinCounter = UIFactory.CreateText(canvas.transform, "CoinCounter", "Coins: 0", 48, TextAnchor.MiddleLeft, UITheme.Text);
            var coinRt = coinCounter.GetComponent<RectTransform>();
            UIFactory.SetAnchored(coinRt, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(50, -50), new Vector2(500, 80));

            var levelIndicator = UIFactory.CreateText(canvas.transform, "LevelIndicator", "Level 1", 48, TextAnchor.MiddleRight, UITheme.Text);
            var levelRt = levelIndicator.GetComponent<RectTransform>();
            UIFactory.SetAnchored(levelRt, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-50, -50), new Vector2(500, 80));
        }

        private static void BuildStoryScene()
        {
            var canvas = UIFactory.CreateRootCanvas("Canvas");

            var bgPanel = UIFactory.CreatePanel(canvas.transform, "Background", UITheme.Background);

            var titleText = UIFactory.CreateText(canvas.transform, "TitleText", "Stories", 72, TextAnchor.MiddleCenter, UITheme.Primary);
            var titleRt = titleText.GetComponent<RectTransform>();
            UIFactory.SetAnchored(titleRt, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -150), new Vector2(800, 120));

            var scrollViewGo = new GameObject("ScrollView");
            scrollViewGo.transform.SetParent(canvas.transform, false);
            var scrollRect = scrollViewGo.AddComponent<ScrollRect>();
            var scrollRt = scrollViewGo.GetComponent<RectTransform>();
            UIFactory.SetAnchored(scrollRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -100), new Vector2(800, 1200));

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(scrollViewGo.transform, false);
            var contentRt = contentGo.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0.5f, 1f);
            contentRt.anchorMax = new Vector2(0.5f, 1f);
            contentRt.pivot = new Vector2(0.5f, 1f);
            contentRt.sizeDelta = new Vector2(800, 1500);

            scrollRect.content = contentRt;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            var backButton = UIFactory.CreateButton(canvas.transform, "BackButton", "Back to Menu");
            var backRt = backButton.GetComponent<RectTransform>();
            UIFactory.SetAnchored(backRt, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 100), new Vector2(500, 120));

            var controller = canvas.gameObject.AddComponent<StorySceneController>();
            controller.titleText = titleText;
            controller.storyListContainer = contentRt;
            controller.backButton = backButton;
        }

        private static Sprite CreateSquareSprite()
        {
            var texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
