﻿// <auto-generated />
using System;
using BelTwit_REST_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BelTwit_REST_API.Migrations
{
    [DbContext(typeof(BelTwitContext))]
    [Migration("20200520191839_v0.6.2")]
    partial class v062
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BelTwit_REST_API.Models.Log", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Action")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("LogLevel")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("BelTwit_REST_API.Models.Reaction", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TweetId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Comment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDislike")
                        .HasColumnType("bit");

                    b.Property<bool>("IsLike")
                        .HasColumnType("bit");

                    b.Property<bool>("IsRetweeted")
                        .HasColumnType("bit");

                    b.HasKey("UserId", "TweetId");

                    b.HasIndex("TweetId");

                    b.ToTable("Reactions");
                });

            modelBuilder.Entity("BelTwit_REST_API.Models.RefreshToken", b =>
                {
                    b.Property<Guid>("TokenValue")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TokenValue");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("BelTwit_REST_API.Models.SubscriberSubscription", b =>
                {
                    b.Property<Guid>("WhoSubscribeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OnWhomSubscribeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("WhoSubscribeId", "OnWhomSubscribeId");

                    b.HasIndex("OnWhomSubscribeId");

                    b.ToTable("SubscriberSubscriptions");
                });

            modelBuilder.Entity("BelTwit_REST_API.Models.Tweet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Dislikes")
                        .HasColumnType("int");

                    b.Property<int>("Likes")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserIdRetweetedFrom")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tweets");
                });

            modelBuilder.Entity("BelTwit_REST_API.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)")
                        .HasMaxLength(100);

                    b.Property<string>("PasswordSalt")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BelTwit_REST_API.Models.Reaction", b =>
                {
                    b.HasOne("BelTwit_REST_API.Models.Tweet", "Tweet")
                        .WithMany("TweetReactions")
                        .HasForeignKey("TweetId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BelTwit_REST_API.Models.User", "User")
                        .WithMany("TweetReactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("BelTwit_REST_API.Models.RefreshToken", b =>
                {
                    b.HasOne("BelTwit_REST_API.Models.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BelTwit_REST_API.Models.SubscriberSubscription", b =>
                {
                    b.HasOne("BelTwit_REST_API.Models.User", "OnWhomSubscribe")
                        .WithMany("Subscribers")
                        .HasForeignKey("OnWhomSubscribeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BelTwit_REST_API.Models.User", "WhoSubscribe")
                        .WithMany("Subscriptions")
                        .HasForeignKey("WhoSubscribeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("BelTwit_REST_API.Models.Tweet", b =>
                {
                    b.HasOne("BelTwit_REST_API.Models.User", "User")
                        .WithMany("Tweets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
