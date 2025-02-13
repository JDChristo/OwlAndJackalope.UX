﻿namespace OwlAndJackalope.UX.Runtime.Conditions
{
    public enum Comparison
    {
        Equal = 0,
        NotEqual = 1,
        GreaterThan = 2,
        GreaterThanEqual = 3,
        LessThan = 4,
        LessThanEqual = 5,
        IsSet = 6,
    }

    public static class ComparisonExtensions
    {
        public static readonly string[] AsString = new[]
        {
            "==",
            "!=",
            ">",
            ">=",
            "<",
            "<=",
            "is set"
        };
    }
}