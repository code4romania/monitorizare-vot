using LinqKit;
using System.Linq.Expressions;

namespace VoteMonitor.Entities;

public static class EfBuilderExtensions
{
    /// <summary>
    /// super simple and dumb translation of .Contains because is not supported pe EF plus
    /// this translates to contains in EF SQL
    /// </summary>
    /// <param name="source"></param>
    /// <param name="contains"></param>
    /// <returns></returns>
    public static IQueryable<Answer> WhereRaspunsContains(this IQueryable<Answer> source, IList<int> contains)
    {
        var ors = contains
            .Aggregate<int, Expression<Func<Answer, bool>>>(null, (expression, id) =>
                expression == null
                    ? (a => a.OptionAnswered.IdQuestion == id)
                    : expression.Or(a => a.OptionAnswered.IdQuestion == id));

        return source.Where(ors);
    }    
    /// <summary>
    /// super simple and dumb translation of .Contains because is not supported pe EF plus
    /// this translates to contains in EF SQL
    /// </summary>
    /// <param name="source"></param>
    /// <param name="contains"></param>
    /// <returns></returns>
    public static IQueryable<AnswerCorrupted> WhereRaspunsContains(this IQueryable<AnswerCorrupted> source, IList<int> contains)
    {
        var ors = contains
            .Aggregate<int, Expression<Func<AnswerCorrupted, bool>>>(null, (expression, id) =>
                expression == null
                    ? (a => a.OptionAnswered.IdQuestion == id)
                    : expression.Or(a => a.OptionAnswered.IdQuestion == id));

        return source.Where(ors);
    }
}
