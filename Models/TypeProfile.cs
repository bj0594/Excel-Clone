using System.Collections.Generic;
using ExcelClone.Data;

namespace ExcelClone.Models;

// Describes the types found in a table column.
//
// The type occurring most often becomes the dominant type.
// If multiple types occur equally often, the type that
// appears first in the column wins the tie.
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
    // type is int or double.
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

        // Store the order in which types first appear.
        // This is only needed when two types have the same count.
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
            DetectedType.Empty;

        int highestCount = 0;

        // Types are examined in first-seen order.
        //
        // Because we only replace the winner when the count
        // is strictly greater, an equal count keeps the type
        // that appeared first in the column.
        foreach (DetectedType type in firstSeenTypes)
        {
            int count =
                GetCount(
                    type,
                    stringCount,
                    intCount,
                    doubleCount,
                    boolCount,
                    dateTimeCount);

            if (count > highestCount)
            {
                highestCount =
                    count;

                dominantType =
                    type;
            }
        }

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

    private static int GetCount(
        DetectedType type,
        int stringCount,
        int intCount,
        int doubleCount,
        int boolCount,
        int dateTimeCount)
    {
        return type switch
        {
            DetectedType.String =>
                stringCount,

            DetectedType.Int =>
                intCount,

            DetectedType.Double =>
                doubleCount,

            DetectedType.Bool =>
                boolCount,

            DetectedType.DateTime =>
                dateTimeCount,

            _ =>
                0
        };
    }

    // Converts the runtime type exposed by ICell into
    // the application's DetectedType enum.
    private static DetectedType GetDetectedType(
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