using UnityEngine;

namespace CoinsOfHope.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;

        [Header("Music Clips")]
        public AudioClip mainMenuMusic;
        public AudioClip gameplayMusic;

        [Header("SFX Clips")]
        public AudioClip coinCollectSfx;
        public AudioClip obstacleHitSfx;
        public AudioClip storyHelpSfx;
        public AudioClip buttonClickSfx;

        private bool isMuted;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                LoadMuteState();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = 0.5f;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = 0.7f;
            }
        }

        void LoadMuteState()
        {
            isMuted = PlayerPrefs.GetInt("AudioMuted", 0) == 1;
            UpdateMuteState();
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource.clip == clip && musicSource.isPlaying) return;

            musicSource.Stop();
            musicSource.clip = clip;
            if (!isMuted)
            {
                musicSource.Play();
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || isMuted) return;
            sfxSource.PlayOneShot(clip);
        }

        public void PlayCoinCollect()
        {
            PlaySFX(coinCollectSfx);
        }

        public void PlayObstacleHit()
        {
            PlaySFX(obstacleHitSfx);
        }

        public void PlayStoryHelp()
        {
            PlaySFX(storyHelpSfx);
        }

        public void PlayButtonClick()
        {
            PlaySFX(buttonClickSfx);
        }

        public void ToggleMute()
        {
            isMuted = !isMuted;
            PlayerPrefs.SetInt("AudioMuted", isMuted ? 1 : 0);
            PlayerPrefs.Save();
            UpdateMuteState();
        }

        public bool IsMuted()
        {
            return isMuted;
        }

        void UpdateMuteState()
        {
            if (isMuted)
            {
                musicSource.Pause();
                sfxSource.volume = 0;
            }
            else
            {
                if (musicSource.clip != null && !musicSource.isPlaying)
                {
                    musicSource.Play();
                }
                sfxSource.volume = 0.7f;
            }
        }
    }
}
