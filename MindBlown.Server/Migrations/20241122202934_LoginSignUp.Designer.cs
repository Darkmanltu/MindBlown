﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MindBlown.Server.Data;

#nullable disable

namespace MindBlown.Server.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241122202934_LoginSignUp")]
    partial class LoginSignUp
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MindBlown.Server.Models.LastWrongAnswerRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("category")
                        .HasColumnType("int");

                    b.Property<string>("helperText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("mnemonicText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("wrongTextMnemonic")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Record");
                });

            modelBuilder.Entity("MindBlown.Server.Models.Mnemonic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Category")
                        .HasColumnType("int");

                    b.Property<string>("HelperText")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MnemonicText")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Mnemonics");
                });

            modelBuilder.Entity("MindBlown.Server.Models.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastActive")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("SessionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId");

                    b.ToTable("ActiveUserSession");
                });

            modelBuilder.Entity("MindBlown.Server.Models.UserMnemonicIDs", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("MnemonicGuids")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserWithMnemonicsIDs");
                });
#pragma warning restore 612, 618
        }
    }
}
