namespace Roommates.Models;
public class Roommate
{
    private static int _rent = 1000;
    public decimal Rent
    {
        get
        {
            return RentPortion * _rent / 100;
        }
    }
    public string FullName
    {
        get
        {
            return $"{FirstName} {LastName}";
        }
    }
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int RentPortion { get; set; }
    public DateTime MovedInDate { get; set; }
    public int? RoomId { get; set; }
    public Room Room { get; set; }
    public List<Chore> Chores { get; set; }
}