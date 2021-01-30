namespace VoteMonitor.Api.Statistics.Handlers
{
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
        public void AndFormCodeFilter(string formCode)
        {
            if (!string.IsNullOrEmpty(formCode))
            {
                Query = $"{Query} AND f.Code = '{formCode}'";
                CacheKey = $"{CacheKey}-{formCode}";
            }
        }

        /// <summary>
        /// Adds an AND condition to the WHERE clause
        /// Filters statistics by gnoId if the ngo is admin
        /// </summary>
        public void AndOngFilter(bool isOrganizer, int ngoId)
        {
            if (!isOrganizer)
            {
                Query = $"{Query} AND O.IdNgo = {ngoId}";
                CacheKey = $"{CacheKey}-{ngoId}";
            }
            else
            {
                CacheKey = $"{CacheKey}-Organizer";
            }
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
