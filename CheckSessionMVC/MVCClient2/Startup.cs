using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;

namespace MVCClient2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true;
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

                services.AddAuthentication(options =>
                    {
                        options.DefaultScheme = "Cookies";
                        options.DefaultChallengeScheme = "oidc";
                    })
                    .AddCookie("Cookies")
                    .AddOpenIdConnect("oidc", options =>
                    {
                        //options.Authority = "https://cognito-idp.ap-southeast-2.amazonaws.com/ap-southeast-2_zpiNmQpmk";
                        options.Authority = "https://stage-auth.domain.com.au/v1";
                        //options.Authority = "https://localhost:5000/v1";

                        //options.Authority = "https://localhost:5001";
                        //options.ClientId = "mvc";alice
                        //options.ClientSecret = "secret";
                        options.GetClaimsFromUserInfoEndpoint = true;
                        //options.Scope.Add("roles");zia.khan@domain.com.aualice
                        options.Scope.Add("email");
                        //options.Scope.Add("offline_access");
                        
                        options.RequireHttpsMetadata = false;

                        //options.Authority = "https://localhost:5000/v1";
                        options.ClientId = "domain-accounts";
                        //options.ClientSecret = "b15e5ab2426a4d4692b0a8534b5f63e5"; 
                        options.ClientSecret = "51725b2dea984643a742135ac74d92b8";
                        //options.ClientSecret = "17c35qo97vi2tvfj945vrugnnl4lm9fsjthprfpf40j8qko9qjqj";
                        options.ResponseType = "code";
                        options.UsePkce  = false;
                        options.SaveTokens = true;
                        //options.ClaimActions.Add(new JsonKeyClaimAction(JwtClaimTypes.Scope, "member-self-management", JwtClaimTypes.Scope));

                        //options.Events.OnTokenValidated = context =>
                        //{
                        // ((ClaimsIdentity)context.HttpContext.User.Identity).AddClaim(new Claim("id_token", 
                        //     context.ProtocolMessage.IdToken));

                        //    //id.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        //    //n.AuthenticationTicket = new AuthenticationTicket(id,n.AuthenticationTicket.Properties);
                        //    return Task.FromResult(0);
                        //};
                        options.Events.OnRedirectToIdentityProviderForSignOut = context  =>
                        {
                            var idTokenHint = context.HttpContext.User.FindFirst("id_token");
                            context.ProtocolMessage.IdTokenHint = null;
                            //if (idTokenHint != null)
                                //context.ProtocolMessage.IdTokenHint = idTokenHint.Value;

                            //context.ProtocolMessage.PostLogoutRedirectUri = "https://localhost:44367";

                            //context.ProtocolMessage.IdTokenHint = "1b2";

                            return Task.FromResult(0);
                        };
                        
                        options.Events.OnTokenResponseReceived = context =>
                        {
                            var refreshToken = context.TokenEndpointResponse.RefreshToken;
                            return Task.FromResult(0);

                        };
                    });

            services.AddAuthorization(options =>
            {
                string domain = "https://dev-auth.domain.com.au/v1";
                options.AddPolicy("member-self-management", policy => policy.Requirements.Add(new HasScopeRequirement("member-self-management", domain)));

                //options.AddPolicy("ManageMembershipServer",
                   // policy => policy.RequireClaim("scope", "member-self-management"));

            });

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //        name: "default",
            //        pattern: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute()
                    .RequireAuthorization();
            });

        }
    }


    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; }
        public string Scope { get; }

        public HasScopeRequirement(string scope, string issuer)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }
    }

    // HasScopeHandler.cs

    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
                return Task.CompletedTask;

            // Split the scopes string into an array
            var scopes = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer).Value.Split(' ');

            // Succeed if the scope array contains the required scope
            if (scopes.Any(s => s == requirement.Scope))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
