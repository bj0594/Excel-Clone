using System.Collections.Generic;
using System.Linq;

namespace ExcelClone.Models;

internal sealed class TypeProfile
{
    public DetectedType DominantType
    {
        get;
        private init;
    }

    public int NonEmptyCount
    {
        get;
        private init;
    }

    public int StringCount
    {
        get;
        private init;
    }

    public int IntCount
    {
        get;
        private init;
    }

    public int DoubleCount
    {
        get;
        private init;
    }

    public int BoolCount
    {
        get;
        private init;
    }

    public int DateTimeCount
    {
        get;
        private init;
    }

    public bool IsNumeric =>
        IntCount + DoubleCount > 0;

    public static TypeProfile FromCells(
        IEnumerable<ICell> cells)
    {
        List<ICell> nonEmptyCells =
            cells
                .Where(
                    cell =>
                        !cell.IsEmpty)
                .ToList();

        if (nonEmptyCells.Count == 0)
        {
            return new TypeProfile
            {
                DominantType =
                    DetectedType.Empty,

                NonEmptyCount =
                    0
            };
        }

        int strings =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(string));

        int ints =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(int));

        int doubles =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(double));

        int bools =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(bool));

        int dates =
            nonEmptyCells.Count(
                cell =>
                    cell.ValueType ==
                    typeof(DateTime));

        DetectedType dominantType =
            DetermineDominantType(
                strings,
                ints,
                doubles,
                bools,
                dates);

        return new TypeProfile
        {
            DominantType =
                dominantType,

            NonEmptyCount =
                nonEmptyCells.Count,

            StringCount =
                strings,

            IntCount =
                ints,

            DoubleCount =
                doubles,

            BoolCount =
                bools,

            DateTimeCount =
                dates
        };
    }

    private static DetectedType
        DetermineDominantType(
            int strings,
            int ints,
            int doubles,
            int bools,
            int dates)
    {
        Dictionary<DetectedType, int>
            counts =
                new()
                {
                    [DetectedType.String] =
                        strings,

                    [DetectedType.Int] =
                        ints,

                    [DetectedType.Double] =
                        doubles,

                    [DetectedType.Bool] =
                        bools,

                    [DetectedType.DateTime] =
                        dates
                };

        return counts
            .OrderByDescending(
                pair =>
                    pair.Value)
            .ThenBy(
                pair =>
                    TypePriority(
                        pair.Key))
            .First()
            .Key;
    }

    private static int TypePriority(
        DetectedType type)
    {
        return type switch
        {
            DetectedType.Int => 0,
            DetectedType.Double => 1,
            DetectedType.DateTime => 2,
            DetectedType.Bool => 3,
            DetectedType.String => 4,
            _ => 5
        };
    }
}