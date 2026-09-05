using ExcelClone.Data;

namespace ExcelClone.Models;

// Describes the types found in a table column.
//
// Each non-empty cell contributes to exactly one type count.
// The type with the highest count becomes the dominant type.
//
// If two or more types have the same number of occurrences,
// the type that reached that count first keeps the lead.
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

        DetectedType dominantType =
            DetectedType.Empty;

        int highestCount = 0;

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

            int currentCount;

            switch (type)
            {
                case DetectedType.String:

                    stringCount++;

                    currentCount =
                        stringCount;

                    break;

                case DetectedType.Int:

                    intCount++;

                    currentCount =
                        intCount;

                    break;

                case DetectedType.Double:

                    doubleCount++;

                    currentCount =
                        doubleCount;

                    break;

                case DetectedType.Bool:

                    boolCount++;

                    currentCount =
                        boolCount;

                    break;

                case DetectedType.DateTime:

                    dateTimeCount++;

                    currentCount =
                        dateTimeCount;

                    break;

                default:

                    currentCount = 0;

                    break;
            }

            // Only replace the current dominant type when
            // the new count is strictly higher.
            //
            // Using ">" rather than ">=" preserves the type
            // that reached the current highest count first
            // when two types are tied.
            if (currentCount > highestCount)
            {
                highestCount =
                    currentCount;

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