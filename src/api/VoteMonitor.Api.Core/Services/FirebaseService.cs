using System.Collections.Generic;
using System.Linq;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace VoteMonitor.Api.Core.Services
{
	public class FirebaseService : IFirebaseService
	{
		public int SendAsync(string from, string title, string message, List<string> recipients)
		{
			if (FirebaseApp.DefaultInstance == null)
			{
				FirebaseApp.Create(new AppOptions()
				{
					Credential = GoogleCredential.GetApplicationDefault(),
				});
			}

			int successCount = 0;

			if (recipients == null)
			{
				return successCount;
			}

			while (recipients.Any())
			{
				var registrationTokens = recipients.Take(100) as IReadOnlyList<string>;
				recipients = recipients.Skip(100).ToList();

				var message2 = new MulticastMessage
				{
					Tokens = registrationTokens,
					Data = new Dictionary<string, string>
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
				successCount += response.Result.SuccessCount;
			}

			return successCount;
		}
	}
}
