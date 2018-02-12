using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Utility
{
    public class LevelManager : MonoBehaviour
    {
        public Canvas canvas;
        public float next_level_wait = 5.0f;

        public void Start()
        {

        }

        public void LoadNextLevelWithFade(string name = "")
        {
            var corutine = LoadNextLevel(name);
            StartCoroutine(corutine);
        }

        public IEnumerator LoadNextLevel(string name = "")
        {

            canvas.GetComponentInChildren<Animator>().SetTrigger("EndScene");
            yield return new WaitForSecondsRealtime(next_level_wait);
            
            if (name == "")
                SceneManager.LoadScene(Application.loadedLevel + 1);
            else
                SceneManager.LoadScene(name);
            Time.timeScale = 1;

        }

        public void LoadLevel(string name)
        {
            Debug.Log("New Level load: " + name);
            SceneManager.LoadScene(name);
        }

        public void QuitRequest()
        {
            Debug.Log("Quit requested");
            Application.Quit();
        }

    }
}