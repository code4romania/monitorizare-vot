using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Handlers;
using VoteMonitor.Api.County.Queries;
using VoteMonitor.Api.Observer.Queries;
using VoteMonitor.Entities;
using Xunit;

namespace VotingIrregularities.Tests.CountyApi
{
    public class CountiesCommandHandlerTests
    {
        Mock<ILogger> _fakeLogger = new Mock<ILogger>();
        private DbContextOptions<VoteMonitorContext> _dbContextOptions;

        public CountiesCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }

        [Fact]
        public async Task When_loading_2_counties_from_db()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.Counties.Add(new County
                {
                    Code = "Code1", Diaspora = false, Id = 1, Name = "Name1", Order = 12, NumberOfPollingStations = 14
                });
                context.Counties.Add(new County
                    {Code = "Code2", Diaspora = true, Id = 3, Name = "Name2", Order = 1, NumberOfPollingStations = 2});
                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);
                var exportResult =
                    await countiesCommandHandler.Handle(new GetCountiesForExport(), new CancellationToken(false));

                exportResult.IsSuccess.ShouldBeTrue();

                exportResult.Value.Count.ShouldBe(2);
                exportResult.Value
                    .FirstOrDefault(x => x.Code == "Code1"
                                         && x.Diaspora == false
                                         && x.Id == 1
                                         && x.Name == "Name1"
                                         && x.Order == 12
                                         && x.NumberOfPollingStations == 14)
                    .ShouldNotBeNull();

                exportResult.Value
                    .FirstOrDefault(x => x.Code == "Code2"
                                         && x.Diaspora == true
                                         && x.Id == 3
                                         && x.Name == "Name2"
                                         && x.Order == 1
                                         && x.NumberOfPollingStations == 2)
                    .ShouldNotBeNull();
            }
        }


        [Fact]
        public async Task When_loading_counties_and_no_data_in_db()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);
                var result = await countiesCommandHandler.Handle(new GetCountiesForExport(), new CancellationToken(false));
                result.IsSuccess.ShouldBeTrue();
                result.Value.Count.ShouldBe(0);
            }
        }

        [Fact]
        public async Task When_importing_counties_and_file_provided_is_not_csv_should_return_correct_validation_message()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                string base64Encoded = "YmFzZTY0IGVuY29kZWQgc3RyaW5n";
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);
                var memoryStream = new MemoryStream(System.Convert.FromBase64String(base64Encoded));
                var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "Data", "dummy.jpg"); ;
                var result = await countiesCommandHandler.Handle(new CreateOrUpdateCounties(formFile), new CancellationToken(false));

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBe("No counties to add or update");
            }
        }

        [Fact]
        public async Task When_importing_counties_and_file_is_an_invalid_csv()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);

                StringBuilder sb = new StringBuilder("Id,Code,Name,NumberOfPollingStations,Diaspora,Order");
                sb.Append(Environment.NewLine);
                sb.Append("2,Iasi,Iasi,1,TRUE,");
                sb.Append(Environment.NewLine);
                sb.Append("3,Cluj,Cluj,1,TRUE,1");

                var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                var formFile = new FormFile(new MemoryStream(buffer), 0, buffer.Length, "Data", "dummy.csv");


                var result = await countiesCommandHandler.Handle(new CreateOrUpdateCounties(formFile), new CancellationToken(false));

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldStartWith("Cannot read csv file provided");
                context.Counties.Count().ShouldBe(0);
            }
        }

        [Fact]
        public async Task When_importing_counties_and_has_invalid_county_code_entry_should_not_update_db_and_invalidate_request()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);

                StringBuilder sb = new StringBuilder("Id,Code,Name,NumberOfPollingStations,Diaspora,Order");
                sb.Append(Environment.NewLine);
                sb.Append("2,Iasi11111111111111111111111111111111111111111111111111111111111111,Iasi,1,TRUE,1");
                sb.Append(Environment.NewLine);
                sb.Append("3,Cluj,Cluj,1,TRUE,1");

                var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                var formFile = new FormFile(new MemoryStream(buffer), 0, buffer.Length, "Data", "dummy.csv");


                var result = await countiesCommandHandler.Handle(new CreateOrUpdateCounties(formFile), new CancellationToken(false));

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldStartWith("Invalid county entry found:");
                context.Counties.Count().ShouldBe(0);
            }
        }

        [Fact]
        public async Task When_importing_counties_and_has_invalid_county_name_entry_should_not_update_db_and_invalidate_request()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);

                StringBuilder sb = new StringBuilder("Id,Code,Name,NumberOfPollingStations,Diaspora,Order");
                sb.Append(Environment.NewLine);
                sb.Append("2,Iasi,IasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiIasiI,1,TRUE,1");
                sb.Append(Environment.NewLine);
                sb.Append("3,Cluj,Cluj,1,TRUE,1");

                var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                var formFile = new FormFile(new MemoryStream(buffer), 0, buffer.Length, "Data", "dummy.csv");


                var result = await countiesCommandHandler.Handle(new CreateOrUpdateCounties(formFile), new CancellationToken(false));

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldStartWith("Invalid county entry found:");
                context.Counties.Count().ShouldBe(0);
            }
        }

        [Fact]
        public async Task When_importing_counties_and_has_duplicated_indexes_should_not_update_db_and_invalidate_request()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);

                StringBuilder sb = new StringBuilder("Id,Code,Name,NumberOfPollingStations,Diaspora,Order");
                sb.Append(Environment.NewLine);
                sb.Append("2,Iasi,Iasi_,1,TRUE,1");
                sb.Append(Environment.NewLine);
                sb.Append("2,Cluj,Cluj_,1,TRUE,1");

                var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                var formFile = new FormFile(new MemoryStream(buffer), 0, buffer.Length, "Data", "dummy.csv");


                var result = await countiesCommandHandler.Handle(new CreateOrUpdateCounties(formFile), new CancellationToken(false));

                result.IsFailure.ShouldBeTrue();
                result.Error.ShouldBe("Duplicated id in csv found");
                context.Counties.Count().ShouldBe(0);
            }
        }

        [Fact]
        public async Task When_importing_counties_should_update_counties_by_id()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.Counties.Add(new County
                {
                    Code = "Code1",
                    Diaspora = false,
                    Id = 3,
                    Name = "Name1",
                    Order = 12,
                    NumberOfPollingStations = 14
                });

                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);

                StringBuilder sb = new StringBuilder("Id,Code,Name,NumberOfPollingStations,Diaspora,Order");
                sb.Append(Environment.NewLine);
                sb.Append("3,Cluj,Cluuuuuuuuuj,1,TRUE,1");

                var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                var formFile = new FormFile(new MemoryStream(buffer), 0, buffer.Length, "Data", "dummy.csv");


                var result = await countiesCommandHandler.Handle(new CreateOrUpdateCounties(formFile), new CancellationToken(false));

                result.IsSuccess.ShouldBeTrue();
            
                context.Counties.Count().ShouldBe(1);
                var county = context.Counties.First();
                county.Id.ShouldBe(3);
                county.Code.ShouldBe("Cluj");
                county.Name.ShouldBe("Cluuuuuuuuuj");
                county.NumberOfPollingStations.ShouldBe(1);
                county.Diaspora.ShouldBe(true);
                county.Order.ShouldBe(1);
            }
        }

        [Fact]
        public async Task When_importing_counties_should_insert_records_for_nonexistent_ids()
        {

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);

                StringBuilder sb = new StringBuilder("Id,Code,Name,NumberOfPollingStations,Diaspora,Order");
                sb.Append(Environment.NewLine);
                sb.Append("3,Cluj,Cluuuuuuuuuj,1,TRUE,999");
                sb.Append(Environment.NewLine);
                sb.Append("1,Iasi,Iasi The Best,13,False,5");

                var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                var formFile = new FormFile(new MemoryStream(buffer), 0, buffer.Length, "Data", "dummy.csv");


                var result = await countiesCommandHandler.Handle(new CreateOrUpdateCounties(formFile), new CancellationToken(false));

                result.IsSuccess.ShouldBeTrue();

                context.Counties.Count().ShouldBe(2);
                var counties = context.Counties.ToArray();
                var clujCounty = counties[0];
                clujCounty.Id.ShouldBe(3);
                clujCounty.Code.ShouldBe("Cluj");
                clujCounty.Name.ShouldBe("Cluuuuuuuuuj");
                clujCounty.NumberOfPollingStations.ShouldBe(1);
                clujCounty.Diaspora.ShouldBe(true);
                clujCounty.Order.ShouldBe(999);

                var iasiCounty = counties[1];
                iasiCounty.Id.ShouldBe(1);
                iasiCounty.Code.ShouldBe("Iasi");
                iasiCounty.Name.ShouldBe("Iasi The Best");
                iasiCounty.NumberOfPollingStations.ShouldBe(13);
                iasiCounty.Diaspora.ShouldBe(false);
                iasiCounty.Order.ShouldBe(5);
            }
        }

        [Fact]
        public async Task When_importing_counties_should_insert_records_for_nonexistent_ids_and_update_the_rest()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.Counties.Add(new County
                {
                    Code = "Code1",
                    Diaspora = false,
                    Id = 1,
                    Name = "Name1",
                    Order = 12,
                    NumberOfPollingStations = 14
                });
                context.Counties.Add(new County
                    { Code = "Code2", Diaspora = true, Id = 3, Name = "Name2", Order = 1, NumberOfPollingStations = 2 });
                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object);

                StringBuilder sb = new StringBuilder("Id,Code,Name,NumberOfPollingStations,Diaspora,Order");
                sb.Append(Environment.NewLine);
                sb.Append("1,Cluj,Cluuuuuuuuuj,1,TRUE,999");
                sb.Append(Environment.NewLine);
                sb.Append("3,Iasi,Iasi The Best,13,False,5");

                var buffer = Encoding.UTF8.GetBytes(sb.ToString());
                var formFile = new FormFile(new MemoryStream(buffer), 0, buffer.Length, "Data", "dummy.csv");
                
                var result = await countiesCommandHandler.Handle(new CreateOrUpdateCounties(formFile), new CancellationToken(false));

                result.IsSuccess.ShouldBeTrue();

                context.Counties.Count().ShouldBe(2);
                var counties = context.Counties.ToArray();
                var clujCounty = counties[0];
                clujCounty.Id.ShouldBe(1);
                clujCounty.Code.ShouldBe("Cluj");
                clujCounty.Name.ShouldBe("Cluuuuuuuuuj");
                clujCounty.NumberOfPollingStations.ShouldBe(1);
                clujCounty.Diaspora.ShouldBe(true);
                clujCounty.Order.ShouldBe(999);

                var iasiCounty = counties[1];
                iasiCounty.Id.ShouldBe(3);
                iasiCounty.Code.ShouldBe("Iasi");
                iasiCounty.Name.ShouldBe("Iasi The Best");
                iasiCounty.NumberOfPollingStations.ShouldBe(13);
                iasiCounty.Diaspora.ShouldBe(false);
                iasiCounty.Order.ShouldBe(5);
            }
        }
    }
}