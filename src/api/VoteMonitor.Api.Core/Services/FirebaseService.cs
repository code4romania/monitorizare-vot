using System.Collections.Generic;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace VoteMonitor.Api.Core.Services
{
    public class FirebaseService : IFirebaseService
    {
        public int SendAsync(string from, string title, string message, IList<string> recipients)
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
                Notification = new Notification
                {
                    Title = title,
                    Body = message
                },
            };

            var response = FirebaseMessaging.DefaultInstance.SendMulticastAsync(message2);
            response.Wait();

            return response.Result.SuccessCount;
        }
    }
}
