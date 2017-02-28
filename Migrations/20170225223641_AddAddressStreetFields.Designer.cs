using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using dc_snoop.DAL;

namespace dcsnoop.Migrations
{
    [DbContext(typeof(SnoopContext))]
    [Migration("20170225223641_AddAddressStreetFields")]
    partial class AddAddressStreetFields
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("dc_snoop.Models.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Precinct");

                    b.Property<string>("Street");

                    b.Property<string>("StreetName");

                    b.Property<string>("StreetNumber");

                    b.Property<string>("StreetQuadrant");

                    b.Property<string>("StreetType");

                    b.Property<string>("Ward");

                    b.Property<string>("Zip");

                    b.HasKey("Id");

                    b.ToTable("Addresses");
                });

            modelBuilder.Entity("dc_snoop.Models.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AddressId");

                    b.Property<string>("Affiliation");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("MiddleName");

                    b.Property<DateTime?>("RegistrationDate");

                    b.Property<string>("Status1211");

                    b.Property<string>("Status1304");

                    b.Property<string>("Status1404");

                    b.Property<string>("Status1407");

                    b.Property<string>("Status1411");

                    b.Property<string>("Status1504");

                    b.Property<string>("Unit");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("People");
                });

            modelBuilder.Entity("dc_snoop.Models.Person", b =>
                {
                    b.HasOne("dc_snoop.Models.Address", "Address")
                        .WithMany("People")
                        .HasForeignKey("AddressId");
                });
        }
    }
}
