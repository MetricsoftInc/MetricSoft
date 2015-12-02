using System.Data.Entity;

namespace AAMResxEditor.Data
{
	public class AAMContext : DbContext
	{
		public AAMContext() : base("name=AAMContext")
		{
		}

		public DbSet<LOCAL_LANGUAGE> LOCAL_LANGUAGE
		{
			get { return this.Set<LOCAL_LANGUAGE>(); }
		}
	}
}
