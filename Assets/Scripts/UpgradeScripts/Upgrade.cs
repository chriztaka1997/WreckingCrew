using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public abstract class Upgrade
{
    public string name;
    public UpgradeData upgradeData;

    public Action<BallEquipMB, float> onBallEnemyCollision;
    public Action<PlayerMB, Enemymovement, float> onDamageTaken;


    public Upgrade(UpgradeData upgradeData)
    {
        this.upgradeData = upgradeData;
        name = upgradeData.name;
    }

    public abstract void OnUpgradeGet(PlayerMB player);

    public static Upgrade UpgradeFactory(UpgradeData upgradeData)
    {
        switch (upgradeData.name)
        {
            case var _ when Regex.IsMatch(upgradeData.name, StatUpgrade.prefix):
                return new StatUpgrade(upgradeData);
            case var _ when Regex.IsMatch(upgradeData.name, BallEQ_Upgrade.prefix):
                return new BallEQ_Upgrade(upgradeData);
            default:
                throw new Exception("No upgrade found");
        }
    }
}

public class StatUpgrade : Upgrade
{
    public const string prefix = @"stat:";

    public StatUpgrade(UpgradeData upgradeData) : base(upgradeData)
    {
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
            case "spinDmg":
                player.stats.spinDmg.flatInc += statData.amount;
                break;
            case "spinDmg%":
                player.stats.spinDmg.multInc += statData.amount;
                break;
            case "throwDmg":
                player.stats.throwDmg.flatInc += statData.amount;
                break;
            case "throwDmg%":
                player.stats.throwDmg.multInc += statData.amount;
                break;
            case "swingDmg":
                player.stats.swingDmg.flatInc += statData.amount;
                break;
            case "swingDmg%":
                player.stats.swingDmg.multInc += statData.amount;
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
            case "maxSpinSpd":
                player.stats.maxSpinSpd.flatInc += statData.amount;
                break;
            case "maxSpinSpd%":
                player.stats.maxSpinSpd.multInc += statData.amount;
                break;
            case "minSpinSpd":
                player.stats.minSpinSpd.flatInc += statData.amount;
                break;
            case "minSpinSpd%":
                player.stats.minSpinSpd.multInc += statData.amount;
                break;
            case "spinSpdRate":
                player.stats.spinSpdRate.flatInc += statData.amount;
                break;
            case "spinSpdRate%":
                player.stats.spinSpdRate.multInc += statData.amount;
                break;
            default:
                throw new Exception("No Stat Data Provided!!");
        }
    }
}

public class BallEQ_Upgrade : Upgrade
{
    public const string prefix = @"balleq:";

    public BallEQ_Upgrade(UpgradeData upgradeData) : base(upgradeData)
    {
        if (upgradeData.data.Count == 0)
        {
            throw new Exception("No Stat Data Provided!!");
        }
    }

    public override void OnUpgradeGet(PlayerMB player)
    {
        GameManagerMB.instance.upgradeMngr.RemoveOtherBallEQ(this);
        UpgradeData.Data statData = upgradeData.data[0];
        player.SetEquipBall(statData.name);
    }
}
