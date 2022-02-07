using Microsoft.EntityFrameworkCore.Migrations;

namespace MovieManagerAPI.Migrations
{
    public partial class identitydata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"                
                IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
                    SET IDENTITY_INSERT [AspNetRoles] ON;
                INSERT INTO [AspNetRoles] ([Id], [ConcurrencyStamp], [Name], [NormalizedName])
                VALUES (N'7a0b978d-d515-4e38-89a3-dea1dbec6275', N'186671ae-532f-4cf9-bb71-3d6ab96fe506', N'Admin', N'Admin');
                IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ConcurrencyStamp', N'Name', N'NormalizedName') AND [object_id] = OBJECT_ID(N'[AspNetRoles]'))
                    SET IDENTITY_INSERT [AspNetRoles] OFF;
                GO

                IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
                    SET IDENTITY_INSERT [AspNetUsers] ON;
                INSERT INTO [AspNetUsers] ([Id], [AccessFailedCount], [ConcurrencyStamp], [Email], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], [NormalizedEmail], [NormalizedUserName], [PasswordHash], [PhoneNumber], [PhoneNumberConfirmed], [SecurityStamp], [TwoFactorEnabled], [UserName])
                VALUES (N'4ecc4de5-27ba-4012-bdce-7dfd8d3c3bff', 0, N'aa4a269a-8ac7-4eff-a467-74041c2180c1', N'eldevelopermacho@bravedeveloper.com', CAST(0 AS bit), CAST(0 AS bit), NULL, N'eldevelopermacho@bravedeveloper.com', N'eldevelopermacho@bravedeveloper.com', N'AQAAAAEAACcQAAAAEFBsCqEL7KecwVkyi4xfXlpEZhZnYTIwBwG9k+yOc/5NF44UEL2U5Fhzb1rrPwDcvA==', NULL, CAST(0 AS bit), N'67e5d4ec-4c57-4405-af7c-e1ec84f6fd3f', CAST(0 AS bit), N'eldevelopermacho@bravedeveloper.com');
                IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessFailedCount', N'ConcurrencyStamp', N'Email', N'EmailConfirmed', N'LockoutEnabled', N'LockoutEnd', N'NormalizedEmail', N'NormalizedUserName', N'PasswordHash', N'PhoneNumber', N'PhoneNumberConfirmed', N'SecurityStamp', N'TwoFactorEnabled', N'UserName') AND [object_id] = OBJECT_ID(N'[AspNetUsers]'))
                    SET IDENTITY_INSERT [AspNetUsers] OFF;
                GO

                IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClaimType', N'ClaimValue', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserClaims]'))
                    SET IDENTITY_INSERT [AspNetUserClaims] ON;
                INSERT INTO [AspNetUserClaims] ([Id], [ClaimType], [ClaimValue], [UserId])
                VALUES (1, N'http://schemas.microsoft.com/ws/2008/06/identity/claims/role', N'Admin', N'4ecc4de5-27ba-4012-bdce-7dfd8d3c3bff');
                IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ClaimType', N'ClaimValue', N'UserId') AND [object_id] = OBJECT_ID(N'[AspNetUserClaims]'))
                    SET IDENTITY_INSERT [AspNetUserClaims] OFF;
                GO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7a0b978d-d515-4e38-89a3-dea1dbec6275");

            migrationBuilder.DeleteData(
                table: "AspNetUserClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4ecc4de5-27ba-4012-bdce-7dfd8d3c3bff");
        }
    }
}
