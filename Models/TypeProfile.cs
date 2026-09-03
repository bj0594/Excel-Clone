using System;
using System.Collections.Generic;
using System.Linq;
using ExcelClone.Data;

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
        DominantType == DetectedType.Int ||
        DominantType == DetectedType.Double;

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
                    0,

                StringCount = 0,
                IntCount = 0,
                DoubleCount = 0,
                BoolCount = 0,
                DateTimeCount = 0
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
                nonEmptyCells,
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
            List<ICell> cells,
            int strings,
            int ints,
            int doubles,
            int bools,
            int dates)
    {
        /*
         * Every actual type is counted separately.
         *
         * The type with the highest number of
         * occurrences wins.
         *
         * If two or more types have the same count,
         * the type that appeared first in the column
         * wins.
         */

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

        int highestCount =
            counts.Values.Max();

        foreach (ICell cell in cells)
        {
            DetectedType type =
                GetDetectedType(
                    cell);

            if (counts[type] ==
                highestCount)
            {
                return type;
            }
        }

        return DetectedType.Empty;
    }

    private static DetectedType
        GetDetectedType(
            ICell cell)
    {
        if (cell.ValueType ==
            typeof(string))
        {
            return DetectedType.String;
        }

        if (cell.ValueType ==
            typeof(int))
        {
            return DetectedType.Int;
        }

        if (cell.ValueType ==
            typeof(double))
        {
            return DetectedType.Double;
        }

        if (cell.ValueType ==
            typeof(bool))
        {
            return DetectedType.Bool;
        }

        if (cell.ValueType ==
            typeof(DateTime))
        {
            return DetectedType.DateTime;
        }

        return DetectedType.String;
    }
}