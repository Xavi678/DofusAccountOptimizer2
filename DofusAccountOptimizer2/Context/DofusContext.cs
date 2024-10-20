﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DofusAccountOptimizer2.Models;

namespace DofusAccountOptimizer2.Context
{
    public partial class DofusContext : DbContext
    {
        public DofusContext()
        {
        }

        public DofusContext(DbContextOptions<DofusContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Classe> Classes { get; set; } = null!;
        public virtual DbSet<Composition> Compositions { get; set; } = null!;
        public virtual DbSet<Configuracio> Configuracios { get; set; } = null!;
        public virtual DbSet<Personatge> Personatges { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite("Data Source=C:\\Users\\ASUS\\AppData\\Local\\DofusAccountOptimizer\\Dofus.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Classe>(entity =>
            {
                entity.ToTable("Classe");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Foto).HasColumnName("FOTO");

                entity.Property(e => e.Nom).HasColumnName("NOM");
            });

            modelBuilder.Entity<Composition>(entity =>
            {
                entity.ToTable("COMPOSITIONS");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.Nom)
                    .HasColumnName("NOM");
                    //.HasDefaultValueSql("\"DEFAULT\"");
            });

            modelBuilder.Entity<Configuracio>(entity =>
            {
                entity.ToTable("CONFIGURACIO");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.EnableKeyboard)
                    .HasColumnName("ENABLE_KEYBOARD")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.EnableMouse)
                    .HasColumnName("ENABLE_MOUSE")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.KeyCodes)
                    .HasColumnType("NVARCHAR(100)")
                    .HasColumnName("KEY_CODES");

                entity.Property(e => e.Language)
                    .HasColumnType("VARCHAR(2)")
                    .HasColumnName("LANGUAGE")
                    .HasDefaultValueSql("'en'");

                entity.Property(e => e.LastCompositionId)
                    .HasColumnName("LAST_COMPOSITION_ID")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.OrderWindows).HasColumnName("ORDER_WINDOWS");

                entity.Property(e => e.OrderWindowsOnChangeComp).HasColumnName("ORDER_WINDOWS_ON_CHANGE_COMP");

                entity.Property(e => e.UpdateIcons).HasColumnName("UPDATE_ICONS");
            });

            modelBuilder.Entity<Personatge>(entity =>
            {
                entity.HasKey(e => new { e.Nom, e.IdComposition });

                entity.ToTable("PERSONATGE");

                entity.Property(e => e.Nom).HasColumnName("NOM");

                entity.Property(e => e.IdComposition).HasColumnName("ID_COMPOSITION");

                entity.Property(e => e.IdClasse).HasColumnName("ID_CLASSE");

                entity.Property(e => e.IsActive).HasColumnName("IS_ACTIVE");

                entity.Property(e => e.KeyCodes)
                    .HasColumnType("NVARCHAR(100)")
                    .HasColumnName("KEY_CODES");

                entity.Property(e => e.Posicio).HasColumnName("POSICIO");

                entity.HasOne(d => d.IdClasseNavigation)
                    .WithMany(p => p.Personatges)
                    .HasForeignKey(d => d.IdClasse);

                entity.HasOne(d => d.IdCompositionNavigation)
                    .WithMany(p => p.Personatges)
                    .HasForeignKey(d => d.IdComposition);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
