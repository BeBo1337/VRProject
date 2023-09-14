using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class PlayerHUDManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private TextMeshProUGUI _countdown;
        [SerializeField] private Image _screenOverlay;

        public static PlayerHUDManager Instance { get; private set; }
        public float AnimationDuration => 4;

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                _countdown.gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public IEnumerator PlayCountdownAnimation()
        {
            _countdown.gameObject.SetActive(true);
            _countdown.text = "3";
            yield return new WaitForSeconds(1);
            _countdown.text = "2";
            yield return new WaitForSeconds(1);
            _countdown.text = "1";
            yield return new WaitForSeconds(1);
            _countdown.text = "GO!!!";
            yield return new WaitForSeconds(1);
            _countdown.gameObject.SetActive(false);
        }

        public void GameOver()
        {
            SetInactive();
            StartCoroutine(GameOverAnimation());
        }

        private IEnumerator GameOverAnimation()
        {
            float duration = 2.0f; // Adjust the duration as needed
            float elapsedTime = 0.0f;
            Color startColor = Color.clear; // Start with a transparent color
            Color targetColor = Color.red; // Change this to the desired end color

            while (elapsedTime < duration)
            {
                _screenOverlay.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Add your bloody animation here, e.g., particle effects or sprite animations

            // You can then reset the screen overlay color to transparent if needed
        }

        public void SetTime(float time)
        {
            _timer.text = $"{time:N2}";
        }
        
        public void SetScore(int score)
        {
            _score.text = $"{score}";
        }

        public void SetInactive()
        {
            _score.gameObject.SetActive(false);
            _timer.gameObject.SetActive(false);
            _countdown.gameObject.SetActive(false);
        }

        public void SetActive()
        {
            _score.gameObject.SetActive(true);
            _timer.gameObject.SetActive(true);
            _countdown.gameObject.SetActive(true);
        }
    }
}