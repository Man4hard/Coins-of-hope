using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CoinsOfHope.UI
{
    public class SplashScreenController : MonoBehaviour
    {
        [SerializeField] private float splashDuration = 2f;

        void Start()
        {
            StartCoroutine(LoadMainMenu());
        }

        IEnumerator LoadMainMenu()
        {
            yield return new WaitForSeconds(splashDuration);
            SceneManager.LoadScene(SceneNames.MainMenu);
        }
    }
}
