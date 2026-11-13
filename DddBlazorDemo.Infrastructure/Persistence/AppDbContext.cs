using DddBlazorDemo.Billing.Domain;
using DddBlazorDemo.Documents.Domain;
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

        var customerIdConverter = new ValueConverter<CustomerId, Guid>(
            v => v.Value,
            v => new CustomerId(v));

        var customerNameConverter = new ValueConverter<CustomerName, string>(
            v => v.Value,
            v => new CustomerName(v));

        var vatNumberConverter = new ValueConverter<VatNumber, string>(
            v => v.Value,
            v => new VatNumber(v));

        var emailConverter = new ValueConverter<EmailAddress, string>(
            v => v.Value,
            v => new EmailAddress(v));

        var phoneConverter = new ValueConverter<PhoneNumber, string>(
            v => v.Value,
            v => new PhoneNumber(v));

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
            b.Property(x => x.CustomerId).HasConversion(customerIdConverter);
            b.HasIndex(x => x.CustomerId);
            b.HasOne<Customer>().WithMany().HasForeignKey(x => x.CustomerId); // FK only, no navigation
            // Snapshot of the customer at creation time; stored in the invoice row.
            b.OwnsOne(x => x.BillTo, ob =>
            {
                ob.Property(p => p.Name)
                    .HasConversion(customerNameConverter)
                    .HasColumnName("BillToName")
                    .HasMaxLength(CustomerName.MaxLength);

                ob.Property(p => p.VatNumber)
                    .HasConversion(vatNumberConverter)
                    .HasColumnName("BillToVatNumber")
                    .HasMaxLength(VatNumber.MaxLength);

                ob.OwnsOne(p => p.Address, ab =>
                {
                    ab.Property(a => a.Street).HasColumnName("BillToStreet").HasMaxLength(PostalAddress.MaxStreetLength);
                    ab.Property(a => a.City).HasColumnName("BillToCity").HasMaxLength(PostalAddress.MaxCityLength);
                    ab.Property(a => a.PostalCode).HasColumnName("BillToPostalCode").HasMaxLength(PostalAddress.MaxPostalCodeLength);
                    ab.Property(a => a.CountryCode).HasColumnName("BillToCountryCode").HasMaxLength(PostalAddress.CountryCodeLength);
                });
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
            b.Property(x => x.Id).HasConversion(customerIdConverter).ValueGeneratedNever();

            // Single-value value objects: one column each, via converter.
            b.Property(x => x.Name)
                .HasConversion(customerNameConverter)
                .HasMaxLength(CustomerName.MaxLength);

            b.Property(x => x.VatNumber)
                .HasConversion(vatNumberConverter)
                .HasMaxLength(VatNumber.MaxLength);
            b.HasIndex(x => x.VatNumber).IsUnique();

            b.Property(x => x.Email)
                .HasConversion(emailConverter)
                .HasMaxLength(EmailAddress.MaxLength);

            b.Property(x => x.Phone)
                .HasConversion(phoneConverter)
                .HasMaxLength(PhoneNumber.MaxLength)
                .IsRequired(false);

            // Multi-value value object: owned, columns in the same table.
            b.OwnsOne(x => x.Address, ab =>
            {
                ab.Property(a => a.Street).HasColumnName("Street").HasMaxLength(PostalAddress.MaxStreetLength);
                ab.Property(a => a.City).HasColumnName("City").HasMaxLength(PostalAddress.MaxCityLength);
                ab.Property(a => a.PostalCode).HasColumnName("PostalCode").HasMaxLength(PostalAddress.MaxPostalCodeLength);
                ab.Property(a => a.CountryCode).HasColumnName("CountryCode").HasMaxLength(PostalAddress.CountryCodeLength);
            });

            b.Property(x => x.IsActive);
        });

        base.OnModelCreating(modelBuilder);
    }
}
