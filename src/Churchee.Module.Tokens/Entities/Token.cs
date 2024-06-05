using Churchee.Common.Attributes;
using Churchee.Common.Data;

namespace Churchee.Module.Tokens.Entities
{
    public class Token : Entity
    {
        public Token(Guid applicationTenantId, string key, string value) : base(applicationTenantId)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; private set; }

        [EncryptProperty]
        public string Value { get; private set; }

    }
}
