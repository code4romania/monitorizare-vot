FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS build-env
WORKDIR /app

# Copy sources
COPY /src/. ./

# Restore packages
RUN dotnet restore

# Build and publish as `Release`
RUN dotnet publish -c Release -o ./out

# test application -- see: dotnet-docker-unit-testing.md
FROM build-env AS testrunner
WORKDIR /app/tests
COPY /src/test/. .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

# Build runtime image
FROM microsoft/dotnet:3.1-aspnetcore-runtime
WORKDIR /
COPY --from=build-env /app/api/VoteMonitor.Api/out/ .
ENTRYPOINT ["dotnet", "VoteMonitor.Api.dll"]