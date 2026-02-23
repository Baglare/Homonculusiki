using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI; // Unity Inspector üzerinden bu objeyi sürükleyip bırakacağız.

    void Update()
    {
        // ESC veya P tuşuna basıldığında duraklat/devam et oyunu.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Menüyü gizle
        Time.timeScale = 1f; // Zamanı normale döndür (Oyun akar)
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); // Menüyü göster
        Time.timeScale = 0f; // Zamanı durdur (Oyunu dondur)
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // Ana menüye dönerken zamanı tekrar 1'e ayarlamayı unutmayın
        Debug.Log("Ana Menü yükleniyor...");
        // Ana menünün Build Settings'deki index'ini vermelisiniz (Örn: 0) veya adını: SceneManager.LoadScene("MainMenu");
        SceneManager.LoadScene(0); 
    }

    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor!");
        Application.Quit();
    }
}
