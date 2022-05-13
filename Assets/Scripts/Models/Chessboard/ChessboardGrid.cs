using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct ChessboardGridPosition {
    public int row, column;
    public ChessboardGridPosition(int row, int column) { this.row = row; this.column = column; }
}

public class ChessboardGrid : MonoBehaviour
{

    #region model
    ChessboardGridPosition position;
    public bool Marked { get; private set; }
    ChessboardGridMarkType mark;
    #endregion

    #region view
    const string strCross = "X", strCircle = "O";
    TextMeshProUGUI lbMark;
    #endregion

    private void Start()
    {
        lbMark = GetComponentInChildren<TextMeshProUGUI>();

        var btnGrid = GetComponent<Button>();
        btnGrid.onClick.AddListener(OnBtnGridClick);
    }

    void OnBtnGridClick() {
        GameManager gm = GameManager.Instance;
        if (gm.CurrentState != GameState.Running) return;
        if (!gm.IsCurrentTurnPlayerHuman())
        {
            //Debug.Log("Not Your Turn!");
            return;
        }
        gm.MarkChessboardGrid(this);
        //CurMark = ChessboardGridMarkType.Cross;
    }

    public void Init(int row, int column) { position = new ChessboardGridPosition(row, column); }

    public ChessboardGridMarkType CurMark {
        get {
            return mark;
        }
        set {
            switch (value) {
                case ChessboardGridMarkType.Cross:
                    lbMark.text = strCross;
                    break;
                case ChessboardGridMarkType.Circle:
                    lbMark.text = strCircle;
                    break;
            }
            mark = value;
            Marked = true;
        }
    }

    public ChessboardGridPosition GetPosition() {
        return position;
    }

    public void ResetData() {
        Marked = false;
        lbMark.text = string.Empty;
    }

}
