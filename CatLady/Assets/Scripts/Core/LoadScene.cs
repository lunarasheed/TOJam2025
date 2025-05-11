using UnityEngine;

public class LoadScene : MonoBehaviour
{
    public string sceneName; // The name of the scene to load
    public void LoadSceneByName()
    {
        // Check if the scene name is not empty
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Load the specified scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is empty or null.");
        }
    }
}
