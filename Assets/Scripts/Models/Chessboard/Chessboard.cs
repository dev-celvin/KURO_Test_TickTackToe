using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard : MonoBehaviour
{

    const int Row = 3, Column = 3;

    ChessboardGrid[,] grids = new ChessboardGrid[Row, Column];

    private void Start()
    {
        InitGrids();
    }

    void InitGrids() {
        var hierarchyGrids = GetComponentsInChildren<ChessboardGrid>();
        for (int i = 0; i < hierarchyGrids.Length; i++) {
            var grid = hierarchyGrids[i];
            grid.Init(i / Column, i % Column);
            var gridPos = grid.GetPosition();
            grids[gridPos.row, gridPos.column] = grid;
            //Debug.Log(i + "    " + hierarchyGrids[i].name);
        }
        //foreach (var grid in hierarchyGrids) {
        //    grids[grid.GetPosRow()-1, grid.GetPosColumn()-1] = grid;
        //}
        //Debug.Log(grids.Length);
    }

    public int GetGridsCount() { return Row * Column; }

    public ChessboardGrid GetGrid(int row, int column) {
        return grids[row, column];
    }

    public int GetMaxRow() { return Row; }
    public int GetMaxColumn() { return Column; }

    public void Flush() {
        foreach (var grid in grids) {
            grid.ResetData();
        }
    }

}
