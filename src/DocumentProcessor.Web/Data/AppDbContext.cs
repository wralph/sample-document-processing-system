using System;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using DocumentProcessor.Web.Models;

namespace DocumentProcessor.Web.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    static AppDbContext()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Document>(entity =>
        {
            entity.ToTable("documents", "dps_dbo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FileName).HasColumnName("filename");
            entity.Property(e => e.OriginalFileName).HasColumnName("originalfilename");
            entity.Property(e => e.FileExtension).HasColumnName("fileextension");
            entity.Property(e => e.FileSize).HasColumnName("filesize");
            entity.Property(e => e.ContentType).HasColumnName("contenttype");
            entity.Property(e => e.StoragePath).HasColumnName("storagepath");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.UploadedBy).HasColumnName("uploadedby");
            entity.Property(e => e.IsDeleted).HasColumnName("isdeleted").HasConversion<int>();

            entity.HasQueryFilter(d => !d.IsDeleted);
        });
    }
}
