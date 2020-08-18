using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using System;
using System.IO;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Core.Services;

namespace VotingIrregularities.Api.Extensions.Startup
{
    public static class FirebaseConfigurationExtension
    {
        public static IApplicationBuilder AddFirebase(this IApplicationBuilder app,
            IConfigurationRoot configuration, Container container)
        {
            var firebaseOptions = configuration.GetSection(nameof(FirebaseServiceOptions));
            var privateKeyPath = firebaseOptions[nameof(FirebaseServiceOptions.ServerKey)];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.GetFullPath(privateKeyPath));


            container.RegisterSingleton<IFirebaseService, FirebaseService>();

            return app;
        }
    }
}