using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartView : MonoBehaviour
{

    void Awake()
    {
        transform.Find("BtnStart").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Game");
        });
        transform.Find("BtnExit").GetComponent<Button>().onClick.AddListener(() => {
            Application.Quit();
        });
    }

}
