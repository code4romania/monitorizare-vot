#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["api/VotingIrregularities.Domain.Migrator/VotingIrregularities.Domain.Migrator.csproj", "api/VotingIrregularities.Domain.Migrator/"]
COPY ["api/VoteMonitor.Entities/VoteMonitor.Entities.csproj", "api/VoteMonitor.Entities/"]
RUN dotnet restore "api/VotingIrregularities.Domain.Migrator/VotingIrregularities.Domain.Migrator.csproj"
COPY . .
WORKDIR "/src/api/VotingIrregularities.Domain.Migrator"
RUN dotnet build "VotingIrregularities.Domain.Migrator.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/api/VotingIrregularities.Domain.Migrator"
RUN dotnet publish "VotingIrregularities.Domain.Migrator.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VotingIrregularities.Domain.Migrator.dll"]