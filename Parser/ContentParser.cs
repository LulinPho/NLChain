using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Basement.Parser
{
    public abstract class ContentParser
    {
        public abstract Regex _regex { get; set; }
        public abstract T ParseTo<T>(string input);

    }

    public class JsonParser : ContentParser
    {
        public override Regex _regex { get; set; } = new Regex("JSONREPLY```\\n([\\s\\S]*?)```");

        public override T ParseTo<T>(string input)
        {
            try
            {
                var jsonBlock = _regex.Match(input).Groups[1].Value;

                T output = JsonSerializer.Deserialize<T>(jsonBlock);

                return output;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed in parsing string into entities.");
            }

        }

    }
}
