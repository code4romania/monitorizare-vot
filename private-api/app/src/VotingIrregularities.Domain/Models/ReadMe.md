Din folderul proiectului VotingIrregularities.Domain se ruleaza :
1. dotnet ef --startup-project ../VotingIrregularities.Api dbcontext scaffold "{connection string}"  Microsoft.EntityFrameworkCore.SqlServer --context VotingContext --output-dir ./Models
2. se sterge connectionstring din VotingContext