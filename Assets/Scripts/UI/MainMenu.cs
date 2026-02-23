using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Oyuna başlama fonksiyonu
    public void PlayGame()
    {
        // Eğer sahnelerinizi "Build Settings" kısmında sıraya koyduysanız bu kod bir sonraki sahneye geçer.
        // Örneğin: Sahne 0 -> Main Menu, Sahne 1 -> Oyun
        // Belirli bir isme sahip sahneyi yüklemek isterseniz: SceneManager.LoadScene("OyunSahneIsmi"); kullanabilirsiniz.
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Seçenekler vs. fonksiyonlarını da buraya ekleyebilirsiniz ileride
    public void OptionsMenu()
    {
        Debug.Log("Ayarlar menüsü açılıyor...");
    }

    // Oyundan çıkış fonksiyonu
    public void QuitGame()
    {
        Debug.Log("Oyundan çıkılıyor!");
        Application.Quit();
    }
}
