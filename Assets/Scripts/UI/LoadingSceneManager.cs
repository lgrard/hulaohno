using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] GameObject pressAButton;
    [SerializeField] Image loadingBar;
    private bool startLevel = false;
    private AsyncOperation operation;
    private GlobalScheme _globalScheme;

    float timer = 0f;
    float minLoadTime = 3f;

    private void Update()
    {
        if (timer >= minLoadTime && operation.progress > 0.8)
        {
            pressAButton.SetActive(true);
            _globalScheme = new GlobalScheme();
            _globalScheme.Enable();
            _globalScheme.Global.Join.performed += StartLevel;
        }

        if (startLevel)
        {
            _globalScheme.Disable();
            operation.allowSceneActivation = true;
}
    }

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadLevelAsync(sceneIndex));
    }

    IEnumerator LoadLevelAsync(int sceneIndex)
    {
        yield return new WaitForSecondsRealtime(0.75f);
        loadingScreen.SetActive(true);

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        operation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = operation.progress / 0.9f;
            loadingBar.fillAmount = progress;
            
            yield return null;

            if(timer < minLoadTime)
                timer += Time.unscaledDeltaTime;
        }

        yield return null;
    }

    private void StartLevel(CallbackContext ctx)
    {
        _globalScheme.Global.Join.performed -= StartLevel;
        startLevel = true;
    }
}
