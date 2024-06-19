using Microsoft.EntityFrameworkCore;

namespace GiftGiver
{
    public partial class giftgiverContext : DbContext
    {
        public giftgiverContext()
        {
        }

        public giftgiverContext(DbContextOptions<giftgiverContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Администратор> Администраторs { get; set; } = null!;
        public virtual DbSet<Желаемое> Желаемоеs { get; set; } = null!;
        public virtual DbSet<Лента> Лентаs { get; set; } = null!;
        public virtual DbSet<Подарки> Подаркиs { get; set; } = null!;
        public virtual DbSet<Пользователь> Пользовательs { get; set; } = null!;
        public virtual DbSet<Роли> Ролиs { get; set; } = null!;
        public virtual DbSet<Статистика> Статистикаs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=DESKTOP-4GCEH3I;Database=giftgiver;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Администратор>(entity =>
            {
                entity.ToTable("Администратор");

                entity.Property(e => e.АдминистраторId).HasColumnName("АдминистраторID");

                entity.Property(e => e.Логин).HasMaxLength(100);

                entity.Property(e => e.Пароль).HasMaxLength(100);

                entity.Property(e => e.РолиId).HasColumnName("РолиID");

                entity.HasOne(d => d.Роли)
                    .WithMany(p => p.Администраторs)
                    .HasForeignKey(d => d.РолиId)
                    .HasConstraintName("FK__Администр__РолиI__398D8EEE");
            });

            modelBuilder.Entity<Желаемое>(entity =>
            {
                entity.ToTable("Желаемое");

                entity.Property(e => e.ЖелаемоеId).HasColumnName("ЖелаемоеID");

                entity.Property(e => e.ПодаркиId).HasColumnName("ПодаркиID");

                entity.Property(e => e.ПользовательId).HasColumnName("ПользовательID");

                entity.HasOne(d => d.Подарки)
                    .WithMany(p => p.Желаемоеs)
                    .HasForeignKey(d => d.ПодаркиId)
                    .HasConstraintName("FK__Желаемое__Подарк__440B1D61");

                entity.HasOne(d => d.Пользователь)
                    .WithMany(p => p.Желаемоеs)
                    .HasForeignKey(d => d.ПользовательId)
                    .HasConstraintName("FK__Желаемое__Пользо__4316F928");
            });

            modelBuilder.Entity<Лента>(entity =>
            {
                entity.ToTable("Лента");

                entity.Property(e => e.ЛентаId).HasColumnName("ЛентаID");

                entity.Property(e => e.ВремяЗапроса).HasColumnType("datetime");

                entity.Property(e => e.ПодаркиId).HasColumnName("ПодаркиID");

                entity.Property(e => e.ПользовательId).HasColumnName("ПользовательID");

                entity.HasOne(d => d.Подарки)
                    .WithMany(p => p.Лентаs)
                    .HasForeignKey(d => d.ПодаркиId)
                    .HasConstraintName("FK__Лента__ПодаркиID__47DBAE45");

                entity.HasOne(d => d.Пользователь)
                    .WithMany(p => p.Лентаs)
                    .HasForeignKey(d => d.ПользовательId)
                    .HasConstraintName("FK__Лента__Пользоват__46E78A0C");
            });

            modelBuilder.Entity<Подарки>(entity =>
            {
                entity.ToTable("Подарки");

                entity.Property(e => e.ПодаркиId).HasColumnName("ПодаркиID");

                entity.Property(e => e.Жанр).HasMaxLength(100);

                entity.Property(e => e.Изображение).HasColumnType("image");

                entity.Property(e => e.Наименование).HasMaxLength(100);

                entity.Property(e => e.Цена).HasColumnType("money");
            });

            modelBuilder.Entity<Пользователь>(entity =>
            {
                entity.ToTable("Пользователь");

                entity.Property(e => e.ПользовательId).HasColumnName("ПользовательID");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Возраст).HasColumnType("date");

                entity.Property(e => e.ДатаПосещения).HasColumnType("datetime");

                entity.Property(e => e.Изображение).HasColumnType("image");

                entity.Property(e => e.Логин).HasMaxLength(100);

                entity.Property(e => e.Пароль).HasMaxLength(100);

                entity.Property(e => e.РолиId).HasColumnName("РолиID");

                entity.Property(e => e.Фио)
                    .HasMaxLength(100)
                    .HasColumnName("ФИО");

                entity.HasOne(d => d.Роли)
                    .WithMany(p => p.Пользовательs)
                    .HasForeignKey(d => d.РолиId)
                    .HasConstraintName("FK__Пользоват__РолиI__3C69FB99");
            });

            modelBuilder.Entity<Роли>(entity =>
            {
                entity.ToTable("Роли");

                entity.Property(e => e.РолиId).HasColumnName("РолиID");

                entity.Property(e => e.Наименование).HasMaxLength(100);
            });

            modelBuilder.Entity<Статистика>(entity =>
            {
                entity.ToTable("Статистика");

                entity.Property(e => e.СтатистикаId).HasColumnName("СтатистикаID");

                entity.Property(e => e.Месяц).HasColumnType("date");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
