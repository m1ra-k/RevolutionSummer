using UnityEngine;

public class StartScreenGameplay : MonoBehaviour
{
    private GameObject buttons;
    private GameObject credits;

    void Awake()
    {
        buttons = GameObject.Find("Canvas").transform.Find("Buttons").gameObject;

        credits = GameObject.Find("Canvas").transform.Find("Credits").gameObject;
        credits.SetActive(false);
    }

    public void ToggleCredits()
    {
        credits.SetActive(!credits.activeSelf);
        buttons.SetActive(!credits.activeSelf);
    }

    public void ExitGame()
    {
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
