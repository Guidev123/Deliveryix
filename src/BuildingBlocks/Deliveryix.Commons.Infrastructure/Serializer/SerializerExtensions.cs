using Newtonsoft.Json;

namespace Deliveryix.Commons.Infrastructure.Serializer
{
    public static class SerializerExtensions
    {
        public static readonly JsonSerializerSettings Instance = new()
        {
            TypeNameHandling = TypeNameHandling.All,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        };
    }
}