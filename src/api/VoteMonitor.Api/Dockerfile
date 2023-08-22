#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["api/VoteMonitor.Api/VoteMonitor.Api.csproj", "api/VoteMonitor.Api/"]
COPY ["api/VoteMonitor.Api.County/VoteMonitor.Api.County.csproj", "api/VoteMonitor.Api.County/"]
COPY ["api/VoteMonitor.Api.Core/VoteMonitor.Api.Core.csproj", "api/VoteMonitor.Api.Core/"]
COPY ["api/VoteMonitor.Entities/VoteMonitor.Entities.csproj", "api/VoteMonitor.Entities/"]
COPY ["api/VoteMonitor.Api.Answer/VoteMonitor.Api.Answer.csproj", "api/VoteMonitor.Api.Answer/"]
COPY ["api/VoteMonitor.Api.Auth/VoteMonitor.Api.Auth.csproj", "api/VoteMonitor.Api.Auth/"]
COPY ["api/VoteMonitor.Api.DataExport/VoteMonitor.Api.DataExport.csproj", "api/VoteMonitor.Api.DataExport/"]
COPY ["api/VoteMonitor.Api.Form/VoteMonitor.Api.Form.csproj", "api/VoteMonitor.Api.Form/"]
COPY ["api/VoteMonitor.Api.Location/VoteMonitor.Api.Location.csproj", "api/VoteMonitor.Api.Location/"]
COPY ["api/VoteMonitor.Api.Ngo/VoteMonitor.Api.Ngo.csproj", "api/VoteMonitor.Api.Ngo/"]
COPY ["api/VoteMonitor.Api.Note/VoteMonitor.Api.Note.csproj", "api/VoteMonitor.Api.Note/"]
COPY ["api/VoteMonitor.Api.Notification/VoteMonitor.Api.Notification.csproj", "api/VoteMonitor.Api.Notification/"]
COPY ["api/VoteMonitor.Api.Observer/VoteMonitor.Api.Observer.csproj", "api/VoteMonitor.Api.Observer/"]
COPY ["api/VoteMonitor.Api.Statistics/VoteMonitor.Api.Statistics.csproj", "api/VoteMonitor.Api.Statistics/"]
COPY ["api/VoteMonitor.Api.PollingStation/VoteMonitor.Api.PollingStation.csproj", "api/VoteMonitor.Api.PollingStation/"]

COPY ["test/VoteMonitor.Api.PollingStation.Tests/VoteMonitor.Api.PollingStation.Tests.csproj", "test/VoteMonitor.Api.PollingStation.Tests/"]
COPY ["test/VoteMonitor.Api.Tests/VoteMonitor.Api.Tests.csproj", "test/VoteMonitor.Api.Tests/"]
COPY ["test/VotingIrregularities.Tests/VotingIrregularities.Tests.csproj", "test/VotingIrregularities.Tests/"]
RUN dotnet restore "api/VoteMonitor.Api/VoteMonitor.Api.csproj"
COPY . .
WORKDIR "/src/api/VoteMonitor.Api"
RUN dotnet build "VoteMonitor.Api.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src"
RUN dotnet test --logger:trx
WORKDIR "/src/api/VoteMonitor.Api"
RUN dotnet publish "VoteMonitor.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
RUN apk add --no-cache --repository https://alpine.global.ssl.fastly.net/alpine/edge/testing/ \
        libgdiplus-dev \
        fontconfig \
        ttf-dejavu
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VoteMonitor.Api.dll"]