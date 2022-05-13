using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{

    const string tipGameRunning = "��Ϸ�����У�����հ׸�������", tipResultHasWinner = "��Ϸ������{0}��ʤ!", tipResultDraw = "��Ϸ������ƽ��!";

    TextMeshProUGUI lbTip;

    void Awake()
    {

        lbTip = transform.Find("Tip").GetComponent<TextMeshProUGUI>();

        transform.Find("BtnExit").GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("Main");
        });

        transform.Find("BtnRestart").GetComponent<Button>().onClick.AddListener(() => {
            GameManager.Instance.Restart();
        });
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnGameStateChange += OnGameStateChange;
    }

    private void OnDisable()
    {
        if(GameManager.Instance != null) GameManager.Instance.OnGameStateChange -= OnGameStateChange;
    }

    void OnGameStateChange(GameState gameState) {
        switch (gameState) {
            case GameState.Running:
                lbTip.text = tipGameRunning;
                break;
            case GameState.End:
                string winnerName = GameManager.Instance.GetWinnerName();
                if (string.IsNullOrEmpty(winnerName))
                {
                    lbTip.text = tipResultDraw;
                }
                else {
                    lbTip.text = string.Format(tipResultHasWinner, winnerName);
                }
                break;
        }
    }

}
