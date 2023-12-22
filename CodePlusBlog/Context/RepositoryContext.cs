using CodePlusBlog.Model;
using Microsoft.EntityFrameworkCore;
using ModalLayer.Model;

namespace CodePlusBlog.Context
{
    public class RepositoryContext:DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options): base(options)
        {

        }

        public DbSet<User> user { get; set; }
        public DbSet<Menu> menu { get; set; }
        public DbSet<ContentList> contentlist { get; set; }
        public DbSet<NotesDetail> notesDetails { get; set; }
    }
}
