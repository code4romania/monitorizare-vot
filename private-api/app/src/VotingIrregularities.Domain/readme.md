### Creare baza de date

Assembly-ul VotingIrregularities.Domain are configurat EF Migrations si poate genera o baza de date impreuna cu date de test.

Pentru acest lucru trebuie urmati pasii de mai jos:

1. Completeaza in appsetings.json SAU adauga intr-un fisier nou appsettings.target.json connectionstring-ul catre instanta de SQL in care vrei sa creezi baza de date

2. ruleaza din consola, in directorul VotingIrregularities.Domain:

```sh
private-api\app\VotingIrregularities.Domain> dotnet run
```

Important este faptul ca actiunea de migrare va *sterge* datele din tabelele RaspunsDisponibil, Intrebare, Sectiune, Optiune.