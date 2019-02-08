FROM microsoft/dotnet:2.1-sdk AS build-env
WORKDIR /app

COPY private-api/app/VotingIrregularities.sln ./
COPY private-api/app/src/VotingIrregularities.Api/VotingIrregularities.Api.csproj src/VotingIrregularities.Api/
COPY private-api/app/src/VotingIrregularities.Domain/VotingIrregularities.Domain.csproj src/VotingIrregularities.Domain/
COPY private-api/app/test/VotingIrregularities.Domain.Tests/VotingIrregularities.Domain.Tests.csproj test/VotingIrregularities.Domain.Tests/
COPY private-api/app/test/VotingIrregularities.Tests/VotingIrregularities.Tests.csproj test/VotingIrregularities.Tests/

# Copy all at once (big image probably?)
#COPY private-api/app/. ./

RUN dotnet restore

# Copy everything else and build
COPY private-api/app/. ./
RUN dotnet publish -c Release -o ./out

# test application -- see: dotnet-docker-unit-testing.md
FROM build-env AS testrunner
WORKDIR /app/tests
COPY private-api/app/test/. .
ENTRYPOINT ["dotnet", "test", "--logger:trx"]

# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /
COPY --from=build-env /app/src/VotingIrregularities.Api/out/ .
ENTRYPOINT ["dotnet", "VotingIrregularities.Api.dll"]