namespace Modules.Identity.Domain.AcessManagement.Models
{
    public sealed class Permission
    {
        public Permission(string code)
        {
            Code = code;
        }

        public string Code { get; private set; } = null!;
    }
}