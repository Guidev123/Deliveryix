using System.Text.Json;

namespace Deliveryix.Commons.Infrastructure.Serializer
{
    public static class JsonSerializerOptionsExtensions
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        public static JsonSerializerOptions GetDefault()
        {
            return _jsonOptions;
        }
    }
}