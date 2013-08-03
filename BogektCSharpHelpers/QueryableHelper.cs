using System.Linq;

namespace BogektCSharpHelpers {
    public static class QueryableHelper {
        public static IQueryable<TEntity> ApplySkipAndTake<TEntity>(
              this IQueryable<TEntity> entities
            , int? skip = null
            , int? take = null) {
            if (skip.HasValue)
                entities = entities.Skip(skip.Value);
            if (take.HasValue)
                entities = entities.Take(take.Value);
            return entities;
        }
    }
}