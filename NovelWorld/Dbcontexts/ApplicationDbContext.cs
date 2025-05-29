using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using NovelWorld.Entities.Auth;
using NovelWorld.Entities.Product;

namespace NovelWorld.Dbcontexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
            
        }
        public DbSet<Novel> Novels { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<UserFollow> UserFollows { get; set; }
        public DbSet<FavoriteNovel> FavoriteNovels { get; set; }
        public DbSet<RevokedToken> RevokedTokens { get; set; }
        public DbSet<Chapter> Chapters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Quan hệ 1-n giữa User và Novel
            modelBuilder.Entity<Novel>()
                .HasOne(t => t.Author)
                .WithMany(u => u.Novels)
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa User -> Xóa Novel của họ

            modelBuilder.Entity<Chapter>()
                .HasOne(n => n.Novel)
                .WithMany(c => c.Chapters)
                .HasForeignKey(n => n.NovelId)
                .OnDelete(DeleteBehavior.Cascade);


            // Quan hệ nhiều-nhiều giữa User và User (Follow)
            modelBuilder.Entity<UserFollow>()
                .HasKey(uf => new { uf.FollowerId, uf.FollowingId });

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollow>()
                .HasOne(uf => uf.Following)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ nhiều-nhiều giữa User và Novel (Favorite Novels)
            modelBuilder.Entity<FavoriteNovel>()
                .HasKey(ft => new { ft.UserId, ft.NovelId });

            modelBuilder.Entity<FavoriteNovel>()
                .HasOne(ft => ft.User)
                .WithMany(u => u.FavoriteNovels)
                .HasForeignKey(ft => ft.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteNovel>()
                .HasOne(ft => ft.Novel)
                .WithMany(t => t.Favorite)
                .HasForeignKey(ft => ft.NovelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RevokedToken>()
                .HasOne(rt => rt.Account)
                .WithMany()  // Nếu Account có danh sách RevokedTokens thì dùng `.WithMany(a => a.RevokedTokens)`
                .HasForeignKey(rt => rt.AccountId)
                .OnDelete(DeleteBehavior.Cascade); // Khi xóa Account, xóa luôn token bị thu hồi

        }
    }
}
