﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchwiBot.Database;

namespace SchwiBot.Migrations
{
    [DbContext(typeof(SchwiDatabaseContext))]
    [Migration("20201104025428_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.9");

            modelBuilder.Entity("SchwiBot.Database.DbGuildStorage", b =>
                {
                    b.Property<ulong>("Snowflake")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Storage")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("UserStorage")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Snowflake");

                    b.ToTable("GuildStorages");
                });

            modelBuilder.Entity("SchwiBot.Database.DbUserStorage", b =>
                {
                    b.Property<ulong>("Snowflake")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Storage")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.HasKey("Snowflake");

                    b.ToTable("UserStorages");
                });
#pragma warning restore 612, 618
        }
    }
}
