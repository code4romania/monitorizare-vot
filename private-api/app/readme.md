## Rulare aplicatie

1. instaleaza .NetCore (Open Source/Free/Multiplatform) de [aici](https://www.microsoft.com/net/core#windows)

2. ruleaza din consola, in folderul app:
    ```sh
    private-api\app> dotnet restore
    ```
  
3. ruleaza din folderul VotingIrregularities.Api:
    ```sh
    private-api\app\VotingIrregularities.Api> dotnet run
    ```
  
4. browse to indicated address: <http://localhost:5000/swagger/ui>
