using Deliveryix.Commons.Domain.DomainObjects;
using Deliveryix.Commons.Domain.ValueObjects;
using Modules.Identity.Domain.Identities.Enums;
using Modules.Identity.Domain.Identities.Errors;
using System.Text.RegularExpressions;

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
                _ => throw new DomainException(IdentityErrors.InvalidDocument().Description)
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
            _ => throw new DomainException(IdentityErrors.InvalidDocument().Description)
        };

        public static implicit operator LegalDocument(string number)
            => new(number);

        protected override void Validate()
        {
            AssertionConcern.EnsureNotEmpty(Number, IdentityErrors.InvalidDocument().Description);
            AssertionConcern.EnsureMatchesPattern(
                @"^\d+$",
                Number,
                IdentityErrors.InvalidDocument().Description);

            var isValid = Type switch
            {
                DocumentType.CPF => IsValidCpf(Number),
                DocumentType.CNPJ => IsValidCnpj(Number),
                _ => false
            };

            AssertionConcern.EnsureTrue(isValid, IdentityErrors.InvalidDocument().Description);
        }

        public static bool IsValidCpf(string cpf)
        {
            if (cpf.Distinct().Count() == 1) return false;

            int[] multipliers1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] multipliers2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

            var sum = cpf[..9].Select((c, i) => (c - '0') * multipliers1[i]).Sum();
            var remainder = sum % 11;
            var digit1 = remainder < 2 ? 0 : 11 - remainder;

            sum = cpf[..10].Select((c, i) => (c - '0') * multipliers2[i]).Sum();
            remainder = sum % 11;
            var digit2 = remainder < 2 ? 0 : 11 - remainder;

            return cpf[9] - '0' == digit1 && cpf[10] - '0' == digit2;
        }

        public static bool IsValidCnpj(string cnpj)
        {
            if (!Regex.IsMatch(cnpj, @"^[A-Z0-9]{12}\d{2}$", new RegexOptions(), TimeSpan.FromMilliseconds(50)))
                return false;

            if (cnpj.Distinct().Count() == 1) return false;

            static int CharValue(char c) => c - 48;

            int[] w1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
            int[] w2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

            var sum = cnpj[..12].Select((c, i) => CharValue(c) * w1[i]).Sum();
            var rem = sum % 11;
            var digit1 = rem < 2 ? 0 : 11 - rem;

            sum = cnpj[..12].Select((c, i) => CharValue(c) * w2[i]).Sum()
                       + digit1 * w2[12];
            rem = sum % 11;
            var digit2 = rem < 2 ? 0 : 11 - rem;

            return cnpj[12] - '0' == digit1 && cnpj[13] - '0' == digit2;
        }
    }
}