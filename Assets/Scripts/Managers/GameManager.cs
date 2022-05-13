using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Running,
    End,
}

public class GameManager : Singleton<GameManager>
{

    #region model
    Chessboard chessboard;
    #endregion

    #region ctrl
    public event Action<GameState> OnGameStateChange;

    GameState gameState;
    List<ChessPlayer> chessPlayers = new List<ChessPlayer>();
    int curTurnPlayerIndex = 0, markedGridCount = 0;
    ChessboardGridPosition lastPlayerStep;
    int winnerPlayerIndex = -1;
    #endregion

    private void Start()
    {
        chessboard = FindObjectOfType<Chessboard>();
        InitChessPlayers();
        CurrentState = GameState.Running;
    }

    void InitChessPlayers() {
        chessPlayers.Clear();
        curTurnPlayerIndex = 0;
        markedGridCount = 0;
        winnerPlayerIndex = -1;
        //chessPlayers.Add(new HumanPlayer("Player1", ChessboardGridMarkType.Cross));
        //chessPlayers.Add(new HumanPlayer("Player2", ChessboardGridMarkType.Circle));
        chessPlayers.Add(new HumanPlayer("Íæ¼Ò", ChessboardGridMarkType.Cross));
        chessPlayers.Add(new AiPlayer("AI", ChessboardGridMarkType.Circle));
    }

    bool CheckGameStateRunning()
    {
        if (CurrentState != GameState.Running)
        {
            Debug.Log("The Game Is Not Running, Current Game State Is " + CurrentState.ToString());
            return false;
        }
        return true;
    }

    void SetTurnToNextPlayer()
    {
        if (++curTurnPlayerIndex == chessPlayers.Count) curTurnPlayerIndex = 0;
        chessPlayers[curTurnPlayerIndex].OnDuty();
    }

    bool CheckGridHorizontalContinuous(ChessboardGrid checkGrid) {
        ChessboardGridPosition checkGridPos = checkGrid.GetPosition();
        for (int i = checkGridPos.column - 1; i >= 0; i--)
        {
            var iGrid = chessboard.GetGrid(checkGridPos.row, i);
            if (!iGrid.Marked || iGrid.CurMark != checkGrid.CurMark)
            {
                return false;
            }
        }
        for (int i = checkGridPos.column + 1; i < chessboard.GetMaxColumn(); i++)
        {
            var iGrid = chessboard.GetGrid(checkGridPos.row, i);
            if (!iGrid.Marked || iGrid.CurMark != checkGrid.CurMark)
            {
                return false;
            }
        }
        return true;
    }
    bool CheckGridVerticalContinuous(ChessboardGrid checkGrid)
    {
        ChessboardGridPosition checkGridPos = checkGrid.GetPosition();
        for (int i = checkGridPos.row - 1; i >= 0; i--)
        {
            var iGrid = chessboard.GetGrid(i, checkGridPos.column);
            if (!iGrid.Marked || iGrid.CurMark != checkGrid.CurMark)
            {
                return false;
            }
        }
        for (int i = checkGridPos.row + 1; i < chessboard.GetMaxRow(); i++)
        {
            var iGrid = chessboard.GetGrid(i, checkGridPos.column);
            if (!iGrid.Marked || iGrid.CurMark != checkGrid.CurMark)
            {
                return false;
            }
        }
        return true;
    }
    bool CheckGridPossiveDiagonalContinuous(ChessboardGrid checkGrid)
    {
        int maxDiagonalCount = Mathf.Max(chessboard.GetMaxRow(), chessboard.GetMaxColumn());
        int continunousCount = 1;
        ChessboardGridPosition checkGridPos = checkGrid.GetPosition();
        for (int i = checkGridPos.row - 1, j = checkGridPos.column + 1; i >= 0 && j < chessboard.GetMaxColumn(); i--, j++)
        {
            var iGrid = chessboard.GetGrid(i, j);
            if (!iGrid.Marked || iGrid.CurMark != checkGrid.CurMark)
            {
                return false;
            }
            continunousCount++;
        }
        for (int i = checkGridPos.row + 1, j = checkGridPos.column - 1; i < chessboard.GetMaxRow() && j >= 0; i++, j--)
        {
            var iGrid = chessboard.GetGrid(i, j);
            if (!iGrid.Marked || iGrid.CurMark != checkGrid.CurMark)
            {
                return false;
            }
            continunousCount++;
        }
        return continunousCount == maxDiagonalCount;
    }
    bool CheckGridNegativeDiagonalContinuous(ChessboardGrid checkGrid)
    {
        int maxDiagonalCount = Mathf.Max(chessboard.GetMaxRow(), chessboard.GetMaxColumn());
        int continunousCount = 1;
        ChessboardGridPosition checkGridPos = checkGrid.GetPosition();
        for (int i = checkGridPos.row - 1, j = checkGridPos.column - 1; i >= 0 && j >= 0; i--, j--)
        {
            var iGrid = chessboard.GetGrid(i, j);
            if (!iGrid.Marked || iGrid.CurMark != checkGrid.CurMark)
            {
                return false;
            }
            continunousCount++;
        }
        for (int i = checkGridPos.row + 1, j = checkGridPos.column + 1; i < chessboard.GetMaxRow() && j < chessboard.GetMaxColumn(); i++, j++)
        {
            var iGrid = chessboard.GetGrid(i, j);
            if (!iGrid.Marked || iGrid.CurMark != checkGrid.CurMark)
            {
                return false;
            }
            continunousCount++;
        }
        return continunousCount == maxDiagonalCount;
    }

    bool CheckGameResult(ChessboardGrid checkGrid) {
        if (
            CheckGridHorizontalContinuous(checkGrid)
            ||
            CheckGridVerticalContinuous(checkGrid)
            ||
            CheckGridPossiveDiagonalContinuous(checkGrid)
            ||
            CheckGridNegativeDiagonalContinuous(checkGrid)
            ) {
            winnerPlayerIndex = curTurnPlayerIndex;
            CurrentState = GameState.End;
            return true;
        }
        if (markedGridCount == chessboard.GetGridsCount()) {
            CurrentState = GameState.End;
            return true;
        }
        return false;
    }

    ChessPlayer GetCurrentTurnPlayer()
    {
        return chessPlayers[curTurnPlayerIndex];
    }

    #region visibleData
    public bool CheckGridMarked(int gridPosRow, int gridPosCol) { return chessboard.GetGrid(gridPosRow, gridPosCol).Marked; }
    public ChessboardGridMarkType GetGridMark(int gridPosRow, int gridPosCol) { return chessboard.GetGrid(gridPosRow, gridPosCol).CurMark; }
    public ChessboardGridPosition GetLastPlayerStep() { return lastPlayerStep; }
    public int GetChessboardMaxRow() { return chessboard.GetMaxRow(); }
    public int GetChessboardMaxColumn() { return chessboard.GetMaxColumn(); }
    #endregion

    public GameState CurrentState
    {
        get {
            return gameState;
        }
        private set {
            gameState = value;
            OnGameStateChange?.Invoke(gameState);
        }
    }

    public string GetWinnerName() {
        return winnerPlayerIndex == -1 ? string.Empty : chessPlayers[winnerPlayerIndex].playerName;
    }

    public bool IsCurrentTurnPlayerHuman() {
        return GetCurrentTurnPlayer() is HumanPlayer;
    }

    public void MarkChessboardGrid(ChessboardGrid grid)
    {
        if (!CheckGameStateRunning()) return;
        if (grid.Marked)
        {
            //Debug.Log("Current Grid Has Been Marked!");
        }
        else
        {
            grid.CurMark = GetCurrentTurnPlayer().usingMarkType;
            markedGridCount++;
            lastPlayerStep = grid.GetPosition();
            //Debug.Log(grid.GetPosRow() + "," + grid.GetPosColumn());
            if (!CheckGameResult(grid)) SetTurnToNextPlayer();
        }
    }
    public void MarkChessboardGrid(ChessboardGridPosition gridPos) {
        MarkChessboardGrid(chessboard.GetGrid(gridPos.row, gridPos.column));
    }

    public void Restart() {
        curTurnPlayerIndex = 0;
        markedGridCount = 0;
        winnerPlayerIndex = -1;
        chessboard.Flush();
        CurrentState = GameState.Running;
    }

}
