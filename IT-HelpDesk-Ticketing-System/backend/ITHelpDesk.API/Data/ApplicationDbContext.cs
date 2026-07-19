using ITHelpDesk.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ITHelpDesk.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Priority> Priorities { get; set; }
    public DbSet<Status> Statuses { get; set; }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketComment> TicketComments { get; set; }
    public DbSet<TicketAttachment> TicketAttachments { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<TicketAssignment> TicketAssignments { get; set; }
    public DbSet<KnowledgeBaseArticle> KnowledgeBaseArticles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ticket -> CreatedByUser
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.CreatedByUser)
            .WithMany(u => u.CreatedTickets)
            .HasForeignKey(t => t.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ticket -> AssignedToUser
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.AssignedToUser)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // KnowledgeBaseArticle -> CreatedByUser
        modelBuilder.Entity<KnowledgeBaseArticle>()
            .HasOne(k => k.CreatedByUser)
            .WithMany(u => u.CreatedArticles)
            .HasForeignKey(k => k.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // KnowledgeBaseArticle -> ApprovedByUser
        modelBuilder.Entity<KnowledgeBaseArticle>()
            .HasOne(k => k.ApprovedByUser)
            .WithMany(u => u.ApprovedArticles)
            .HasForeignKey(k => k.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TicketAssignment -> AssignedToUser
        modelBuilder.Entity<TicketAssignment>()
            .HasOne(t => t.AssignedToUser)
            .WithMany(u => u.AssignedTicketsHistory)
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TicketAssignment -> AssignedByUser
        modelBuilder.Entity<TicketAssignment>()
            .HasOne(t => t.AssignedByUser)
            .WithMany(u => u.AssignedByHistory)
            .HasForeignKey(t => t.AssignedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Role>().HasData(
new Role
{
    Id = 1,
    Name = "Admin",
    Description = "System Administrator"
},
new Role
{
    Id = 2,
    Name = "Manager",
    Description = "IT Manager"
},
new Role
{
    Id = 3,
    Name = "IT Support Agent",
    Description = "IT Support Staff"
},
new Role
{
    Id = 4,
    Name = "Employee",
    Description = "Regular Employee"
}
);
        modelBuilder.Entity<Category>().HasData(
         new Category { Id = 1, Name = "Hardware", Description = "Hardware related issues" },
         new Category { Id = 2, Name = "Software", Description = "Software related issues" },
         new Category { Id = 3, Name = "Network", Description = "Network related issues" },
         new Category { Id = 4, Name = "Email", Description = "Email related issues" },
         new Category { Id = 5, Name = "Access Request", Description = "User access requests" },
         new Category { Id = 6, Name = "Other", Description = "Other support requests" }
     );
        modelBuilder.Entity<Priority>().HasData(
            new Priority { Id = 1, Name = "Low", DisplayOrder = 1 },
            new Priority { Id = 2, Name = "Medium", DisplayOrder = 2 },
            new Priority { Id = 3, Name = "High", DisplayOrder = 3 },
            new Priority { Id = 4, Name = "Critical", DisplayOrder = 4 }
        );
        modelBuilder.Entity<Status>().HasData(
            new Status { Id = 1, Name = "Open", DisplayOrder = 1 },
            new Status { Id = 2, Name = "In Progress", DisplayOrder = 2 },
            new Status { Id = 3, Name = "Pending", DisplayOrder = 3 },
            new Status { Id = 4, Name = "Resolved", DisplayOrder = 4 },
            new Status { Id = 5, Name = "Closed", DisplayOrder = 5 }
        );
    }
}