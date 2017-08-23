using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Woodpecker.Core.Metrics.Configuration
{
    public class ConfigurationMapper
    {
        private char pairSeparator = ';';
        private char itemSeparator = '=';

        public IConfiguration Map(string connectionString)
        {
            var resultDictinary = connectionString.Split(pairSeparator).Select(ValidatePairSeparator).ToDictionary(result => result[0], result => result[1]);

            if (resultDictinary.Count != 4)
            {
                throw new ArgumentException(string.Format("The argument count must be 4 in the connection string: '{0}'.", connectionString));
            }

            return new Configuration()
            {
                ResourceId = ValidateItemName("ResourceId", resultDictinary),
                ClientSecret = ValidateItemName("ClientSecret", resultDictinary),
                ClientId = ValidateItemName("ClientId", resultDictinary),
                TenantId = ValidateItemName("TenantId", resultDictinary)
            };
        }

        private string ValidateItemName(string itemName, Dictionary<string, string> dictionary)
        {
            if (!dictionary.ContainsKey(itemName))
            {
                throw new ArgumentException(string.Format("The name '{0}' is not present in the connection string.", itemName));
            }

            var value = dictionary[itemName];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(string.Format("The value of '{0}' should not be empty or whitespace value in the connection string.", itemName));
            }

            return value;
        }

        private string[] ValidatePairSeparator(string itemPairString)
        {
            var match = Regex.Match(itemPairString, @"^\s*(\w+)\s*=\s*(\S*)\s*$", RegexOptions.Compiled);
            if (!match.Success)
            {
                throw new ArgumentException(string.Format("The name value pair '{0}' must be separated by '{1}' in the connection string.", itemPairString, itemSeparator));
            }

            return new[] { match.Groups[1].Value, match.Groups[2].Value };
        }
    }
}

