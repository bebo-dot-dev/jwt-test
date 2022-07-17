namespace jwt_test
{
    public static class JwtConstants
    {
        /// <summary>
        /// SuperSecretKey is the token signing key, store this somewhere secure i.e. in a k8s secret
        /// </summary>
        public const string SuperSecretKey = "RWEH$%G$F$GH^%&^J^^&H$GG££FDC££$VV%H&DHH^&&J&^&*K&&&*K&";

        /// <summary>
        /// Audience limits validation of tokens to a specific token consumer (applicationId, similar to okta clientId)
        /// </summary>
        public const string Audience = "CE6559AA-54FA-4D4C-B879-AF1BF54FC9B8"; 
    }    
}

