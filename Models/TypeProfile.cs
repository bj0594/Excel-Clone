using System;
using System.Collections.Generic;
using ExcelClone.Data;

namespace ExcelClone.Models;

// Describes the types found in a table column.
//
// Each non-empty cell contributes to exactly one type count.
// The type with the highest count becomes the dominant type.
//
// If two or more types have the same number of occurrences,
// the type that appears first in the column wins the tie.
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

    // Numeric operations are available when the dominant
    // type of the column is int or double.
    public bool IsNumeric =>
        DominantType == DetectedType.Int ||
        DominantType == DetectedType.Double;

    public static TypeProfile FromCells(
        IEnumerable<ICell> cells)
    {
        int nonEmptyCount = 0;

        int stringCount = 0;
        int intCount = 0;
        int doubleCount = 0;
        int boolCount = 0;
        int dateTimeCount = 0;

        // Keep the first occurrence of each type.
        //
        // This gives us deterministic tie-breaking without
        // requiring another pass through the cells later.
        List<DetectedType> firstSeenTypes =
            new();

        foreach (ICell cell in cells)
        {
            if (cell.IsEmpty)
            {
                continue;
            }

            nonEmptyCount++;

            DetectedType type =
                GetDetectedType(
                    cell);

            switch (type)
            {
                case DetectedType.String:

                    stringCount++;
                    break;

                case DetectedType.Int:

                    intCount++;
                    break;

                case DetectedType.Double:

                    doubleCount++;
                    break;

                case DetectedType.Bool:

                    boolCount++;
                    break;

                case DetectedType.DateTime:

                    dateTimeCount++;
                    break;
            }

            if (!firstSeenTypes.Contains(type))
            {
                firstSeenTypes.Add(
                    type);
            }
        }

        if (nonEmptyCount == 0)
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

        DetectedType dominantType =
            DetermineDominantType(
                firstSeenTypes,
                stringCount,
                intCount,
                doubleCount,
                boolCount,
                dateTimeCount);

        return new TypeProfile
        {
            DominantType =
                dominantType,

            NonEmptyCount =
                nonEmptyCount,

            StringCount =
                stringCount,

            IntCount =
                intCount,

            DoubleCount =
                doubleCount,

            BoolCount =
                boolCount,

            DateTimeCount =
                dateTimeCount
        };
    }

    // Finds the type with the highest number of occurrences.
    //
    // firstSeenTypes is already ordered by first appearance,
    // so using ">" rather than ">=" preserves the first type
    // when two types have an equal count.
    private static DetectedType
        DetermineDominantType(
            IReadOnlyList<DetectedType> firstSeenTypes,
            int stringCount,
            int intCount,
            int doubleCount,
            int boolCount,
            int dateTimeCount)
    {
        Dictionary<DetectedType, int> counts =
            new()
            {
                [DetectedType.String] =
                    stringCount,

                [DetectedType.Int] =
                    intCount,

                [DetectedType.Double] =
                    doubleCount,

                [DetectedType.Bool] =
                    boolCount,

                [DetectedType.DateTime] =
                    dateTimeCount
            };

        DetectedType dominantType =
            DetectedType.Empty;

        int highestCount = 0;

        foreach (DetectedType type in firstSeenTypes)
        {
            int count =
                counts[type];

            if (count > highestCount)
            {
                highestCount =
                    count;

                dominantType =
                    type;
            }
        }

        return dominantType;
    }

    // Converts the runtime type exposed by ICell into
    // the application's DetectedType enum.
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