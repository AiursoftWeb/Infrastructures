﻿// <auto-generated />
using System;
using Aiursoft.Wrapgate.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Aiursoft.Wrapgate.Migrations
{
    [DbContext(typeof(WrapgateDbContext))]
    [Migration("20210114055959_TestIndex")]
    partial class TestIndex
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.1");

            modelBuilder.Entity("Aiursoft.Wrapgate.SDK.Models.WrapRecord", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("AppId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("RecordUniqueName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Tags")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TargetUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AppId");

                    b.HasIndex("RecordUniqueName")
                        .IsUnique()
                        .HasFilter("[RecordUniqueName] IS NOT NULL");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("Aiursoft.Wrapgate.SDK.Models.WrapgateApp", b =>
                {
                    b.Property<string>("AppId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AppId");

                    b.ToTable("WrapApps");
                });

            modelBuilder.Entity("Aiursoft.Wrapgate.SDK.Models.WrapRecord", b =>
                {
                    b.HasOne("Aiursoft.Wrapgate.SDK.Models.WrapgateApp", "App")
                        .WithMany("WrapRecords")
                        .HasForeignKey("AppId");

                    b.Navigation("App");
                });

            modelBuilder.Entity("Aiursoft.Wrapgate.SDK.Models.WrapgateApp", b =>
                {
                    b.Navigation("WrapRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
