using System.Text.RegularExpressions;
using System.Globalization;

public static class FilterCriteriaParser
{
    private static readonly Regex CriteriaRegex = new Regex(
        @"^\s*(\w+)\s*(>=|<=|=|<|>)\s*(-?\d+(\.\d+)?)\s*$",
        RegexOptions.Compiled
    );

    private static readonly Regex OperationValueRegex = new Regex(
        @"^\s*(>=|<=|=|<|>)\s*(-?\d+(\.\d+)?)\s*$",
        RegexOptions.Compiled
    );

    public static bool TryParse(string input, out FilterCriteria criteria)
    {
        criteria = null;
        var match = CriteriaRegex.Match(input);
        if (!match.Success)
            return false;

        string attribute = match.Groups[1].Value;
        string opString = match.Groups[2].Value;
        string valueStr = match.Groups[3].Value;

        if (!float.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
            return false;

        if (!TryParseOperation(opString, out FilterOperation operation))
            return false;

        criteria = new FilterCriteria(attribute, operation, value);
        return true;
    }

    public static bool TryParseOperationAndValue(string input, out FilterOperation operation, out float value)
    {
        operation = default;
        value = 0f;

        var match = OperationValueRegex.Match(input);
        if (!match.Success)
            return false;

        string opString = match.Groups[1].Value;
        string valueStr = match.Groups[2].Value;

        if (!float.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
            return false;

        return TryParseOperation(opString, out operation);
    }

    public static bool TryParseOperation(string op, out FilterOperation operation)
    {
        operation = op switch
        {
            "=" => FilterOperation.Equal,
            "<" => FilterOperation.LessThan,
            ">" => FilterOperation.GreaterThan,
            "<=" => FilterOperation.LessThanOrEqual,
            ">=" => FilterOperation.GreaterThanOrEqual,
            _ => default
        };

        return op is "=" or "<" or ">" or "<=" or ">=";
    }
}
