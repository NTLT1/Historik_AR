using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Animator menuAnimator; 
    [SerializeField] private Button toggleButton; 
    [SerializeField] private GameObject[] uiElementsToHide; 
    private bool isMenuVisible;

    void Start()
    {
        
        foreach (GameObject uiElement in uiElementsToHide)
        {
            uiElement.SetActive(false);
        }
        if (menuAnimator == null)
        {
            Debug.LogError("Animator is missing!");
            return;
        }
    }

    public void ToggleMenu()
    {
        if (menuAnimator == null)
        {
            Debug.LogError("Animator is missing!");
            return;
        }

        isMenuVisible = !isMenuVisible;
        menuAnimator.SetBool("IsVisible", isMenuVisible);

        if (isMenuVisible)
        {
            foreach (GameObject uiElement in uiElementsToHide)
            {
                uiElement.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject uiElement in uiElementsToHide)
            {
                uiElement.SetActive(false);
            }
        }
    }
}
