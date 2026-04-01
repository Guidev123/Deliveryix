using Deliveryix.Commons.Domain.ValueObjects;
using Modules.Identity.Domain.Identities.Enums;

namespace Modules.Identity.Domain.Identities.ValueObjects
{
    public sealed record LegalDocument : ValueObject
    {
        public const int CpfLength = 11;
        public const int CnpjLength = 14;

        private LegalDocument(string number)
        {
            Number = number;
            var type = number.Length switch
            {
                CpfLength => DocumentType.CPF,
                CnpjLength => DocumentType.CNPJ,
                _ => throw new NotImplementedException()
            };

            Type = type;
            Validate();
        }

        private LegalDocument()
        { }

        public string Number { get; } = null!;
        public DocumentType Type { get; }
        public IdentityType IdentityType => Type switch
        {
            DocumentType.CPF => IdentityType.Individual,
            DocumentType.CNPJ => IdentityType.Business,
            _ => throw new NotImplementedException()
        };

        public static implicit operator LegalDocument(string number)
            => new(number);

        protected override void Validate()
        {
        }
    }
}