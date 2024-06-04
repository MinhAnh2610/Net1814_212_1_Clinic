﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Data.Models;

public partial class Net1814_212_1_ClinicContext : DbContext
{
    public Net1814_212_1_ClinicContext()
    {
    }

    public Net1814_212_1_ClinicContext(DbContextOptions<Net1814_212_1_ClinicContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<AppointmentDetail> AppointmentDetails { get; set; }

    public virtual DbSet<Clinic> Clinics { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Record> Records { get; set; }

    public virtual DbSet<RecordDetail> RecordDetails { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-3ED3JRS\\SQLEXPRESS;Initial Catalog=Net1814_212_1_Clinic;Persist Security Info=True;User ID=soybean;Password=123456;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointment");

            entity.Property(e => e.AppointmentId)
                .ValueGeneratedNever()
                .HasColumnName("AppointmentID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");
            entity.Property(e => e.DentistName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Total).HasColumnType("money");

            entity.HasOne(d => d.Customer).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("App_Cus");
        });

        modelBuilder.Entity<AppointmentDetail>(entity =>
        {
            entity.HasKey(e => e.AppointmentDetailId).HasName("PK_AppointmentServices");

            entity.ToTable("AppointmentDetail");

            entity.Property(e => e.AppointmentDetailId)
                .ValueGeneratedNever()
                .HasColumnName("AppointmentDetailID");
            entity.Property(e => e.AppointmentId).HasColumnName("AppointmentID");
            entity.Property(e => e.ServiceId).HasColumnName("ServiceID");

            entity.HasOne(d => d.Appointment).WithMany(p => p.AppointmentDetails)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AppS_App");

            entity.HasOne(d => d.Service).WithMany(p => p.AppointmentDetails)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("AppS_Ser");
        });

        modelBuilder.Entity<Clinic>(entity =>
        {
            entity.HasKey(e => e.ClinicId).HasName("PK_Company");

            entity.ToTable("Clinic");

            entity.Property(e => e.ClinicId)
                .ValueGeneratedNever()
                .HasColumnName("ClinicID");
            entity.Property(e => e.Address).IsRequired();
            entity.Property(e => e.Contact)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.OwnerName)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("CustomerID");
            entity.Property(e => e.FirstName).IsRequired();
            entity.Property(e => e.LastName).IsRequired();
            entity.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Record>(entity =>
        {
            entity.ToTable("Record");

            entity.Property(e => e.RecordId)
                .ValueGeneratedNever()
                .HasColumnName("RecordID");
            entity.Property(e => e.ClinicId).HasColumnName("ClinicID");
            entity.Property(e => e.CustomerId).HasColumnName("CustomerID");

            entity.HasOne(d => d.Clinic).WithMany(p => p.Records)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Rec_Cli");

            entity.HasOne(d => d.Customer).WithMany(p => p.Records)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Rec_Cus");
        });

        modelBuilder.Entity<RecordDetail>(entity =>
        {
            entity.ToTable("RecordDetail");

            entity.Property(e => e.RecordDetailId)
                .ValueGeneratedNever()
                .HasColumnName("RecordDetailID");
            entity.Property(e => e.AppointmentDetailId).HasColumnName("AppointmentDetailID");
            entity.Property(e => e.Evaluation)
                .IsRequired()
                .HasColumnType("text");
            entity.Property(e => e.Reccommend)
                .IsRequired()
                .HasColumnType("text");
            entity.Property(e => e.RecordId).HasColumnName("RecordID");

            entity.HasOne(d => d.AppointmentDetail).WithMany(p => p.RecordDetails)
                .HasForeignKey(d => d.AppointmentDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RecD_AppD");

            entity.HasOne(d => d.Record).WithMany(p => p.RecordDetails)
                .HasForeignKey(d => d.RecordId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("RecD_Rec");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.ToTable("Service");

            entity.Property(e => e.ServiceId)
                .ValueGeneratedNever()
                .HasColumnName("ServiceID");
            entity.Property(e => e.ClinicId).HasColumnName("ClinicID");
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("money");

            entity.HasOne(d => d.Clinic).WithMany(p => p.Services)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Ser_Com");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}