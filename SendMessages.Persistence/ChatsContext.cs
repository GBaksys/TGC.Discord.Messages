﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SendMessages.Domain;

namespace SendMessages.Persistence
{
    public partial class ChatsContext : DbContext
    {
        private readonly string _connectionString;

        public ChatsContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        //public ChatsContext(DbContextOptions<ChatsContext> options)
        //    : base(options)
        //{
        //}

        public virtual DbSet<EsoChat> EsoChats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EsoChat>(entity =>
            {
                entity.HasKey(e => e.TimeStamp)
                    .HasName("PK__esochats__6EBFE36640EDDD91");

                entity.Property(e => e.TimeStamp)
                    .HasColumnType("datetime")
                    .HasColumnName("time_stamp");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("chat_text")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Channel)
                    .IsRequired()
                    .HasColumnName("chat_channel")
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.EsoUserId)
                    .IsRequired()
                    .HasColumnName("eso_userid")
                    .HasMaxLength(25)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}