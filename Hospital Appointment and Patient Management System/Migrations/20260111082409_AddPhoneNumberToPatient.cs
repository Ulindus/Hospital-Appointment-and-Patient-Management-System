using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_Appointment_and_Patient_Management_System.Migrations
{
    
    public partial class AddPhoneNumberToPatient : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }

      
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Patients");
        }
    }
}
