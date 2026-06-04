using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SecureTaskHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedPasswords : Migration
    {
        // Corrects the seeded password hashes to match "P@ssword1".
        // The initial migration was created with a different set of hardcoded hashes.
        // These replacements bring HasData, the snapshot, and the live database into sync.
        const string AdminId = "d633d3fa-5890-413d-a0b7-242601a49a75";
        const string AliceId = "495d56e4-43b1-4e54-9cef-10e917d28f63";
        const string BobId   = "21809ee3-a838-42e1-99b5-ad07b8deb31b";

        const string NewAdminHash = "AQAAAAIAAYagAAAAEMP818yxP3ieYL8yLcqz610LhpuIq2as0NSTCr7SD9/dwM3Zq/MehPV16S/MG9bshA==";
        const string NewAliceHash = "AQAAAAIAAYagAAAAECYgX4PAQWRU8bzBR+6KquSbnLMIMERJ9qqNn/byfYokNqlGXORsBzlz5bT7Cz6qVA==";
        const string NewBobHash   = "AQAAAAIAAYagAAAAEM3YmPjAE7F+MGthkwPbzGpY7c6Ucwv/qxsLCR5UYZUgHZ9LyNsh5Fx8ebTvopXCWA==";

        const string OldAdminHash = "AQAAAAIAAYagAAAAEJ3h9d5WYzthMCKEbVqfZYGmEP4rdZdKcLhKYo3EqXz+uGqC8X1Pq5SiJ4KdVkzSLQ==";
        const string OldAliceHash = "AQAAAAIAAYagAAAAEGxF7p4M3NqK8yWjLZv5rQwXzB9cT2sH6vN1kP8oY4eD3mR7aJ9iU5tL2fO6wE1xKg==";
        const string OldBobHash   = "AQAAAAIAAYagAAAAEDn8K5r2TvH6pM9wJ3sX1cY7oB4eP8qL2fN6kR9iZ3tA5mG1uV7hO4jE6wD8cS2nLx==";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE \"AspNetUsers\" SET \"PasswordHash\" = '{NewAdminHash}' WHERE \"Id\" = '{AdminId}'");
            migrationBuilder.Sql($"UPDATE \"AspNetUsers\" SET \"PasswordHash\" = '{NewAliceHash}' WHERE \"Id\" = '{AliceId}'");
            migrationBuilder.Sql($"UPDATE \"AspNetUsers\" SET \"PasswordHash\" = '{NewBobHash}'   WHERE \"Id\" = '{BobId}'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE \"AspNetUsers\" SET \"PasswordHash\" = '{OldAdminHash}' WHERE \"Id\" = '{AdminId}'");
            migrationBuilder.Sql($"UPDATE \"AspNetUsers\" SET \"PasswordHash\" = '{OldAliceHash}' WHERE \"Id\" = '{AliceId}'");
            migrationBuilder.Sql($"UPDATE \"AspNetUsers\" SET \"PasswordHash\" = '{OldBobHash}'   WHERE \"Id\" = '{BobId}'");
        }
    }
}
