using DddBlazorDemo.Billing.Domain;
using DddBlazorDemo.Documents.Domain;
using DddBlazorDemo.Domain.Billing;
using DddBlazorDemo.Infrastructure.Outbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DddBlazorDemo.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="modelBuilder"></param>
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
            b.Property(x => x.Id).ValueGeneratedNever();

            b.Property(x => x.Number)
                .HasConversion(invoiceNumberConverter)
                .HasMaxLength(32);

            b.Property(x => x.ProviderEan)
                .HasConversion(eanConverter)
                .HasMaxLength(20);

            b.Property(x => x.Currency)
                .HasConversion(currencyConverter)
                .HasMaxLength(3);

            // inside the Invoice block:
            b.Property(x => x.CustomerId);
            b.HasIndex(x => x.CustomerId);
            b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId); // FK only, no navigation
            b.OwnsOne(x => x.BillTo, ob =>
            {
                ob.Property(p => p.Name).HasColumnName("BillToName").HasMaxLength(BillTo.MaxNameLength);
                ob.Property(p => p.Gln).HasConversion(eanConverter).HasColumnName("BillToGln").HasMaxLength(20);
            });

            b.Property(x => x.Status);

            b.Ignore(x => x.Lines);

            b.OwnsMany<InvoiceLine>("_lines", lb =>
            {
                lb.ToTable("Billing_InvoiceLines");
                lb.WithOwner().HasForeignKey("InvoiceId");
                lb.HasKey(x => x.Id);
                // The domain assigns line ids. Without this EF treats a pre-set Guid key as
                // "generated and already set" and tracks new lines as Modified, not Added.
                lb.Property(x => x.Id).ValueGeneratedNever();

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
            b.Property(x => x.Id).ValueGeneratedNever();

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

        modelBuilder.Entity<Customer>(b =>
        {
            b.ToTable("Billing_Customers");
            b.HasKey(x => x.Id);
            b.Property(x => x.Id).ValueGeneratedNever();
            b.Property(x => x.Name).HasMaxLength(200);
            b.Property(x => x.Gln).HasConversion(eanConverter).HasMaxLength(20);
        });

        base.OnModelCreating(modelBuilder);
    }
}
