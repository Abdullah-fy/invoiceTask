using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace itRoot.Models;

public partial class dbContext : DbContext
{
    public dbContext()
    {
    }

    public dbContext(DbContextOptions<dbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<inVoice> inVoices { get; set; }

    public virtual DbSet<inVoiceItem> inVoiceItems { get; set; }

    public virtual DbSet<user> users { get; set; }
    public DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ItRootsTask;Integrated Security=True;Trust Server Certificate=True;Command Timeout=300");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<inVoice>(entity =>
        {
            entity.HasKey(e => e.inVoiceId).HasName("PK__inVoices__DFA984BA43A08341");

            entity.Property(e => e.inVoiceDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.user).WithMany(p => p.inVoices).HasConstraintName("FK__inVoices__userId__3D5E1FD2");
        });

        modelBuilder.Entity<inVoiceItem>(entity =>
        {
            entity.HasKey(e => e.itemId).HasName("PK__inVoiceI__56A128AA11AC13A7");

            entity.Property(e => e.lineTotal).HasComputedColumnSql("([quantity]*[price])", false);

            entity.HasOne(d => d.inVoice).WithMany(p => p.inVoiceItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__inVoiceIt__inVoi__403A8C7D");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.userId).HasName("PK__user__CB9A1CFF5B6F3451");

            entity.Property(e => e.isConfirmed).HasDefaultValue(false);
        });
        modelBuilder.Entity<EmailConfirmationToken>(entity =>
        {
            entity.ToTable("EmailConfirmationToken");

            entity.HasKey(e => e.tokenId);
            entity.Property(e => e.tokenId)
                .HasColumnName("tokenId")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.token)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnType("varchar(255)")
                .HasColumnName("token");

            entity.Property(e => e.userId)
                .IsRequired()
                .HasColumnName("userId");

            entity.Property(e => e.createdAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt")
                .HasDefaultValueSql("(getdate())");

            entity.Property(e => e.expiresAt)
                .HasColumnType("datetime")
                .HasColumnName("expiresAt");

            entity.Property(e => e.isUsed)
                .HasColumnName("isUsed")
                .HasDefaultValue(false);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.userId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_EmailConfirmationToken_Users");

            entity.HasIndex(e => e.token)
                .HasDatabaseName("IX_EmailConfirmationToken_Token");
            entity.HasIndex(e => e.userId)
                .HasDatabaseName("IX_EmailConfirmationToken_UserId");
        });
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
