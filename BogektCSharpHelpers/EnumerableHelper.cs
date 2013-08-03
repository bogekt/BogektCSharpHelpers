using System.Collections.Generic;
using System.Linq;

namespace BogektCSharpHelpers {
    public static class EnumerableHelper {
        public static IEnumerable<TEntity> ApplySkipAndTake<TEntity>(
              this IEnumerable<TEntity> entities
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