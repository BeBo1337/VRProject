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

        [SerializeField] private GameObject _result;
        [SerializeField] private TextMeshProUGUI _toptext;
        [SerializeField] private TextMeshProUGUI _finalscore;
        [SerializeField] private TextMeshProUGUI _finaltime;
        [SerializeField] private TextMeshProUGUI _countdownmenu;
        [SerializeField] private Image _screenOverlay;

        public static PlayerHUDManager Instance { get; private set; }
        public float AnimationDuration => 4;

        protected void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                _countdown.gameObject.SetActive(false);
            }
            else
            {
                Destroy(gameObject);
            }
            
            _result.SetActive(false);
        }

        public IEnumerator PlayCountdownAnimation()
        {
            _countdown.text = "3";
            _countdown.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            _countdown.text = "2";
            yield return new WaitForSeconds(1);
            _countdown.text = "1";
            yield return new WaitForSeconds(1);
            _countdown.text = "GO!!!";
            yield return new WaitForSeconds(1);
            _countdown.gameObject.SetActive(false);
        }

        public void GameOver(int finalScore, float finalTime)
        {
            SetInactive();
            StartCoroutine(GameOverHandler(finalScore, finalTime));
        }

        // Handles the "Lose" fade to red if lost, UI texts for the end screen.
        private IEnumerator GameOverHandler(int finalScore, float finalTime)
        {
            int n = 5;
            float duration = 2.0f; 
            float elapsedTime = 0.0f;
            Color startColor = Color.clear;
            Color targetColor = Color.red;

            if (finalScore < 30)
            {
                while (elapsedTime < duration)
                {
                    _screenOverlay.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                _toptext.text = "You Won!";
                _toptext.color = Color.yellow;
                n = 8;
            }

            LeaderboardManager.Instance.GameOver(finalScore);
            _finalscore.text = "You killed " + finalScore + " Zombies";
            _finaltime.text = "You took " + finalTime + " Seconds";
            _result.SetActive(true);
            StartCoroutine(PlayCountdownToMenu(n));
        }
        
        public IEnumerator PlayCountdownToMenu(int n)
        {
            _countdownmenu.text = "Returning to menu in " + n + "...";
            _countdownmenu.gameObject.SetActive(true);
            for (int i = n-1; i >= 0; i--)
            {
                yield return new WaitForSeconds(1);
                _countdownmenu.text = "Returning to menu in " + i + "...";
            }
        }
      
        public IEnumerator NextLevelAnimation()
        {
            _countdown.text = "Level " + GameManager._level + " complete";
            _countdown.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            _countdown.gameObject.SetActive(false);
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
        
    }
}