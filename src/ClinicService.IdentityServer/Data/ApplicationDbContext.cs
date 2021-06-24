using ClinicService.IdentityServer.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinicService.IdentityServer.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<FunctionCommand>()
                .HasKey(k => new { k.FunctionId, k.CommandId });

            builder.Entity<Permission>()
                .HasKey(k => new { k.FunctionId, k.CommandId, k.RoleId });
        }

        public DbSet<Appointment> Appointments { get; set; }

        public DbSet<ClinicBranch> ClinicBranches { get; set; }

        public DbSet<Command> Commands { get; set; }

        public DbSet<Function> Functions { get; set; }

        public DbSet<FunctionCommand> FunctionCommands { get; set; }

        public DbSet<MedicalExamination> MedicalExaminations { get; set; }

        public DbSet<MedicalExaminationAttachment> MedicalExaminationAttachments { get; set; }

        public DbSet<MedicalExaminationDetail> MedicalExaminationDetails { get; set; }

        public DbSet<MedicalService> MedicalServices { get; set; }

        public DbSet<MedicalServiceAttachment> MedicalServiceAttachments { get; set; }

        public DbSet<MedicalServiceType> MedicalServiceTypes { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<PaymentMethod> PaymentMethods { get; set; }

        public DbSet<PaymentMethodAttachment> PaymentMethodAttachments { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<Prescription> Prescriptions { get; set; }

        public DbSet<Reappointment> Reappointments { get; set; }

        public DbSet<Specialty> Specialties { get; set; }

        public DbSet<SpecialtyAttachment> SpecialtyAttachments { get; set; }

        public DbSet<StatusCategory> StatusCategories { get; set; }

        public DbSet<WebsiteSection> WebsiteSections { get; set; }

        public DbSet<WebsiteSectionAttachment> WebsiteSectionAttachments { get; set; }
    }
}
