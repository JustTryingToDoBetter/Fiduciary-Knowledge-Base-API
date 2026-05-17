using fiduciary.Models;
using Microsoft.EntityFrameworkCore;

namespace fiduciary.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {

    }

    public DbSet<KnowledgeArticle> KnowledgeArticle => Set<KnowledgeArticle>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KnowledgeArticle>(entity => 
        {
            entity.HasKey(article => article.Id);

            entity.Property(article => article.Title)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(article => article.Summary)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(article => article.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(article => article.Tags)
                .IsRequired()
                .HasMaxLength(500);
            
            entity.Property(article => article.Source)
                .HasMaxLength(500);

            entity.HasIndex(article => article.Title);
            entity.HasIndex(article => article.Category);

        });


        modelBuilder.Entity<KnowledgeArticle>().HasData(
            new KnowledgeArticle
            {
                Id = 1,
                Title = "What is fiduciary duty?",
                Summary = "A fiduciary duty is a responsibility to act in another person's best interests.",
                Content = "A fiduciary must act with care, honesty, loyalty, and good faith when managing another person's assets or interests.",
                Category = "Fiduciary",
                Tags = "fiduciary,duty,trust,compliance",
                Source = "Internal starter note",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new KnowledgeArticle
            {
                Id = 2,
                Title = "What is deceased estate administration?",
                Summary = "The process of managing and distributing a deceased person's estate.",
                Content = "Deceased estate administration involves identifying assets and liabilities, working with executors, preparing documentation, and distributing the estate according to legal requirements.",
                Category = "Estate Administration",
                Tags = "estate,deceased,executor,assets",
                Source = "Internal starter note",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new KnowledgeArticle
            {
                Id = 3,
                Title = "What is trust administration?",
                Summary = "The management of a trust according to its deed and applicable law.",
                Content = "Trust administration includes managing trustees, beneficiaries, trust assets, documentation, reporting, and compliance obligations.",
                Category = "Trust Administration",
                Tags = "trust,trustees,beneficiaries,compliance",
                Source = "Internal starter note",
                CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}