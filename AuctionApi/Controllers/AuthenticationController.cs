using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApi.Controllers
{
    public class AuthenticationController : ControllerBase
    {
        private const string _clientId = "18pe5conu8fkudpqg6g59e5mib";
        private readonly RegionEndpoint _region = RegionEndpoint.USEast2;

        public class UserDetail
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }

        [HttpPost]
        [Route("api/register")]
        public async Task<ActionResult<string>> Register(UserDetail user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);
            return Ok();
            /*
            user.Username = "user-test";
            user.Password = "Qwertyuiop01!";
            user.Email = "smartalex302@aol.com";
            */
            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Password = user.Password,
                Username = user.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = user.Email
            };
            request.UserAttributes.Add(emailAttribute);

            var response = await cognito.SignUpAsync(request);

            return Ok();
        }


        [HttpPost]
        [Route("api/signin")]
        public async Task<ActionResult<string>> SignIn(UserDetail user)
        {
            /*
            user.Username = "user-test";
            user.Password = "Qwertyuiop01!";
            */
            try
            {
                var cognito = new AmazonCognitoIdentityProviderClient(_region);

                var request = new AdminInitiateAuthRequest
                {
                    UserPoolId = "us-east-2_Yqdph91I6",
                    ClientId = _clientId,
                    AuthFlow = AuthFlowType.ADMIN_NO_SRP_AUTH
                };

                request.AuthParameters.Add("USERNAME", user.Username);
                request.AuthParameters.Add("PASSWORD", user.Password);

                var response = await cognito.AdminInitiateAuthAsync(request);

                return Ok(response.AuthenticationResult.IdToken);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

    }
}
