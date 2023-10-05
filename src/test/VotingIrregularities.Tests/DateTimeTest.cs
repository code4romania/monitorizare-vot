using Shouldly;
using Xunit;

namespace VotingIrregularities.Tests;

public class DateTimeTest
{
    [Theory]
    [MemberData(nameof(DateTimeComparisonTestData))]
    public void SanityCheck(DateTime? first, DateTime? second, DateTime? expected)
    {
        var result = (first?? DateTime.MinValue) > (second ?? DateTime.MinValue) ? first : second;

        result.ShouldBe(expected);
    }

    private static DateTime EarliestDate = new(2020, 11, 12);
    private static DateTime LatestDate = new(2020, 11, 13);
    public static IEnumerable<object[]> DateTimeComparisonTestData =>
        new List<object[]>
        {
            new object[] {EarliestDate,EarliestDate,EarliestDate },
            new object[] {EarliestDate,LatestDate , LatestDate},
            new object[] { LatestDate, EarliestDate, LatestDate},
            new object[] { null, LatestDate, LatestDate},
            new object[] { LatestDate, null, LatestDate},
            new object[] { null, null, null},
        };
}
