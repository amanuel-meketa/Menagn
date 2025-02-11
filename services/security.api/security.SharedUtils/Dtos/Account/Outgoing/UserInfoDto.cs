namespace security.sharedUtils.Dtos.Account.Outgoing
{
    public class UserInfoDto
    {
        public string Exp { get; set; } // Expiration time
        public string Iat { get; set; } // Issued at time
        public string Jti { get; set; } // JWT ID
        public string Iss { get; set; } // Issuer
        public string Aud { get; set; } // Audience
        public string NameIdentifier { get; set; } // Name Identifier
        public string Typ { get; set; } // Token type
        public string Azp { get; set; } // Authorized party
        public string SessionState { get; set; } // Session state
        public string AuthnClassReference { get; set; } // Authentication class reference
        public string AllowedOrigins { get; set; } // Allowed origins
        public string RealmAccess { get; set; } // Realm access roles
        public string ResourceAccess { get; set; } // Resource access roles
        public string Scope { get; set; } // Scope
        public string Sid { get; set; } // Session ID
        public string EmailVerified { get; set; } // Email verified
        public string Name { get; set; } // User's full name
        public string PreferredUsername { get; set; } // Preferred username
        public string GivenName { get; set; } // Given name
        public string Surname { get; set; } // Surname
        public string EmailAddress { get; set; } // Email address
        public string? AccessToken { get; set; }
    }
}
