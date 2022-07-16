namespace jwt_test
{
    public static class JwtConstants
    {
        public const string JwtSuperSecretKey = "RWEH$%G$F$GH^%&^J^^&H$GG££FDC££$VV%H&DHH^&&J&^&*K&&&*K&"; //store in a k8s secret
        public const string JwtTokenIssuer = "https://token.issuer.com"; //identifies the issuing service 
        public const string JwtTokenAudience = "https://token.audience.com"; //limit validation of tokens to specific token consumers 
    }    
}

