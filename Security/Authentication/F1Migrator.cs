/*   * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the
     * documentation and/or other materials provided with the distribution.
     * Neither the name of BCrypt.Net nor the names of its contributors may be used to endorse or promote products derived from this software without
     * specific prior written permission.
     *
     * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
     * TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
     * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
     * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
     * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
     * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
     */

using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Net;
using RestSharp;
using RestSharp.Authenticators;
using Rock;
using Rock.Attribute;
using Rock.Model;
using Rock.Security;
using Rock.Security.Authentication;
using Rock.Web.Cache;

namespace cc.newspring.F1.Security.Authentication
{
    [Description( "F1 Authentication and Migration Provider" )]
    [Export( typeof( AuthenticationComponent ) )]
    [ExportMetadata( "ComponentName", "F1 Migrator" )]
    [TextField( "Base Url", "The base url to use to check the credentials against", true, "https://newspring.fellowshiponeapi.com/" )]
    [TextField( "Resource Url", "The resource url to use to check the credentials against", true, "v1/WeblinkUser/AccessToken" )]
    [TextField( "ConsumerKey", "The rest url to use to check the credentials against", true )]
    [TextField( "ConsumerSecret", "The rest url to use to check the credentials against", true )]
    public class F1Migrator : AuthenticationComponent
    {
        /// <summary>
        /// Initializes the class.
        /// </summary>
        static F1Migrator()
        {
        }

        /// <summary>
        /// Gets the type of the service.
        /// </summary>
        /// <value>
        /// The type of the service.
        /// </value>
        public override AuthenticationServiceType ServiceType
        {
            get { return AuthenticationServiceType.Internal; }
        }

        /// <summary>
        /// Determines if user is directed to another site (i.e. Facebook, Gmail, Twitter, etc) to confirm approval of using
        /// that site's credentials for authentication.
        /// </summary>
        /// <value>
        /// The requires remote authentication.
        /// </value>
        public override bool RequiresRemoteAuthentication
        {
            get { return false; }
        }

        /// <summary>
        /// Authenticates the specified user name and password
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override bool Authenticate( UserLogin user, string password )
        {
            var passwordIsCorrect = CheckF1Password( user.UserName, password );

            if ( passwordIsCorrect )
            {
                // TODO: Does this work?
                user.EntityTypeId = EntityTypeCache.Read( Rock.SystemGuid.EntityType.AUTHENTICATION_DATABASE.AsGuid() ).Id;
                var databaseAuth = new Database();
                databaseAuth.SetPassword( user, password );
            }

            return passwordIsCorrect;
        }

        /// <summary>
        /// Authenticates the user based on a request from a third-party provider.  Will set the username and returnUrl values.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Authenticate( System.Web.HttpRequest request, out string userName, out string returnUrl )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encodes the password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password"></param>
        /// <returns></returns>
        public override string EncodePassword( UserLogin user, string password )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the password.
        /// </summary>
        /// <param name="UserLogin">The user login.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        /// 
        public override void SetPassword( UserLogin user, string password )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates the login URL.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Uri GenerateLoginUrl( System.Web.HttpRequest request )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tests the Http Request to determine if authentication should be tested by this
        /// authentication provider.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool IsReturningFromAuthentication( System.Web.HttpRequest request )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the URL of an image that should be displayed.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string ImageUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating whether [supports change password].
        /// </summary>
        /// <value>
        /// <c>true</c> if [supports change password]; otherwise, <c>false</c>.
        /// </value>
        public override bool SupportsChangePassword
        {
            get { return false; }
        }

        /// <summary>
        /// Changes the password.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="oldPassword">The old password.</param>
        /// <param name="newPassword">The new password.</param>
        /// <param name="warningMessage">The warning message.</param>
        /// <returns></returns>
        public override bool ChangePassword( UserLogin user, string oldPassword, string newPassword, out string warningMessage )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks the f1 password using the F1 API.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        private bool CheckF1Password( string username, string password )
        {
            var baseUrl = GetAttributeValue( "BaseUrl" );
            var resourceUrl = GetAttributeValue( "ResourceUrl" );
            var consumerKey = GetAttributeValue( "ConsumerKey" );
            var consumerSecret = GetAttributeValue( "ConsumerSecret" );

            var restClient = new RestClient( baseUrl )
            {
                Authenticator = OAuth1Authenticator.ForRequestToken( consumerKey, consumerSecret )
            };
            
            // Get information about the person who logged in
            var restRequest = new RestRequest( resourceUrl, Method.POST );
            var body = Base64Encode( username + " " + password );
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.AddBody( body );
            var restResponse = restClient.Execute( restRequest );
            return restResponse.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Base64s the plainText.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <returns></returns>
        private static string Base64Encode( string plainText )
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes( plainText );
            return System.Convert.ToBase64String( plainTextBytes );
        }
	}
}