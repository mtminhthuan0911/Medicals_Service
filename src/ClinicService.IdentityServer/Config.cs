// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ClinicService.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("clinic.service.api", "Clinic Service Api")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "swagger",
                    ClientSecrets = { new Secret("swagger.secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris =           { "https://localhost:5001/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "https://localhost:5001/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins =     { "https://localhost:5001" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "clinic.service.api"
                    }
                },
                new Client
                {
                    ClientId = "webclient",
                    ClientSecrets = { new Secret("webclient.secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    // where to redirect to after login
                    RedirectUris = { "https://localhost:5002/signin-oidc" },

                    //FrontChannelLogoutUri = "https://localhost:5002/signout-oidc",

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                    AllowOfflineAccess = true,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "clinic.service.api"
                    }
                },
                new Client
                {
                    ClientId = "webadmin",

                    ClientSecrets = { new Secret("webadmin.secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Implicit,

                    // where to redirect to after login
                    RedirectUris = { "http://localhost:4200/auth-callback" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:4200" },

                    AllowedCorsOrigins = new List<string> { "http://localhost:4200" },

                    AllowAccessTokensViaBrowser = true,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "clinic.service.api"
                    }
                },
            };
    }
}