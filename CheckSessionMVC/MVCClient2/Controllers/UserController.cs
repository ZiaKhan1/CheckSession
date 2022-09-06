using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IdentityModel;
using System.Net.Http;
using IdentityModel.Client;
using System.IdentityModel.Tokens.Jwt;

namespace MVCClient2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //[Authorize] With authorize, it will be redirected to auth.. 
        //make it tricky to be used from front end.
        public async Task<string> GetSubjectFromCode(string code)
        {
            try
            { 
            //From front-end, if this call is made just after
            //getting the authorization code, then user retrived
            //via token endpoint will be same as the current user.
            //However, using this would mean having to have the 
            //authorize attribute- for which there are issues.
            //sear ealrier comments.
            
            //var sub = User?.Claims?.FirstOrDefault(x => x.Type == "sub").Value;
            var client = new HttpClient();
            var response = await client.RequestAuthorizationCodeTokenAsync
            (new AuthorizationCodeTokenRequest
            {
                Address = "https://localhost:5000/v1/connect/token",
                Code = code,
                ClientId = "domain-accounts",
                ClientSecret = "98170e196e1b4ede8c1683bfb24960b5",
                RedirectUri = "https://localhost:44367/frontend"
            });

            if (response.IsError)
                return string.Empty;

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(response.IdentityToken);

            return token.Subject;
            }
            catch(Exception){ 
                return string.Empty;
            }

        }
    }
}