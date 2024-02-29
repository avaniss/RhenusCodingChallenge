using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Infrastructure.Common
{
    public class IdGenerator : ValueGenerator<long>
    {
        private long _current;
        public override bool GeneratesTemporaryValues => false;

        public override long Next(EntityEntry entry) => Interlocked.Increment(ref _current);
    }
}
