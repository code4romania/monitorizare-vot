using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using VoteMonitor.Api.County.Commands;
using VoteMonitor.Api.County.Handlers;
using VoteMonitor.Api.County.Models;
using VoteMonitor.Api.County.Profiles;
using VoteMonitor.Api.County.Queries;
using VoteMonitor.Entities;
using Xunit;

namespace VotingIrregularities.Tests.CountyApi
{
    public class CountiesCommandHandlerTests
    {
        readonly Mock<ILogger<CountiesCommandHandler>> _fakeLogger = new Mock<ILogger<CountiesCommandHandler>>();
        private readonly DbContextOptions<VoteMonitorContext> _dbContextOptions;
        private readonly IMapper _mapper;
        private readonly MapperConfiguration _configuration = new MapperConfiguration(cfg =>
            cfg.AddProfile(new CountyProfile()));

        public CountiesCommandHandlerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<VoteMonitorContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _mapper = new Mapper(_configuration);
        }

        [Fact]
        public void Should_have_valid_automapper_config()
        {
            _configuration.AssertConfigurationIsValid();
        }

        [Fact]
        public async Task When_loading_2_counties_from_db_for_export()
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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);
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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);
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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);
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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);

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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);

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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);

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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);

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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);

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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);

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
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);

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

        [Fact]
        public async Task When_loading_all_counties_should_return_them_ordered()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.Counties.Add(new County { Code = "Code1", Diaspora = false, Id = 1, Name = "Name1", Order = 12, NumberOfPollingStations = 14 });
                context.Counties.Add(new County { Code = "Code2", Diaspora = true, Id = 2, Name = "Name2", Order = 3, NumberOfPollingStations = 2 });
                context.Counties.Add(new County { Code = "Code3", Diaspora = true, Id = 3, Name = "Name3", Order = 1, NumberOfPollingStations = 2 });
                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);
                var counties =
                    await countiesCommandHandler.Handle(new GetCountiesForExport(), new CancellationToken(false));

                counties.IsSuccess.ShouldBeTrue();

                counties.Value.Count.ShouldBe(3);
                var c1 = counties.Value[0];
                c1.Id.ShouldBe(3);
                c1.Code.ShouldBe("Code3");
                c1.Name.ShouldBe("Name3");
                c1.NumberOfPollingStations.ShouldBe(2);
                c1.Diaspora.ShouldBe(true);
                c1.Order.ShouldBe(1);

                var c2 = counties.Value[1];
                c2.Id.ShouldBe(2);
                c2.Code.ShouldBe("Code2");
                c2.Name.ShouldBe("Name2");
                c2.NumberOfPollingStations.ShouldBe(2);
                c2.Diaspora.ShouldBe(true);
                c2.Order.ShouldBe(3);

                var c3 = counties.Value[2];
                c3.Id.ShouldBe(1);
                c3.Code.ShouldBe("Code1");
                c3.Name.ShouldBe("Name1");
                c3.NumberOfPollingStations.ShouldBe(14);
                c3.Diaspora.ShouldBe(false);
                c3.Order.ShouldBe(12);
            }

        }

        [Fact]
        public async Task When_loading_county_by_nonexistent_id()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.Counties.Add(new County { Code = "Code1", Diaspora = false, Id = 1, Name = "Name1", Order = 12, NumberOfPollingStations = 14 });
                context.Counties.Add(new County { Code = "Code2", Diaspora = true, Id = 2, Name = "Name2", Order = 3, NumberOfPollingStations = 2 });
                context.Counties.Add(new County { Code = "Code3", Diaspora = true, Id = 3, Name = "Name3", Order = 1, NumberOfPollingStations = 2 });
                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);
                var county =
                    await countiesCommandHandler.Handle(new GetCounty(588), new CancellationToken(false));

                county.IsFailure.ShouldBeTrue();
                county.Error.ShouldBe("Could not find county with id = 588");
            }
        }

        [Fact]
        public async Task When_loading_county_by_existent_id()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.Counties.Add(new County { Code = "Code1", Diaspora = false, Id = 1, Name = "Name1", Order = 12, NumberOfPollingStations = 14 });
                context.Counties.Add(new County { Code = "Code2", Diaspora = true, Id = 2, Name = "Name2", Order = 3, NumberOfPollingStations = 2 });
                context.Counties.Add(new County { Code = "Code3", Diaspora = true, Id = 3, Name = "Name3", Order = 1, NumberOfPollingStations = 2 });
                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);
                var county =
                    await countiesCommandHandler.Handle(new GetCounty(2), new CancellationToken(false));

                county.IsSuccess.ShouldBeTrue();
                var c2 = county.Value;
                c2.Id.ShouldBe(2);
                c2.Code.ShouldBe("Code2");
                c2.Name.ShouldBe("Name2");
                c2.NumberOfPollingStations.ShouldBe(2);
                c2.Diaspora.ShouldBe(true);
                c2.Order.ShouldBe(3);
            }
        }

        [Fact]
        public async Task When_updating_county_by_nonexistent_id()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.Counties.Add(new County { Code = "Code1", Diaspora = false, Id = 1, Name = "Name1", Order = 12, NumberOfPollingStations = 14 });
                context.Counties.Add(new County { Code = "Code2", Diaspora = true, Id = 2, Name = "Name2", Order = 3, NumberOfPollingStations = 2 });
                context.Counties.Add(new County { Code = "Code3", Diaspora = true, Id = 3, Name = "Name3", Order = 1, NumberOfPollingStations = 2 });
                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);
                var county =
                    await countiesCommandHandler.Handle(new UpdateCounty(588, new UpdateCountyRequest()), new CancellationToken(false));

                county.IsFailure.ShouldBeTrue();
                county.Error.ShouldBe("Could not find county with id = 588");
            }
        }

        [Fact]
        public async Task When_updating_county_by_existent_id()
        {
            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                context.Counties.Add(new County { Code = "Code1", Diaspora = false, Id = 1, Name = "Name1", Order = 12, NumberOfPollingStations = 14 });
                context.Counties.Add(new County { Code = "Code2", Diaspora = true, Id = 2, Name = "Name2", Order = 3, NumberOfPollingStations = 2 });
                context.Counties.Add(new County { Code = "Code3", Diaspora = true, Id = 3, Name = "Name3", Order = 1, NumberOfPollingStations = 2 });
                context.SaveChanges();
            }

            using (var context = new VoteMonitorContext(_dbContextOptions))
            {
                var countiesCommandHandler = new CountiesCommandHandler(context, _fakeLogger.Object, _mapper);
                var updateCountyModel = new UpdateCountyRequest(){
                    Name = "Super Iasi",
                    Code = "IS",
                    Order = 33,
                    Diaspora = false,
                    NumberOfPollingStations = 767
                };
                var county =
                    await countiesCommandHandler.Handle(new UpdateCounty(2,updateCountyModel), new CancellationToken(false));

                county.IsSuccess.ShouldBeTrue();
                context.Counties.Count().ShouldBe(3);
                var updatedCounty = await context.Counties.FirstOrDefaultAsync(x => x.Id == 2);

                updatedCounty.ShouldNotBeNull();
                updatedCounty.Id.ShouldBe(2);
                updatedCounty.Code.ShouldBe("IS");
                updatedCounty.Name.ShouldBe("Super Iasi");
                updatedCounty.NumberOfPollingStations.ShouldBe(767);
                updatedCounty.Diaspora.ShouldBe(false);
                updatedCounty.Order.ShouldBe(33);
            }
        }

    }
}