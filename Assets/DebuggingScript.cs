using UnityEngine;
using UnityEngine.SceneManagement;

public class DebuggingScript : MonoBehaviour
{

    public void ResetRun(){
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }   
}
