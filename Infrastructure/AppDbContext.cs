using DddBlazorDemo.Billing.Domain;
using DddBlazorDemo.Documents.Domain;
using DddBlazorDemo.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DddBlazorDemo.Infrastructure;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var invoiceNumberConverter = new ValueConverter<InvoiceNumber, string>(
            v => v.Value,
            v => new InvoiceNumber(v));

        var eanConverter = new ValueConverter<EanNumber, string>(
            v => v.Value,
            v => new EanNumber(v));

        var currencyConverter = new ValueConverter<Currency, string>(
            v => v.Code,
            v => new Currency(v));

        modelBuilder.Entity<Invoice>(b =>
        {
            b.ToTable("Billing_Invoices");
            b.HasKey(x => x.Id);

            b.Property(x => x.Number)
                .HasConversion(invoiceNumberConverter)
                .HasMaxLength(32);

            b.Property(x => x.ProviderEan)
                .HasConversion(eanConverter)
                .HasMaxLength(20);

            b.Property(x => x.Currency)
                .HasConversion(currencyConverter)
                .HasMaxLength(3);

            b.Property(x => x.Status);

            b.Ignore(x => x.Lines);

            b.OwnsMany<InvoiceLine>("_lines", lb =>
            {
                lb.ToTable("Billing_InvoiceLines");
                lb.WithOwner().HasForeignKey("InvoiceId");
                lb.HasKey(x => x.Id);

                lb.Property(x => x.Description).HasMaxLength(200);

                lb.OwnsOne(x => x.Amount, mb =>
                {
                    mb.Property(p => p.Amount).HasColumnName("Amount");

                    mb.Property(p => p.Currency)
                        .HasConversion(currencyConverter)
                        .HasColumnName("Currency")
                        .HasMaxLength(3);
                });
            });

            b.Navigation("_lines").UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Document>(b =>
        {
            b.ToTable("Docs_Documents");
            b.HasKey(x => x.Id);

            b.Property(x => x.FileName).HasMaxLength(128);
            b.Property(x => x.WorkflowStatus);
            b.Property(x => x.IsPdfUpToDate);
            b.Property(x => x.CreatedAtUtc);

            b.HasIndex(x => x.InvoiceId);
        });

        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.ToTable("OutboxMessages");
            b.HasKey(x => x.Id);

            b.Property(x => x.Type).HasMaxLength(200);
            b.Property(x => x.PayloadJson);

            b.Property(x => x.OccurredAtUtc);
            b.Property(x => x.CreatedAtUtc);
            b.Property(x => x.ProcessedAtUtc);

            b.Property(x => x.AttemptCount);
            b.Property(x => x.LastAttemptUtc);

            b.Property(x => x.LastError).HasMaxLength(2000);

            b.Property(x => x.LockedUntilUtc);
            b.Property(x => x.LockOwner).HasMaxLength(128);

            b.HasIndex(x => x.ProcessedAtUtc);
            b.HasIndex(x => x.OccurredAtUtc);
            b.HasIndex(x => x.LockedUntilUtc);
        });

        base.OnModelCreating(modelBuilder);
    }
}
