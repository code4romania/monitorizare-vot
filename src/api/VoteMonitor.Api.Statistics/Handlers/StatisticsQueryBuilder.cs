using VoteMonitor.Api.Core.Services;

namespace VoteMonitor.Api.Statistics.Handlers {
    public class StatisticsQueryBuilder
    {
        /// <summary>
        /// The SQL query string that is executed in the db
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// The cache key for the query
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// Appends a statement to the sql query string
        /// </summary>
        /// <param name="statement"></param>
        public void Append(string statement)
        {
            Query = $"{Query} {statement}";
        }

        /// <summary>
        /// Adds an AND condition to the WHERE clause
        /// Filters statistics by FormCode
        /// </summary>
        public void AndFormularFilter(string formular)
        {
            if(!string.IsNullOrEmpty(formular))
            {
                Query = $"{Query} AND f.Code = '{formular}'";
                CacheKey = $"{CacheKey}-{formular}";
            }
        }

        /// <summary>
        /// Adds an AND condition to the WHERE clause
        /// Filters statistics by IdNgo if the ong is admin
        /// </summary>
        public void AndOngFilter(bool organizator, int idONG)
        {
            if(!organizator) 
            {
                Query = $"{Query} AND O.IdNgo = {idONG}";
                CacheKey = $"{CacheKey}-{idONG}";
            }
            else
                CacheKey = $"{CacheKey}-Organizer";
        }

        /// <summary>
        /// Adds a WHERE clause
        /// Filters statistics by IdNgo if the ong is admin
        /// </summary>
        public void WhereOngFilter(bool organizator, int idONG)
        {
            if (!organizator) 
            {
                Query = $"{Query} WHERE O.IdNgo = {idONG}";
                CacheKey = $"{CacheKey}-{idONG}";
            }
            else
                CacheKey = $"{CacheKey}-Organizer";
        }

        /// <summary>
        /// Returns a query with ORDER BY, OFFSET and FETCH clauses
        /// </summary>
        public string GetPaginatedQuery(int page, int pageSize)
        {
            return $"{Query} ORDER BY Value DESC OFFSET {(page - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";
        }
    }
}
