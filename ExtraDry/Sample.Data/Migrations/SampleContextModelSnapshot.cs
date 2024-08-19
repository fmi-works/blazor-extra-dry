﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.SqlServer.Types;
using Sample.Data;

#nullable disable

namespace Sample.Data.Migrations
{
    [DbContext(typeof(SampleContext))]
    partial class SampleContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Sample.Shared.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AnnualRevenue")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("BankingDetails")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateOnly>("CeoBirthdayHoliday")
                        .HasColumnType("date");

                    b.Property<string>("ContactEmail")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ContactPhone")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("nvarchar(24)");

                    b.Property<string>("CustomFields")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(max)")
                        .HasDefaultValueSql("'{}'")
                        .HasAnnotation("Relational:JsonPropertyName", "fields");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<DateTime?>("DissolutionDate")
                        .HasColumnType("datetime2");

                    b.Property<TimeOnly?>("EndOfBusinessHours")
                        .HasColumnType("time");

                    b.Property<DateTime>("IncorporationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastTrademarkReview")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Ownership")
                        .HasColumnType("int");

                    b.Property<int?>("PrimarySectorId")
                        .HasColumnType("int");

                    b.Property<decimal>("SalesMargin")
                        .HasPrecision(18, 2)
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(24)
                        .HasColumnType("nvarchar(24)");

                    b.Property<TimeOnly>("StartOfBusinessHours")
                        .HasColumnType("time");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PrimarySectorId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("Sample.Shared.Content", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Layout")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Contents");
                });

            modelBuilder.Entity("Sample.Shared.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .HasMaxLength(120)
                        .HasColumnType("nvarchar(120)");

                    b.Property<int?>("EmployerId")
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("TerminationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("EmployerId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Sample.Shared.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("IsDeleted")
                        .HasColumnType("int");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<SqlHierarchyId>("Lineage")
                        .HasColumnType("hierarchyid");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<string>("Slug")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.HasIndex("Uuid")
                        .IsUnique();

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("Sample.Shared.Sector", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Group")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Sectors");
                });

            modelBuilder.Entity("Sample.Shared.Template", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Schema")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<Guid>("Uuid")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("Sample.Shared.Company", b =>
                {
                    b.HasOne("Sample.Shared.Sector", "PrimarySector")
                        .WithMany()
                        .HasForeignKey("PrimarySectorId");

                    b.OwnsOne("ExtraDry.Core.VersionInfo", "Version", b1 =>
                        {
                            b1.Property<int>("CompanyId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("DateCreated")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("DateModified")
                                .HasColumnType("datetime2");

                            b1.Property<string>("UserCreated")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.Property<string>("UserModified")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.HasKey("CompanyId");

                            b1.ToTable("Companies");

                            b1.WithOwner()
                                .HasForeignKey("CompanyId");
                        });

                    b.Navigation("PrimarySector");

                    b.Navigation("Version")
                        .IsRequired();
                });

            modelBuilder.Entity("Sample.Shared.Content", b =>
                {
                    b.OwnsOne("ExtraDry.Core.VersionInfo", "Version", b1 =>
                        {
                            b1.Property<int>("ContentId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("DateCreated")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("DateModified")
                                .HasColumnType("datetime2");

                            b1.Property<string>("UserCreated")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.Property<string>("UserModified")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.HasKey("ContentId");

                            b1.ToTable("Contents");

                            b1.WithOwner()
                                .HasForeignKey("ContentId");
                        });

                    b.Navigation("Version")
                        .IsRequired();
                });

            modelBuilder.Entity("Sample.Shared.Employee", b =>
                {
                    b.HasOne("Sample.Shared.Company", "Employer")
                        .WithMany()
                        .HasForeignKey("EmployerId");

                    b.OwnsOne("ExtraDry.Core.UserTimestamp", "Revision", b1 =>
                        {
                            b1.Property<int>("EmployeeId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("Timestamp")
                                .HasColumnType("datetime2");

                            b1.Property<string>("User")
                                .IsRequired()
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employees");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.OwnsOne("ExtraDry.Core.VersionInfo", "Version", b1 =>
                        {
                            b1.Property<int>("EmployeeId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("DateCreated")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("DateModified")
                                .HasColumnType("datetime2");

                            b1.Property<string>("UserCreated")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.Property<string>("UserModified")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employees");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.Navigation("Employer");

                    b.Navigation("Revision")
                        .IsRequired();

                    b.Navigation("Version")
                        .IsRequired();
                });

            modelBuilder.Entity("Sample.Shared.Region", b =>
                {
                    b.HasOne("Sample.Shared.Region", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.OwnsOne("ExtraDry.Core.VersionInfo", "Version", b1 =>
                        {
                            b1.Property<int>("RegionId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("DateCreated")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("DateModified")
                                .HasColumnType("datetime2");

                            b1.Property<string>("UserCreated")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.Property<string>("UserModified")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.HasKey("RegionId");

                            b1.ToTable("Regions");

                            b1.WithOwner()
                                .HasForeignKey("RegionId");
                        });

                    b.Navigation("Parent");

                    b.Navigation("Version")
                        .IsRequired();
                });

            modelBuilder.Entity("Sample.Shared.Sector", b =>
                {
                    b.HasOne("Sample.Shared.Company", null)
                        .WithMany("AdditionalSectors")
                        .HasForeignKey("CompanyId");

                    b.OwnsOne("ExtraDry.Core.VersionInfo", "Version", b1 =>
                        {
                            b1.Property<int>("SectorId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("DateCreated")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("DateModified")
                                .HasColumnType("datetime2");

                            b1.Property<string>("UserCreated")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.Property<string>("UserModified")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.HasKey("SectorId");

                            b1.ToTable("Sectors");

                            b1.WithOwner()
                                .HasForeignKey("SectorId");
                        });

                    b.Navigation("Version")
                        .IsRequired();
                });

            modelBuilder.Entity("Sample.Shared.Template", b =>
                {
                    b.OwnsOne("ExtraDry.Core.VersionInfo", "Version", b1 =>
                        {
                            b1.Property<int>("TemplateId")
                                .HasColumnType("int");

                            b1.Property<DateTime>("DateCreated")
                                .HasColumnType("datetime2");

                            b1.Property<DateTime>("DateModified")
                                .HasColumnType("datetime2");

                            b1.Property<string>("UserCreated")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.Property<string>("UserModified")
                                .IsRequired()
                                .HasMaxLength(80)
                                .HasColumnType("nvarchar(80)");

                            b1.HasKey("TemplateId");

                            b1.ToTable("Templates");

                            b1.WithOwner()
                                .HasForeignKey("TemplateId");
                        });

                    b.Navigation("Version")
                        .IsRequired();
                });

            modelBuilder.Entity("Sample.Shared.Company", b =>
                {
                    b.Navigation("AdditionalSectors");
                });
#pragma warning restore 612, 618
        }
    }
}
