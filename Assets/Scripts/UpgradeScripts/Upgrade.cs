using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Upgrade
{
    public string name;

    public Action<BallEquipMB, float> onBallEnemyCollision;
    public Action<PlayerMB, Enemymovement, float> onDamageTaken;

    public UpgradeData upgradeData;

    public abstract void OnUpgradeGet(PlayerMB player);

    public static Upgrade UpgradeFactory(UpgradeData upgradeData)
    {
        switch (upgradeData.name)
        {
            case "hp_small":
                return new StatUpgrade(upgradeData);
            default:
                throw new Exception("No upgrade found");
        }
    }
}

public class StatUpgrade : Upgrade
{
    public StatUpgrade(UpgradeData upgradeData)
    {
        this.upgradeData = upgradeData;
        if (upgradeData.data.Count == 0)
        {
            throw new Exception("No Stat Data Provided!!");
        }
    }

    public override void OnUpgradeGet(PlayerMB player)
    {
        UpgradeData.Data statData = upgradeData.data[0];
        switch (statData.name)
        {
            case "maxHP":
                player.stats.ChangeHP(statData.amount, 0);
                player.OnMaxHPChange();
                break;
            case "maxHP%":
                player.stats.ChangeHP(0, statData.amount);
                player.OnMaxHPChange();
                break;
            case "attack":
                player.stats.attack.flatInc += statData.amount;
                break;
            case "attack%":
                player.stats.attack.multInc += statData.amount;
                break;
            case "precision":
                player.stats.precision.flatInc += statData.amount;
                break;
            case "precision%":
                player.stats.precision.multInc += statData.amount;
                break;
            case "movSpd":
                player.stats.movSpd.flatInc += statData.amount;
                break;
            case "movSpd%":
                player.stats.movSpd.multInc += statData.amount;
                break;
            case "dodge":
                player.stats.dodge.flatInc += statData.amount;
                break;
            case "dodge%":
                player.stats.dodge.multInc += statData.amount;
                break;
            case "healTick":
                player.stats.healTick.flatInc += statData.amount;
                break;
            case "healTick%":
                player.stats.healTick.multInc += statData.amount;
                break;
            case "tenacity":
                player.stats.tenacity.flatInc += statData.amount;
                break;
            case "tenacity%":
                player.stats.tenacity.multInc += statData.amount;
                break;
            default:
                throw new Exception("No Stat Data Provided!!");
        }
    }
}
