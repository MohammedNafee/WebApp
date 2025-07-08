namespace WebAPIApp.Attributes
{
    // This attribute is used to specify that a method or class requires a specific claim to be present in the user's JWT token.
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RequiredClaimAttribute: Attribute
    {
        // The claim is a key-value pair, where the key is the claim type and the value is the claim value.
        public string ClaimType { get; }
        public string ClaimValue { get; }

        // Constructor to initialize the required claim type and value.
        public RequiredClaimAttribute(string claimType, string claimValue)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }
    }
}
