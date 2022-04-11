using System.Text.Json;

namespace nat_api.core.serialization
{
    public class LowerCamelCase : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            var lastValue = name.Length - 1;
            lastValue = lastValue > 0 ? lastValue : 0;
            var newName = name.Substring(0, 1).ToLower() + name.Substring(1, lastValue);
            return newName;
        }
    }
}
