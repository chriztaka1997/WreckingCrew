using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpgradeSelectorMB : MonoBehaviour
{
    public List<UpgradeButtonMB> buttons;
    public UI_ManagerMB uiMngr;

    public const int numUpgrades = 3;

    public void SetButtons(List<UpgradeData> upgrades)
    {
        if (upgrades.Count != numUpgrades)
        {
            throw new Exception("Upgrade name list size issue");
        }

        for (int i = 0; i < numUpgrades; i++)
        {
            buttons[i].SetUpgrade(upgrades[i], uiMngr);
        }
    }
}
