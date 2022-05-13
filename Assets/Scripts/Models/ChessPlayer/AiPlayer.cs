using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiPlayer : ChessPlayer
{

    #region ctrl
    ChessboardGridPosition decidingMarkPos;
    #endregion

    public AiPlayer(string name, ChessboardGridMarkType markType) : base(name, markType) { }

    bool CheckGridHorizontalDefending(ChessboardGridPosition checkGridPos, ChessboardGridMarkType checkGridMarkType) {
        bool multiMarked = false, mark = false; ;  
        GameManager visibleData = GameManager.Instance;
        for (int i = checkGridPos.column - 1; i >= 0; i--)
        {
            if (visibleData.CheckGridMarked(checkGridPos.row, i))
            {
                if (visibleData.GetGridMark(checkGridPos.row, i) == checkGridMarkType)
                {
                    multiMarked = true;
                    if (mark) return true;
                }
            }
            else {
                decidingMarkPos.row = checkGridPos.row;
                decidingMarkPos.column = i;
                mark = true;
                if (multiMarked) return true;
            }
        }
        for (int i = checkGridPos.column + 1; i < visibleData.GetChessboardMaxColumn(); i++)
        {
            if (visibleData.CheckGridMarked(checkGridPos.row, i))
            {
                if (visibleData.GetGridMark(checkGridPos.row, i) == checkGridMarkType)
                {
                    multiMarked = true;
                    if (mark) return true;
                }
            }
            else
            {
                decidingMarkPos.row = checkGridPos.row;
                decidingMarkPos.column = i;
                mark = true;
                if (multiMarked) return true;
            }
        }
        return false;
    }
    bool CheckGridVerticalDefending(ChessboardGridPosition checkGridPos, ChessboardGridMarkType checkGridMarkType)
    {
        bool multiMarked = false, mark = false;
        GameManager visibleData = GameManager.Instance;
        for (int i = checkGridPos.row - 1; i >= 0; i--)
        {
            if (visibleData.CheckGridMarked(i, checkGridPos.column))
            {
                if (visibleData.GetGridMark(i, checkGridPos.column) == checkGridMarkType)
                {
                    multiMarked = true;
                    if (mark) return true;
                }
            }
            else
            {
                decidingMarkPos.row = i;
                decidingMarkPos.column = checkGridPos.column;
                mark = true;
                if (multiMarked) return true;
            }
        }
        for (int i = checkGridPos.row + 1; i < visibleData.GetChessboardMaxRow(); i++)
        {
            if (visibleData.CheckGridMarked(i, checkGridPos.column))
            {
                if (visibleData.GetGridMark(i, checkGridPos.column) == checkGridMarkType)
                {
                    multiMarked = true;
                    if (mark) return true;
                }
            }
            else
            {
                decidingMarkPos.row = i;
                decidingMarkPos.column = checkGridPos.column;
                mark = true;
                if (multiMarked) return true;
            }
        }
        return false;
    }
    bool CheckGridPossiveDiagonalDefending(ChessboardGridPosition checkGridPos, ChessboardGridMarkType checkGridMarkType) {
        bool multiMarked = false, mark = false;
        GameManager visibleData = GameManager.Instance;
        for (int i = checkGridPos.row - 1, j = checkGridPos.column + 1; i >= 0 && j < visibleData.GetChessboardMaxColumn(); i--, j++)
        {
            if (visibleData.CheckGridMarked(i, j))
            {
                if (visibleData.GetGridMark(i, j) == checkGridMarkType)
                {
                    multiMarked = true;
                    if (mark) return true;
                }
            }
            else
            {
                decidingMarkPos.row = i;
                decidingMarkPos.column = j;
                mark = true;
                if (multiMarked) return true;
            }
        }
        for (int i = checkGridPos.row + 1, j = checkGridPos.column - 1; i < visibleData.GetChessboardMaxRow() && j >= 0; i++, j--)
        {
            if (visibleData.CheckGridMarked(i, j))
            {
                if (visibleData.GetGridMark(i, j) == checkGridMarkType)
                    multiMarked = true;
                    if (mark) return true;
            }
            else
            {
                decidingMarkPos.row = i;
                decidingMarkPos.column = j;
                mark = true;
                if (multiMarked) return true;
            }
        }

        return false;
    }
    bool CheckGridNegativeDiagonalDefending(ChessboardGridPosition checkGridPos, ChessboardGridMarkType checkGridMarkType)
    {
        bool multiMarked = false, mark = false;
        GameManager visibleData = GameManager.Instance;
        for (int i = checkGridPos.row - 1, j = checkGridPos.column - 1; i >= 0 && j >= 0; i--, j--)
        {
            if (visibleData.CheckGridMarked(i, j))
            {
                if (visibleData.GetGridMark(i, j) == checkGridMarkType)
                {
                    multiMarked = true;
                    if (mark) return true;
                }
            }
            else
            {
                decidingMarkPos.row = i;
                decidingMarkPos.column = j;
                mark = true;
                if (multiMarked) return true;
            }
        }
        for (int i = checkGridPos.row + 1, j = checkGridPos.column + 1; i < visibleData.GetChessboardMaxRow() && j < visibleData.GetChessboardMaxColumn(); i++, j++)
        {
            if (visibleData.CheckGridMarked(i, j))
            {
                if (visibleData.GetGridMark(i, j) == checkGridMarkType)
                    multiMarked = true;
                if (mark) return true;
            }
            else
            {
                decidingMarkPos.row = i;
                decidingMarkPos.column = j;
                mark = true;
                if (multiMarked) return true;
            }
        }

        return false;
    }

    void FindMarkableGrid() {
        GameManager visibleData = GameManager.Instance;
        decidingMarkPos.row = visibleData.GetChessboardMaxRow() / 2;
        decidingMarkPos.column = visibleData.GetChessboardMaxColumn() / 2;
        if (!visibleData.CheckGridMarked(decidingMarkPos.row, decidingMarkPos.column)) return;
        for (int iRow = 0; iRow < visibleData.GetChessboardMaxRow(); iRow++)
        {
            for (int iCol = 0; iCol < visibleData.GetChessboardMaxColumn(); iCol++)
            {
                if (!visibleData.CheckGridMarked(iRow, iCol))
                {
                    decidingMarkPos.row = iRow;
                    decidingMarkPos.column = iCol;
                    return;
                }
            }
        }
    }

    void ThinkAndMarkGrid() {
        GameManager visibleData = GameManager.Instance;
        ChessboardGridPosition lastPlayerStep = visibleData.GetLastPlayerStep();
        ChessboardGridMarkType markType = visibleData.GetGridMark(lastPlayerStep.row, lastPlayerStep.column);

        if (!(CheckGridHorizontalDefending(lastPlayerStep, markType)
            || CheckGridVerticalDefending(lastPlayerStep, markType)
            || CheckGridPossiveDiagonalDefending(lastPlayerStep, markType)
            || CheckGridNegativeDiagonalDefending(lastPlayerStep, markType)))
        {
            FindMarkableGrid();
        }
        GameManager.Instance.MarkChessboardGrid(decidingMarkPos);
    }

    public override void OnDuty()
    {
        ThinkAndMarkGrid();
    }
}
