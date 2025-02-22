﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginMainLogic : MonoBehaviour
{
    public float speed = 1;
    public CanvasGroup canvasGroup;
    public GameObject continueGameBtn;

    void Awake()
    {
        var anim = FindObjectsOfType<Animation>();
        foreach (var item in anim) {
            item.SetSpeed(speed);
        }
        if (GameStateManager.curState == GameStateManager.GameState.None) {
            GameStateManager.curState = GameStateManager.GameState.Main;
        } else {
            GameObject camera2 = Camera.main.gameObject;
            var post2 = camera2.AddComponent<JumpLevelGaussianBlur>();
            post2.ShowLevel(() => {
                DestroyImmediate(post2);
                StartCoroutine(ShowMenu());
            }, .25f, 20);
        }
        continueGameBtn.SetActive(SaveLoadData.HasArchives());
    }

    private void Start() {
        StartCoroutine(ShowMenu());
    }

    private IEnumerator ShowMenu() {
        canvasGroup.alpha = 0;
        yield return new WaitForSeconds(8/speed);
        float show = 0;
        float add = 0.01f;
        while (show < 1) {
            show += Time.deltaTime * add;
            add = add + show;
            canvasGroup.alpha = show;
            yield return new WaitForEndOfFrame();
        }
    }

    public void ContinueGame() {
        SaveLoadData.LoadGame();
    }

    public void NewGame() {
        GameStateManager.curState = GameStateManager.GameState.CreateRole;
        GameObject camera = Camera.main.gameObject;
        var post = camera.AddComponent<JumpLevelGaussianBlur>();
        post.JumpLevel(() => {
            SceneManager.LoadScene("CreateRole");
        }, 0.25f, 20);
    }

    public void SettingGame() {

    }

    public void QuitGame() {
        Application.Quit();
    }
}
