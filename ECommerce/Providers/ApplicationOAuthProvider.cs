using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using AutobuyDirectApi.Models;
using System.Web.Http.Cors;
using System.Collections.Concurrent;
using Microsoft.Owin.Security.Infrastructure;
using System.Security.Cryptography;
using System.IO;

namespace AutobuyDirectApi.Providers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated(); //   
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            // var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            //ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            //if (user == null)
            //{
            //    context.SetError("invalid_grant", "The user name or password is incorrect.");
            //    return;
            //}

            //ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
            //   OAuthDefaults.AuthenticationType);
            //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
            //    CookieAuthenticationDefaults.AuthenticationType);

            //AuthenticationProperties properties = CreateProperties(user.UserName);
            //AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            //context.Validated(ticket);
            //context.Request.Context.Authentication.SignIn(cookiesIdentity);




            var identity = new ClaimsIdentity(context.Options.AuthenticationType);

            context.OwinContext.Response.Headers.Set("Access-Control-Allow-Origin", "*");

            using (var db = new EcommEntities())
            {
                if (db != null)
                {

                    int user_type = 0;
                    string err = "Provided username and password is incorrect";
                    string code = "1";
                    string userid = "";
                    string pushid = "";
                    userid = context.UserName;
                    if (context.UserName.Contains("-pushid"))
                    {
                        userid = context.UserName.Substring(0, context.UserName.LastIndexOf("-pushid"));
                        pushid = context.UserName.Substring(context.UserName.LastIndexOf("-pushid") + 7);
                    }
                    var user = db.Customers.ToList();
                    //   user_type = (int)user.Where(a => a.cust_email.Trim().ToLower() == userid.Trim().ToLower()).Select(a => a.user_type).Single();


                    try
                    {
                        //userid = context.UserName;                                                

                        if (user != null)
                        {

                            if (!string.IsNullOrEmpty(user.Where(u => (string.Equals(u.cust_mobile.Trim(), userid.Trim(), StringComparison.OrdinalIgnoreCase)) && u.cat_password == context.Password && u.cust_status == 1).FirstOrDefault().cust_mobile))
                            {
                                var login1 = db.Customers.Where(a => a.cust_mobile.Trim().ToLower() == userid.Trim().ToLower());
                                //foreach (User_Info uf in login1)
                                //{
                                //    uf.User_status = 1;
                                //    if (pushid != null && pushid != "")
                                //        uf.Push_id = pushid;
                                //}
                                //db.SaveChanges();


                                identity.AddClaim(new Claim(ClaimTypes.Role, userid));

                                var props = new AuthenticationProperties(new Dictionary<string, string>
                                        {
                                            {
                                                "userdisplayname", userid
                                            },
                                            {
                                                 "role", "customer"
                                            }
                                         });

                                var ticket = new AuthenticationTicket(identity, props);
                                context.Validated(ticket);
                                context.Validated(identity);
                            }

                            else
                            {
                                context.SetError("invalid_grant", "Provided username and password is incorrect");

                                context.Rejected();

                            }


                        }
                    }
                    catch (System.Exception e)
                    {
                        context.SetError(code, err);

                        //return e;//Response.status(Response.Status.UNAUTHORIZED).build();
                    }
                }
                else
                {
                    context.SetError("invalid_grant", "Provided username and password is incorrect");
                    context.Rejected();
                }

            }

        }

    }

    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        private static ConcurrentDictionary<string, AuthenticationTicket> _refreshTokens = new ConcurrentDictionary<string, AuthenticationTicket>();
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            var guid = Guid.NewGuid().ToString();

            // copy all properties and set the desired lifetime of refresh token  
            var refreshTokenProperties = new AuthenticationProperties(context.Ticket.Properties.Dictionary)
            {
                IssuedUtc = context.Ticket.Properties.IssuedUtc,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(1200000)//DateTime.UtcNow.AddYears(1)  
            };
            var refreshTokenTicket = new AuthenticationTicket(context.Ticket.Identity, refreshTokenProperties);

            _refreshTokens.TryAdd(guid, refreshTokenTicket);

            // consider storing only the hash of the handle  
            context.SetToken(guid);
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            AuthenticationTicket ticket;
            string header = context.OwinContext.Request.Headers["Authorization"];

            if (_refreshTokens.TryRemove(context.Token, out ticket))
            {
                context.SetTicket(ticket);
            }
        }
    }
}