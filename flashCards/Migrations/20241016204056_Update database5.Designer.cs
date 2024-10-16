﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace flashCards.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241016204056_Update database5")]
    partial class Updatedatabase5
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.27");

            modelBuilder.Entity("flashCards.Models.Flashcard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("FlashcardPackId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ImageURL")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("SubQuestion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("FlashcardPackId");

                    b.ToTable("Flashcards");
                });

            modelBuilder.Entity("flashCards.Models.FlashcardPack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("PackName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("FlashcardPacks");
                });

            modelBuilder.Entity("flashCards.Models.Flashcard", b =>
                {
                    b.HasOne("flashCards.Models.FlashcardPack", "FlashcardPack")
                        .WithMany("Flashcards")
                        .HasForeignKey("FlashcardPackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FlashcardPack");
                });

            modelBuilder.Entity("flashCards.Models.FlashcardPack", b =>
                {
                    b.Navigation("Flashcards");
                });
#pragma warning restore 612, 618
        }
    }
}
