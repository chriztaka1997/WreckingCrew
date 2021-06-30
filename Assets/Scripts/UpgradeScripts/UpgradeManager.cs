using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class UpgradeManager
{
    public List<Upgrade> upgrades;

    public UpgradeManager()
    {
        if (upgrades == null) upgrades = new List<Upgrade>();
        else upgrades.Clear();
    }

    public void AddUpgrade(string name, PlayerMB player)
    {
        Upgrade upgrade = UpgradePaletteMB.instance.GetUpgrade(name);
        if (upgrade != null)
        {
            upgrades.Add(upgrade);
        }
        else Debug.Log("Upgrade could not be added");

        upgrade.OnUpgradeGet(player);
    }

    public void ResetUpgrades(PlayerMB player)
    {
        foreach (Upgrade u in upgrades)
        {
            u.OnUpgradeGet(player);
        }
    }

    public void ClearUpgrades()
    {
        upgrades.Clear();
    }

    public string UpgradeStringListJson()
    {
        List<string> strings = new List<string>();
        foreach (Upgrade u in upgrades)
        {
            strings.Add(u.name);
        }
        return JsonConvert.SerializeObject(strings);
    }
}
