using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public void ContinueGame()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void ExitGame()
    {
        Application.Quit();
    }
}
