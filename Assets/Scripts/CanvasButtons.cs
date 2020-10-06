using UnityEngine;
using UnityEngine.SceneManagement;


public class CanvasButtons : MonoBehaviour
{
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadInstagram()
    {
        Application.OpenURL("https://instagram.com/burya_game?igshid=1sjym9bbmnefp");
    }
}
