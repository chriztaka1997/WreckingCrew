using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeButtonMB : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI title, desc;
    public Image image;
    UI_ManagerMB uiMngr;
    public string upgradeName;

    public void SetUpgrade(UpgradeData upgrade, UI_ManagerMB uiMngr)
    {
        title.text = upgrade.title;
        desc.text = upgrade.desc;
        image.sprite = upgrade.sprite;
        upgradeName = upgrade.name;
        this.uiMngr = uiMngr;
    }

    public void OnPress()
    {
        uiMngr.OnUpgradeSelected(upgradeName);
    }
}
