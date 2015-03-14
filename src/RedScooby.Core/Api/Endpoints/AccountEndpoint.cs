// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System.Threading.Tasks;
using RedScooby.Api.Core;
using RedScooby.Data.Design;
using RedScooby.Logging;
using RedScooby.Models;

namespace RedScooby.Api.Endpoints
{
    public class AccountEndpoint : RedscoobyApiEndpoint
    {
        public enum PhoneVerificationType
        {
            Text = 0,
            Call = 1
        }

        private UserModel registeredModel;

        internal AccountEndpoint(RedscoobyApiEndpoint parent)
            : base(parent, "account") { }

        public async Task<LoginResult> Login(string phoneNumber, string token, string phoneVerificationKey)
        {
            Log.Trace(f => f("Api call: Login: ", phoneNumber, token, phoneVerificationKey));
            await Task.Delay(1000);
            var user = registeredModel ?? new SampleAppModel().GetUserModel();
            user.Session.IsAuthenticationValid = true;
            return new LoginResult
            {
                User = user,
            };
        }

        public async Task<UserRegistrationResult> RegisterUser(UserModel user)
        {
            Log.Trace(f => f("Api call: RegisterUser: ", user));
            await Task.Delay(1000);
            //var res = await Client.PostAsync(GetApiPath("/register"), user, Formatter);
            registeredModel = user;
            registeredModel.Id = 100;
            return new UserRegistrationResult();
        }

        public async Task<PhoneVerificationRequestResult> RequestPhoneVerification(string phoneNumber,
            PhoneVerificationType type)
        {
            Log.Trace(f => f("Api call: RequestPhoneVerification: ", phoneNumber, type));
            await Task.Delay(1000);
            return new PhoneVerificationRequestResult {Token = "DummyToken"};
        }

        public class PhoneVerificationRequestResult
        {
            public string Error { get; set; }
            public string Token { get; set; }
        }

        public class UserRegistrationResult
        {
            public string Error { get; set; }
            public int UserId { get; set; }
        }

        public class LoginResult
        {
            public string Error { get; set; }
            public UserModel User { get; set; }
        }
    }
}
