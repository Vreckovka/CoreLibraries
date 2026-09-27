using System;
using System.Reflection;
using System.Security.Authentication;
using System.Threading.Tasks;
using PCloudClient.Protocol;

namespace PCloudClient.Api
{
	/// <summary>RPCs related to authentication</summary>
	public static class Authentication
	{
		static Authentication()
    {
      deviceInfoString = GetDeviceInfo();
    }

    public static string GetDeviceInfo()
    {
      Assembly ass = Assembly.GetEntryAssembly();
      var apa = ass.GetCustomAttribute<AssemblyProductAttribute>();
      if( null != apa )
        return $"{ apa.Product }, { ass.GetName().Version }";
      else
        return ass.FullName;
    }

		/// <summary>Apparently, their web interface shows that data for live sessions. Good idea to set into something more descriptive before using the library.</summary>
		public static string deviceInfoString;

		static async Task<string> getDigest( this Connection conn )
		{
			var request = new RequestBuilder( "getdigest" );
			var response = await conn.send( request );
			return (string)response[ "digest" ];
		}

		/// <summary>Login with e-mail and password</summary>
		public static async Task login( this Connection conn, string email, string password )
		{
			if( conn.isAuthenticated )
				await conn.logout();

			string digest = await conn.getDigest();
			string passwordDigest = Utils.Utils.sha1( password + Utils.Utils.sha1( email.ToLowerInvariant() ) + digest );
			var req = new RequestBuilder( "userinfo" );
			req.add( "getauth", 1L );
			req.add( "logout", true );
			req.add( "username", email );
			req.add( "digest", digest );
			req.add( "passworddigest", passwordDigest );
			// Set device global parameter
			req.add( "device", deviceInfoString );
			var response = await conn.send( req );
			if( response.dict.TryGetValue( "auth", out object auth ) &&
				auth is string token && !string.IsNullOrWhiteSpace( token ) )
			{
				conn.authToken = token;
				return;
			}

			// Binary API login can authenticate this connection without issuing a token.
			// Verify that a credential-free request still identifies the same account.
			if( !response.dict.TryGetValue( "userid", out object userId ) || userId == null )
				throw new AuthenticationException( "pCloud returned neither an authentication token nor an account identity." );

			var verification = await conn.send( new RequestBuilder( "userinfo" ) );
			if( !verification.dict.TryGetValue( "userid", out object verifiedUserId ) ||
				!object.Equals( userId, verifiedUserId ) )
				throw new AuthenticationException( "pCloud did not confirm the authenticated account on this connection." );

			conn.isSessionAuthenticated = true;
		}

		/// <summary>Logout</summary>
		public static async Task logout( this Connection conn )
		{
			if( string.IsNullOrEmpty( conn.authToken ) && conn.isSessionAuthenticated )
			{
				// The logout global parameter clears the connection's login even when
				// there is no reusable token for the logout method to invalidate.
				var reset = new RequestBuilder( "getdigest" );
				reset.add( "logout", true );
				await conn.send( reset );
				conn.isSessionAuthenticated = false;
				return;
			}

			var req = conn.newRequest( "logout" );
			var response = await conn.send( req );
			if( !(bool)response[ "auth_deleted" ] )
				throw new ApplicationException( "Unable to logout" );
			conn.authToken = null;
			conn.isSessionAuthenticated = false;
		}

		/// <summary>Change current user's password; requires SSL encrypted server connection.</summary>
		public static Task changePassword( this Connection conn, string oldPassword, string newPassword )
		{
			if( !conn.isEncrypted )
				throw new NotSupportedException( "For security reasons, you must use encrypted connection to change a password." );

			var req = conn.newRequest( "changepassword" );
			req.addSecret( "oldpassword", oldPassword );
			req.addSecret( "newpassword", newPassword );
			return conn.send( req );
		}
	}
}