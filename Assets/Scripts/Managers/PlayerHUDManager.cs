using System.Collections;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class PlayerHUDManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _timer;
        [SerializeField] private TextMeshProUGUI _countdown;

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

        public void SetTime(float time)
        {
            _timer.text = $"{time:N2}";
        }
        
        public void SetScore(int score)
        {
            _score.text = $"{score}";
        }
    }
}