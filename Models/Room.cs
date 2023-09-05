namespace Roommates.Models;
public class Room
{
    private int _monthlyRent = 450;
    public int Id { get; set; }
    public string Name { get; set; }
    public int MaxOccupancy { get; set; }

    public List<Roommate> Roommates { get; set; }
}