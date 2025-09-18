using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace lazyzu.Jira.Database.FieldOption.CodeGen.Shared
{
    public class VariableNameBuilder
    {
        private HashSet<string> insertedName = new HashSet<string>();

        public string BuildVariableName<TId>(string name, TId id)
        {
            var variableName = ToVariableName(name);

            if (insertedName.Contains(variableName))
            {
                variableName = $"{variableName}_Id{id}";
            }

            insertedName.Add(variableName);

            return variableName;
        }

        public string BuildVariableName<TId>(IEnumerable<IOption<TId>> options)
        {
            var _options = options?.ToArray() ?? new IOption<TId>[0];

            if (_options.Any() == false) return string.Empty;

            var firstOption = options.First();
            var variableParts = _options.Select(option =>
            {
                return ToVariableName(option.Value
                                    , bottomLinePrefix_IfNumberStart: option == firstOption);
            }).ToArray();

            var variableName = string.Join("_", variableParts);

            if (insertedName.Contains(variableName))
            {
                variableName = $"{variableName}_Id{_options.Last().Id}";
            }

            insertedName.Add(variableName);

            return variableName;
        }

        private static string ToVariableName(string name, bool bottomLinePrefix_IfNumberStart = true)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new NotSupportedException("not able to build property name by empty string");
            }
            else
            {
                var optionName = name
                    .Replace(',', ' ')
                    .Dehumanize()
                    .Dehumanize()
                    .Underscore()
                    .Pascalize();

                if (bottomLinePrefix_IfNumberStart && Regex.IsMatch(optionName, @"^\d")) optionName = $"_{optionName}";

                return optionName;
            }
        }

        public interface IOption<TId>
        {
            TId Id { get; }
            string Value { get; }
        }
    }
}
