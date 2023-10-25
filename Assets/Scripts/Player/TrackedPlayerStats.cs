using System.Collections.Generic;

public class TrackedPlayerStats {
    public Dictionary<string, int> TrackedStats = new();
    public Dictionary<string, int> TotalTrackedStats = new();

    public void IncrementStat(string stat, int value) {
        if (TrackedStats.ContainsKey(stat)) {
            TrackedStats[stat] += value;
        }
        else {
            TrackedStats.Add(stat, value);
        }
    }

    public int RetrieveStat(string statName) {
        if (TrackedStats.TryGetValue(statName, out int value))
            return value;
        else return 0;
    }

    public void CommitLevelPlayerStats() {
        foreach (KeyValuePair<string, int> keyValuePair in TrackedStats) {
            if (TrackedStats.ContainsKey(keyValuePair.Key)) {
                TotalTrackedStats[keyValuePair.Key] += keyValuePair.Value;
            }
            else {
                TotalTrackedStats.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        TrackedStats = new();
    }
}
