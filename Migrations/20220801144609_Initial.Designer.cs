﻿// <auto-generated />
using System;
using HeroesCup.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace heroescupreloaded.Migrations
{
    [DbContext(typeof(HeroesCupDbContext))]
    [Migration("20220801144609_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.7");

            modelBuilder.Entity("HeroesCup.Data.Models.Club", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedOn")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationName")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("OrganizationType")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Points")
                        .HasColumnType("INTEGER");

                    b.Property<long>("UpdatedOn")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Clubs");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.ClubImage", b =>
                {
                    b.Property<Guid?>("ClubId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("TEXT");

                    b.HasKey("ClubId", "ImageId");

                    b.HasIndex("ImageId");

                    b.ToTable("ClubImages");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Hero", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ClubId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCoordinator")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("Heroes");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.HeroMission", b =>
                {
                    b.Property<Guid>("HeroId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MissionId")
                        .HasColumnType("TEXT");

                    b.HasKey("HeroId", "MissionId");

                    b.HasIndex("MissionId");

                    b.ToTable("HeroMissions");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Image", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("Bytes")
                        .HasColumnType("BLOB");

                    b.Property<string>("ContentType")
                        .HasColumnType("TEXT");

                    b.Property<string>("Filename")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Mission", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ClubId")
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedOn")
                        .HasColumnType("INTEGER");

                    b.Property<int>("DurationInHours")
                        .HasColumnType("INTEGER");

                    b.Property<long>("EndDate")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPinned")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SchoolYear")
                        .HasColumnType("TEXT");

                    b.Property<string>("Slug")
                        .HasColumnType("TEXT");

                    b.Property<int>("Stars")
                        .HasColumnType("INTEGER");

                    b.Property<long>("StartDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("UpdatedOn")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.MissionContent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Contact")
                        .HasColumnType("TEXT");

                    b.Property<string>("Equipment")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("MissionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("What")
                        .HasColumnType("TEXT");

                    b.Property<string>("When")
                        .HasColumnType("TEXT");

                    b.Property<string>("Where")
                        .HasColumnType("TEXT");

                    b.Property<string>("Why")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("MissionId")
                        .IsUnique();

                    b.ToTable("MissionContents");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.MissionIdea", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedOn")
                        .HasColumnType("INTEGER");

                    b.Property<long>("EndDate")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Location")
                        .HasColumnType("TEXT");

                    b.Property<string>("Organization")
                        .HasColumnType("TEXT");

                    b.Property<string>("Slug")
                        .HasColumnType("TEXT");

                    b.Property<long>("StartDate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TimeheroesUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("UpdatedOn")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Slug")
                        .IsUnique();

                    b.ToTable("MissionIdeas");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.MissionIdeaImage", b =>
                {
                    b.Property<Guid?>("MissionIdeaId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("TEXT");

                    b.HasKey("MissionIdeaId", "ImageId");

                    b.HasIndex("ImageId");

                    b.ToTable("MissionIdeaImages");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.MissionImage", b =>
                {
                    b.Property<Guid?>("MissionId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("TEXT");

                    b.HasKey("MissionId", "ImageId");

                    b.HasIndex("ImageId");

                    b.ToTable("MissionImages");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Story", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("CreatedOn")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsPublished")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("MissionId")
                        .HasColumnType("TEXT");

                    b.Property<long>("UpdatedOn")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("MissionId")
                        .IsUnique();

                    b.ToTable("Stories");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.StoryImage", b =>
                {
                    b.Property<Guid?>("StoryId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ImageId")
                        .HasColumnType("TEXT");

                    b.HasKey("StoryId", "ImageId");

                    b.HasIndex("ImageId");

                    b.ToTable("StoryImages");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.ClubImage", b =>
                {
                    b.HasOne("HeroesCup.Data.Models.Club", "Club")
                        .WithMany("ClubImages")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HeroesCup.Data.Models.Image", "Image")
                        .WithMany("ClubImages")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");

                    b.Navigation("Image");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Hero", b =>
                {
                    b.HasOne("HeroesCup.Data.Models.Club", "Club")
                        .WithMany("Heroes")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.HeroMission", b =>
                {
                    b.HasOne("HeroesCup.Data.Models.Hero", "Hero")
                        .WithMany("HeroMissions")
                        .HasForeignKey("HeroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HeroesCup.Data.Models.Mission", "Mission")
                        .WithMany("HeroMissions")
                        .HasForeignKey("MissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Hero");

                    b.Navigation("Mission");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Mission", b =>
                {
                    b.HasOne("HeroesCup.Data.Models.Club", "Club")
                        .WithMany("Missions")
                        .HasForeignKey("ClubId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Club");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.MissionContent", b =>
                {
                    b.HasOne("HeroesCup.Data.Models.Mission", "Mission")
                        .WithOne("Content")
                        .HasForeignKey("HeroesCup.Data.Models.MissionContent", "MissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Mission");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.MissionIdeaImage", b =>
                {
                    b.HasOne("HeroesCup.Data.Models.Image", "Image")
                        .WithMany("MissionIdeaImages")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HeroesCup.Data.Models.MissionIdea", "MissionIdea")
                        .WithMany("MissionIdeaImages")
                        .HasForeignKey("MissionIdeaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("MissionIdea");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.MissionImage", b =>
                {
                    b.HasOne("HeroesCup.Data.Models.Image", "Image")
                        .WithMany("MissionImages")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HeroesCup.Data.Models.Mission", "Mission")
                        .WithMany("MissionImages")
                        .HasForeignKey("MissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Mission");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Story", b =>
                {
                    b.HasOne("HeroesCup.Data.Models.Mission", "Mission")
                        .WithOne("Story")
                        .HasForeignKey("HeroesCup.Data.Models.Story", "MissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Mission");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.StoryImage", b =>
                {
                    b.HasOne("HeroesCup.Data.Models.Image", "Image")
                        .WithMany("StoryImages")
                        .HasForeignKey("ImageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HeroesCup.Data.Models.Story", "Story")
                        .WithMany("StoryImages")
                        .HasForeignKey("StoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Image");

                    b.Navigation("Story");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Club", b =>
                {
                    b.Navigation("ClubImages");

                    b.Navigation("Heroes");

                    b.Navigation("Missions");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Hero", b =>
                {
                    b.Navigation("HeroMissions");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Image", b =>
                {
                    b.Navigation("ClubImages");

                    b.Navigation("MissionIdeaImages");

                    b.Navigation("MissionImages");

                    b.Navigation("StoryImages");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Mission", b =>
                {
                    b.Navigation("Content");

                    b.Navigation("HeroMissions");

                    b.Navigation("MissionImages");

                    b.Navigation("Story");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.MissionIdea", b =>
                {
                    b.Navigation("MissionIdeaImages");
                });

            modelBuilder.Entity("HeroesCup.Data.Models.Story", b =>
                {
                    b.Navigation("StoryImages");
                });
#pragma warning restore 612, 618
        }
    }
}
