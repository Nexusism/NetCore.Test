
//using Microsoft.EntityFrameworkCore;

//namespace NetCore.Databases.Data
   
//{
//    public partial class DBFirstDBContext : DbContext
//    {
//        public DBFirstDBContext()
//        {
//        }

//        public DBFirstDBContext(DbContextOptions<DBFirstDBContext> options)
//            : base(options)
//        {
//        }

//        // DB 테이블 리스트 지정
//        public virtual DbSet<User> Users { get; set; } = null!;
//        public virtual DbSet<UserRole> UserRoles { get; set; } = null!;
//        public virtual DbSet<UserRolesByUser> UserRolesByUsers { get; set; } = null!;

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//                optionsBuilder.UseSqlServer("Name=ConnectionStrings:DBFirstDBConnection");
//            }
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<User>(entity =>
//            {
//                entity.ToTable("User");

//                entity.HasIndex(e => e.UserEmail, "IX_User_UserEmail")
//                    .IsUnique();

//                entity.Property(e => e.UserId)
//                    .HasMaxLength(50)
//                    .IsUnicode(false);

//                entity.Property(e => e.IsMemberShipWithDrawn)
//                    .IsRequired()
//                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

//                entity.Property(e => e.JoinedUtcDate).HasDefaultValueSql("(sysutcdatetime())");

//                entity.Property(e => e.Password).HasMaxLength(130);

//                entity.Property(e => e.UserEmail)
//                    .HasMaxLength(320)
//                    .IsUnicode(false);

//                entity.Property(e => e.UserName).HasMaxLength(100);
//            });

//            modelBuilder.Entity<UserRole>(entity =>
//            {
//                entity.HasKey(e => e.RoleId);

//                entity.ToTable("UserRole");

//                entity.Property(e => e.RoleId)
//                    .HasMaxLength(50)
//                    .IsUnicode(false);

//                entity.Property(e => e.ModifiedUtcDate).HasDefaultValueSql("(sysutcdatetime())");

//                entity.Property(e => e.RoleName).HasMaxLength(100);
//            });

//            modelBuilder.Entity<UserRolesByUser>(entity =>
//            {
//                entity.HasKey(e => new { e.UserId, e.RoleId });

//                entity.ToTable("UserRolesByUser");

//                entity.Property(e => e.UserId)
//                    .HasMaxLength(50)
//                    .IsUnicode(false);

//                entity.Property(e => e.RoleId)
//                    .HasMaxLength(50)
//                    .IsUnicode(false);

//                entity.Property(e => e.OwnedUtcDate).HasDefaultValueSql("(sysutcdatetime())");

//                entity.HasOne(d => d.Role)
//                    .WithMany(p => p.UserRolesByUsers)
//                    .HasForeignKey(d => d.RoleId);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.UserRolesByUsers)
//                    .HasForeignKey(d => d.UserId);
//            });

//            OnModelCreatingPartial(modelBuilder);
//        }

//        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//    }
//}
