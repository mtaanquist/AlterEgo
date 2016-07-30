using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AlterEgo.Data;

namespace AlterEgo.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20160730094617_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0-rtm-21431");

            modelBuilder.Entity("AlterEgo.Models.Achievement", b =>
                {
                    b.Property<int>("Id");

                    b.Property<bool>("AccountWide");

                    b.Property<string>("Description");

                    b.Property<int>("FactionId");

                    b.Property<string>("Icon");

                    b.Property<int>("Points");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Achievements");
                });

            modelBuilder.Entity("AlterEgo.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id");

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<int>("GuildRank");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedUserName")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("AlterEgo.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<int>("ReadableBy");

                    b.Property<int>("SortOrder");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("AlterEgo.Models.Character", b =>
                {
                    b.Property<string>("Name");

                    b.Property<string>("Realm");

                    b.Property<int>("AchievementPoints");

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("Battlegroup");

                    b.Property<int?>("CharacterClassId");

                    b.Property<int?>("CharacterRaceId");

                    b.Property<int>("Class");

                    b.Property<int>("Gender");

                    b.Property<string>("Guild");

                    b.Property<int>("GuildRank");

                    b.Property<string>("GuildRealm");

                    b.Property<long>("LastModified");

                    b.Property<int>("Level");

                    b.Property<string>("MemberCharacterName");

                    b.Property<string>("MemberCharacterRealm");

                    b.Property<string>("MemberGuildName");

                    b.Property<string>("MemberGuildRealm");

                    b.Property<int>("PlayerId");

                    b.Property<string>("PlayerId1");

                    b.Property<int>("Race");

                    b.Property<string>("Thumbnail");

                    b.HasKey("Name", "Realm");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("CharacterClassId");

                    b.HasIndex("CharacterRaceId");

                    b.HasIndex("PlayerId1");

                    b.HasIndex("MemberGuildName", "MemberGuildRealm", "MemberCharacterName", "MemberCharacterRealm")
                        .IsUnique();

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("AlterEgo.Models.Class", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("Mask");

                    b.Property<string>("Name");

                    b.Property<string>("PowerType");

                    b.HasKey("Id");

                    b.ToTable("Classes");
                });

            modelBuilder.Entity("AlterEgo.Models.Criteria", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int?>("AchievementId");

                    b.Property<string>("Description");

                    b.Property<int>("Max");

                    b.Property<int>("OrderIndex");

                    b.HasKey("Id");

                    b.HasIndex("AchievementId");

                    b.ToTable("Criteria");
                });

            modelBuilder.Entity("AlterEgo.Models.Forum", b =>
                {
                    b.Property<int>("ForumId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CanDeleteThreads");

                    b.Property<int>("CanEditThreads");

                    b.Property<int>("CanLockThreads");

                    b.Property<int>("CanStickyThreads");

                    b.Property<int>("CategoryId");

                    b.Property<string>("Description");

                    b.Property<int?>("LatestPostPostId");

                    b.Property<string>("Name");

                    b.Property<int>("ReadableBy");

                    b.Property<int>("SortOrder");

                    b.Property<int>("WritableBy");

                    b.HasKey("ForumId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("LatestPostPostId");

                    b.ToTable("Forums");
                });

            modelBuilder.Entity("AlterEgo.Models.Guild", b =>
                {
                    b.Property<string>("Name");

                    b.Property<string>("Realm");

                    b.Property<int>("AchievementPoints");

                    b.Property<string>("Battlegroup");

                    b.Property<long>("LastModified");

                    b.Property<int>("Level");

                    b.Property<int>("Side");

                    b.HasKey("Name", "Realm");

                    b.ToTable("Guilds");
                });

            modelBuilder.Entity("AlterEgo.Models.Member", b =>
                {
                    b.Property<string>("GuildName");

                    b.Property<string>("GuildRealm");

                    b.Property<string>("CharacterName");

                    b.Property<string>("CharacterRealm");

                    b.Property<int>("Rank");

                    b.HasKey("GuildName", "GuildRealm", "CharacterName", "CharacterRealm");

                    b.HasIndex("GuildName", "GuildRealm");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("AlterEgo.Models.News", b =>
                {
                    b.Property<int>("NewsId")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AchievementId");

                    b.Property<string>("Character");

                    b.Property<string>("Context");

                    b.Property<string>("GuildCharacterName");

                    b.Property<string>("GuildCharacterRealm");

                    b.Property<string>("GuildName");

                    b.Property<string>("GuildRealm");

                    b.Property<int>("ItemId");

                    b.Property<long>("Timestamp");

                    b.Property<string>("Type");

                    b.HasKey("NewsId");

                    b.HasIndex("AchievementId");

                    b.HasIndex("GuildCharacterName", "GuildCharacterRealm");

                    b.HasIndex("GuildName", "GuildRealm");

                    b.ToTable("News");
                });

            modelBuilder.Entity("AlterEgo.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId");

                    b.Property<string>("AuthorUserId");

                    b.Property<string>("Content");

                    b.Property<DateTime>("EditedAt");

                    b.Property<string>("EditorId");

                    b.Property<int>("EditorUserId");

                    b.Property<DateTime>("PostedAt");

                    b.Property<int>("ThreadId");

                    b.HasKey("PostId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("EditorId");

                    b.HasIndex("ThreadId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("AlterEgo.Models.Race", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("Mask");

                    b.Property<string>("Name");

                    b.Property<string>("Side");

                    b.HasKey("Id");

                    b.ToTable("Races");
                });

            modelBuilder.Entity("AlterEgo.Models.Thread", b =>
                {
                    b.Property<int>("ThreadId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AuthorId");

                    b.Property<string>("AuthorUserId");

                    b.Property<DateTime>("CreatedAt");

                    b.Property<string>("EditorId");

                    b.Property<int>("EditorUserId");

                    b.Property<int>("ForumId");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsLocked");

                    b.Property<bool>("IsStickied");

                    b.Property<DateTime>("LatestPostTime");

                    b.Property<DateTime>("ModifiedAt");

                    b.Property<string>("Name");

                    b.Property<int>("Views");

                    b.HasKey("ThreadId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("EditorId");

                    b.HasIndex("ForumId");

                    b.ToTable("Threads");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 256);

                    b.Property<string>("NormalizedName")
                        .HasAnnotation("MaxLength", 256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .HasName("RoleNameIndex");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("AlterEgo.Models.Character", b =>
                {
                    b.HasOne("AlterEgo.Models.ApplicationUser")
                        .WithMany("Characters")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("AlterEgo.Models.Class", "CharacterClass")
                        .WithMany()
                        .HasForeignKey("CharacterClassId");

                    b.HasOne("AlterEgo.Models.Race", "CharacterRace")
                        .WithMany()
                        .HasForeignKey("CharacterRaceId");

                    b.HasOne("AlterEgo.Models.ApplicationUser", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId1");

                    b.HasOne("AlterEgo.Models.Member", "Member")
                        .WithOne("Character")
                        .HasForeignKey("AlterEgo.Models.Character", "MemberGuildName", "MemberGuildRealm", "MemberCharacterName", "MemberCharacterRealm");
                });

            modelBuilder.Entity("AlterEgo.Models.Criteria", b =>
                {
                    b.HasOne("AlterEgo.Models.Achievement")
                        .WithMany("Criteria")
                        .HasForeignKey("AchievementId");
                });

            modelBuilder.Entity("AlterEgo.Models.Forum", b =>
                {
                    b.HasOne("AlterEgo.Models.Category", "Category")
                        .WithMany("Forums")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AlterEgo.Models.Post", "LatestPost")
                        .WithMany()
                        .HasForeignKey("LatestPostPostId");
                });

            modelBuilder.Entity("AlterEgo.Models.Member", b =>
                {
                    b.HasOne("AlterEgo.Models.Guild", "Guild")
                        .WithMany("Members")
                        .HasForeignKey("GuildName", "GuildRealm")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AlterEgo.Models.News", b =>
                {
                    b.HasOne("AlterEgo.Models.Achievement", "Achievement")
                        .WithMany()
                        .HasForeignKey("AchievementId");

                    b.HasOne("AlterEgo.Models.Character", "GuildCharacter")
                        .WithMany()
                        .HasForeignKey("GuildCharacterName", "GuildCharacterRealm");

                    b.HasOne("AlterEgo.Models.Guild", "Guild")
                        .WithMany("News")
                        .HasForeignKey("GuildName", "GuildRealm");
                });

            modelBuilder.Entity("AlterEgo.Models.Post", b =>
                {
                    b.HasOne("AlterEgo.Models.ApplicationUser", "Author")
                        .WithMany("AuthoredPosts")
                        .HasForeignKey("AuthorId");

                    b.HasOne("AlterEgo.Models.ApplicationUser", "Editor")
                        .WithMany("EditedPosts")
                        .HasForeignKey("EditorId");

                    b.HasOne("AlterEgo.Models.Thread", "Thread")
                        .WithMany("Posts")
                        .HasForeignKey("ThreadId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AlterEgo.Models.Thread", b =>
                {
                    b.HasOne("AlterEgo.Models.ApplicationUser", "Author")
                        .WithMany("AuthoredThreads")
                        .HasForeignKey("AuthorId");

                    b.HasOne("AlterEgo.Models.ApplicationUser", "Editor")
                        .WithMany("EditedThreads")
                        .HasForeignKey("EditorId");

                    b.HasOne("AlterEgo.Models.Forum", "Forum")
                        .WithMany("Threads")
                        .HasForeignKey("ForumId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("AlterEgo.Models.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AlterEgo.Models.ApplicationUser")
                        .WithMany("Logins")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AlterEgo.Models.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
