using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReportCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI className, kills, damage;
    TrackedPlayerStats trackedPlayerStats;

    public void Init(Player player) {
        trackedPlayerStats = player.TrackedPlayerStats;

        className.text = player.PlayerClass.ClassInfo.name;
        kills.text = $"Kills - {trackedPlayerStats.RetrieveStat("Kills")}";
        damage.text = $"Damage Dealt - {trackedPlayerStats.RetrieveStat("Damage Dealt")}";
    }
}
