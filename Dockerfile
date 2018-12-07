FROM microsoft/dotnet:1.1-sdk

WORKDIR /opt/app

COPY private-api/app /opt/app
RUN dotnet restore

ENV ASPNETCORE_URLS=http://+:8000

WORKDIR /opt/app/src/VotingIrregularities.Api
CMD ["dotnet", "run"]
