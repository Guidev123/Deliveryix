namespace Deliveryix.Commons.WebApi.AuthorizationHeaders
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class RequireAuthorizationHeadersAttribute : Attribute;
}