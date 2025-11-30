using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotless.Infrastructure.Migrations.Identity
{
    
    
    /// <inheritdoc />
    public partial class AddAddressToApplicationUser : Migration
    {
        
        
        
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            
            // Update existing users to have valid address data
            // We check if columns exist first or just run update assuming they do (since error said they do)
            migrationBuilder.Sql("UPDATE [Identity].[Users] SET Street = 'Unknown', City = 'Unknown', Country = 'Unknown', ZipCode = '00000' WHERE Street IS NULL");
        }

        ///// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No down operation needed as we are just updating data
        }
    }
}
