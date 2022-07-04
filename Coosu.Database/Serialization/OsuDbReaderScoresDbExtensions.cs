using System.Collections.Generic;
using Coosu.Database.DataTypes;
using Coosu.Database.Internal;

namespace Coosu.Database.Serialization;

public static class OsuDbReaderScoresDbExtensions
{
    private static readonly ObjectStructure ScoreStructure;

    static OsuDbReaderScoresDbExtensions()
    {
        var mappingHelper = StructureHelperPool.GetHelperByType(StructureHelperPool.TypeScoresDb);
        var rootStructure = (ObjectStructure)mappingHelper.RootStructure;
        var parentStructure = ((ArrayStructure)rootStructure.Structures[2]).ObjectStructure!;
        var arrayStructure = (ArrayStructure)parentStructure.Structures[2];
        var scoreStructure = ScoreStructure = arrayStructure.ObjectStructure!;
    }

    public static IEnumerable<ScoreBeatmap> EnumerateScoreBeatmaps(this OsuDbReader reader)
    {
        ScoreBeatmap? beatmap = default;
        //int index = 0;

        while (!reader.IsEndOfStream && reader.Read())
        {
            //Console.WriteLine("Name = " + GetString(reader.Name) + "; " +
            //                  "Path = " + GetString(reader.Path) + "; " +
            //                  "Value = " + GetString(reader.Value) + "; " +
            //                  "NodeType = " + GetString(reader.NodeType) + "; " +
            //                  "DataType = " + GetString(reader.DataType) + "; " +
            //                  "TargetType = " + GetString(reader.TargetType));

            if (reader.NodeType == NodeType.ObjectStart)
            {
                beatmap = new ScoreBeatmap();
                FillScoreBeatmap(beatmap, reader);
                //Console.WriteLine("Beatmap index: " + index);
                continue;
            }

            if (reader.NodeType == NodeType.ObjectEnd && beatmap != null)
            {
                yield return beatmap;
                //index++;
                beatmap = default;
            }

            if (reader.NodeType == NodeType.ArrayEnd && reader.NodeId == 7)
            {
                yield break;
            }

            if (beatmap == default)
            {
                continue;
            }

            if (reader.NodeType is not (NodeType.ArrayStart or NodeType.KeyValue))
            {
                continue;
            }
        }
    }

    private static void FillScoreBeatmap(ScoreBeatmap scoreBeatmap, OsuDbReader reader)
    {
        int itemIndex = -1;
        int scoreCount = 0;

        while (!reader.IsEndOfStream && reader.Read())
        {
            var name = reader.Name;
            var nodeType = reader.NodeType;

            if (nodeType is NodeType.ArrayStart or NodeType.KeyValue)
            {
                itemIndex++;
            }
            else
            {
                continue;
            }

            if (itemIndex == 0) scoreBeatmap.Hash = reader.GetString();
            else if (itemIndex == 1) scoreCount = reader.GetInt32();
            else if (itemIndex == 2)
            {
                scoreBeatmap.Scores.Capacity = scoreCount;
                scoreBeatmap.Scores.AddRange(EnumerateScores(reader));
                return;
            }
        }
    }

    private static IEnumerable<Score> EnumerateScores(OsuDbReader reader)
    {
        Score? score = default;
        //int index = 0;
        int arrayCount = 0;

        while (!reader.IsEndOfStream && reader.Read())
        {
            //Console.WriteLine("Name = " + GetString(reader.Name) + "; " +
            //                  "Path = " + GetString(reader.Path) + "; " +
            //                  "Value = " + GetString(reader.Value) + "; " +
            //                  "NodeType = " + GetString(reader.NodeType) + "; " +
            //                  "DataType = " + GetString(reader.DataType) + "; " +
            //                  "TargetType = " + GetString(reader.TargetType));

            if (reader.NodeType == NodeType.ObjectStart)
            {
                score = new Score();
                //Console.WriteLine("Beatmap index: " + index);
                continue;
            }

            if (reader.NodeType == NodeType.ObjectEnd && score != null)
            {
                yield return score;
                //index++;
                score = default;
            }

            if (reader.NodeType == NodeType.ArrayEnd && reader.NodeId == 7)
            {
                yield break;
            }

            if (score == default)
            {
                continue;
            }

            if (reader.NodeType is not (NodeType.ArrayStart or NodeType.KeyValue))
            {
                continue;
            }

            FillProperty(reader, score, ref arrayCount);
        }
    }

    private static void FillProperty(OsuDbReader reader, Score beatmap, ref int arrayCount)
    {
        var nodeId = reader.NodeId;
        if (nodeId == 9) beatmap.GameMode = (DbGameMode)reader.GetByte();
        else if (nodeId == 10) beatmap.ScoreVersion = reader.GetInt32();
        else if (nodeId == 11) beatmap.BeatmapHash = reader.GetString();
        else if (nodeId == 12) beatmap.Player = reader.GetString();
        else if (nodeId == 13) beatmap.ReplayHash = reader.GetString();
        else if (nodeId == 14) beatmap.Count300 = reader.GetInt16();
        else if (nodeId == 15) beatmap.Count100 = reader.GetInt16();
        else if (nodeId == 16) beatmap.Count50 = reader.GetInt16();
        else if (nodeId == 17) beatmap.CountGeki = reader.GetInt16();
        else if (nodeId == 18) beatmap.CountKatu = reader.GetInt16();
        else if (nodeId == 19) beatmap.CountMiss = reader.GetInt16();
        else if (nodeId == 20) beatmap.ReplayScore = reader.GetInt32();
        else if (nodeId == 21) beatmap.MaxCombo = reader.GetInt16();
        else if (nodeId == 22) beatmap.IsPerfect = reader.GetBoolean();
        else if (nodeId == 23) beatmap.Mods = (Mods)reader.GetInt32();
        else if (nodeId == 24) beatmap.LifeBarGraph = reader.GetString();
        else if (nodeId == 25) beatmap.Timestamp = reader.GetDateTime();
        else if (nodeId == 26) beatmap.ReplayLength = reader.GetInt32();
        else if (nodeId == 27) beatmap.OnlineScoreId = reader.GetInt64();
        else if (nodeId == 28) beatmap.TargetPracticeAccuracy = reader.GetDouble();
    }
}