using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour {

	public enum screens
    {
        mainGame,
        emails,
        shop,
        spellGame,
        settings,
        transactions
    }
    public screens screenToOpen;
}
