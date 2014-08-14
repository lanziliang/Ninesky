using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Ninesky.Models;

namespace Ninesky.DAL
{
    /// <summary>
    /// 数据上下文
    /// </summary>
    public class NineskyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoleRelation> UserRoleRelations { get; set; }
        public DbSet<UserConfig> Userconfigs { get; set; }
        public NineskyDbContext()
            : base("DefaultConnection")
        {
            Database.CreateIfNotExists();
        }
    }
}
