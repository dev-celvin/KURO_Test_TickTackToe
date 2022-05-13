using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPlayer
{

    #region model
    public string playerName;
    public ChessboardGridMarkType usingMarkType;
    #endregion

    public ChessPlayer(string name, ChessboardGridMarkType markType) {
        playerName = name;
        usingMarkType = markType;
    }

    public virtual void OnDuty() { }

}
