using System.Collections.Generic;
using UnityEngine;

public enum FilterOperation
{
    Equal,
    LessThan,
    GreaterThan,
    LessThanOrEqual,
    GreaterThanOrEqual
}

public class FilterCriteria
{
    public string Attribute { get; private set; }
    public FilterOperation Operation { get; private set; }
    public float Value { get; private set; }

    public FilterCriteria(string attribute, FilterOperation operation, float value)
    {
        Attribute = attribute;
        Operation = operation;
        Value = value;
    }

    public bool Evaluate(Dictionary<string, float> attributes)
    {
        if (!attributes.TryGetValue(Attribute, out float attrValue))
            return false;

        return Operation switch
        {
            FilterOperation.Equal => attrValue == Value,
            FilterOperation.LessThan => attrValue < Value,
            FilterOperation.GreaterThan => attrValue > Value,
            FilterOperation.LessThanOrEqual => attrValue <= Value,
            FilterOperation.GreaterThanOrEqual => attrValue >= Value,
            _ => false
        };
    }

    public override string ToString()
    {
        string op = Operation switch
        {
            FilterOperation.Equal => "=",
            FilterOperation.LessThan => "<",
            FilterOperation.GreaterThan => ">",
            FilterOperation.LessThanOrEqual => "<=",
            FilterOperation.GreaterThanOrEqual => ">=",
            _ => "?"
        };
        return $"{Attribute} {op} {Value}";
    }
}
