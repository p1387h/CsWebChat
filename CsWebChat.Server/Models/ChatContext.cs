using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CsWebChat.Server.Models
{
    public class ChatContext : DbContext
    {
        public ChatContext(DbContextOptions<ChatContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey("Name")
                .HasName("PK_Name");

            modelBuilder.Entity<Message>()
                .HasKey("MessageId")
                .HasName("PK_Message");
            modelBuilder.Entity<Message>()
                .HasOne<User>("Sender")
                .WithMany("MessageSent");
            modelBuilder.Entity<Message>()
                .HasOne<User>("Receiver")
                .WithMany("MessageReceived");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<User> User { get; set; }
        public DbSet<Message> Message { get; set; }
    }
}
