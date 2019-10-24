using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoteMonitor.Api.Core.Services.Impl
{
    public class FirebaseService : IFirebaseService
    {
        public int SendAsync(String from, String title, String message, IList<string> recipients)
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                });
            }

            var registrationTokens = recipients as IReadOnlyList<string>;

            var message2 = new MulticastMessage()
            {
                Tokens = registrationTokens,
                Data = new Dictionary<string, string>()
                {
                    { "title", title },
                    { "body", message },
                },
            };

            var response = FirebaseMessaging.DefaultInstance.SendMulticastAsync(message2);
            response.Wait();

            return response.Result.SuccessCount;
        }
    }
}
