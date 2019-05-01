FROM microsoft/dotnet:2.2-sdk AS build-env
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
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /
COPY --from=build-env /app/api/VotingIrregularities.Api/out/ .
ENTRYPOINT ["dotnet", "VotingIrregularities.Api.dll"]