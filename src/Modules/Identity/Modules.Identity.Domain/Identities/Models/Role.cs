namespace Modules.Identity.Domain.Identities.Models
{
    public sealed class Role
    {
        public Role(string name)
        {
            Name = name;
        }

        public string Name { get; private set; } = null!;
    }
}