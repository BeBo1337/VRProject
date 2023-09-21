using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class MenuObjectController : MonoBehaviour
{
    // The material to use when this object is inactive (not being gazed at).
    public Material InactiveMaterial;
    // The material to use when this object is active (gazed at).
    public Material GazedAtMaterial;

    private Renderer _myRenderer;
    [SerializeField] private TMP_Text HiddenText;
    private const string gameScene = "GameScene";
    private const string playGameText = "Play Game";
    private const string exitText = "Exit";
    public void Start()
    {
        _myRenderer = GetComponent<Renderer>();
        SetMaterial(false);
    }
    public void OnPointerEnter()
    {
        SetMaterial(true);
    }
    
    public void OnPointerExit()
    {
        SetMaterial(false);
    }
    
    public void OnPointerClick()
    {
        // Get the child canvas that has a child TMP text
        Canvas childCanvas = GetComponentInChildren<Canvas>();
        if (childCanvas != null)
        {
            TMP_Text childTmpText = childCanvas.GetComponentInChildren<TMP_Text>();
            if (childTmpText != null)
            {
                // If the child TMP text has text, then load the game scene
                if (childTmpText.text == playGameText)
                {
                    SceneManager.LoadScene(gameScene);
                }
                else if (childTmpText.text == exitText)
                {
                    // Otherwise, exit the game
                    Application.Quit();
                }
                else
                {
                    HiddenText.enabled = true;
                }
            }
        }
    }
    
    private void SetMaterial(bool gazedAt)
    {
        if (InactiveMaterial != null && GazedAtMaterial != null)
        {
            _myRenderer.material = gazedAt ? GazedAtMaterial : InactiveMaterial;
        }
    }
}
