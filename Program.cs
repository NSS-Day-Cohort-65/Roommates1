using Roommates.Models;

// constant in snake case
const int TOTAL_RENT = 1000;

List<Room> rooms = new List<Room>
{
    new Room { Id = 1, MaxOccupancy = 2, Name = "Bedroom 1"},
    new Room { Id = 2, MaxOccupancy = 1, Name = "Bedroom 2" },
    new Room { Id = 3, MaxOccupancy = 3, Name = "Den"},
    new Room { Id = 4, MaxOccupancy = 4, Name = "Basement"}
};

List<Roommate> roommates = new List<Roommate>
{
    new Roommate {Id = 1, FirstName = "Nic", LastName = "Lahde", MovedInDate = new DateTime(2021, 1, 25), RentPortion = 20, RoomId = 2 },
    new Roommate {Id = 2, FirstName = "Alex", LastName = "Bishop", MovedInDate = new DateTime(2021, 2, 15), RentPortion = 15, RoomId = 1 },
    new Roommate {Id = 3, FirstName = "Dan", LastName = "Brady", MovedInDate = new DateTime(2021, 2, 10), RentPortion = 10, RoomId = 3 },
};

List<Chore> chores = new List<Chore>
{
    new Chore {Id = 1, Name = "Take Out Trash", RoommateId = 1 },
    new Chore {Id = 2, Name = "Vacuum", RoommateId = 2 },
    new Chore {Id = 3, Name = "Do Dishes"},
};


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// get all rooms
app.MapGet("/rooms", () =>
{
    return rooms;
});

// get room by id with roomates
app.MapGet("/rooms/{roomId}", (int roomId) =>
{
    Room foundRoom = rooms.FirstOrDefault(r => r.Id == roomId);

    if (foundRoom == null)
    {
        return Results.NotFound();
    }

    List<Roommate> foundRoommates = roommates.Where(rm => rm.RoomId == roomId).ToList();

    foundRoom.Roommates = foundRoommates;

    return Results.Ok(foundRoom);
});

// update room 
app.MapPut("/rooms/{roomid}", (int roomId, Room room) =>
{
    if (roomId != room.Id) return Results.BadRequest();

    Room foundRoom = rooms.FirstOrDefault(r => r.Id == roomId);

    if (foundRoom == null) return Results.NotFound();

    // only changes reference pointer
    // foundRoom = room;

    foundRoom.Name = room.Name;
    foundRoom.MaxOccupancy = room.MaxOccupancy;

    return Results.NoContent();
});

// delete a room
app.MapDelete("/rooms/{id}", (int id) =>
{
    Room roomToDelete = rooms.SingleOrDefault(r => r.Id == id);
    if (roomToDelete == null)
    {
        return Results.NotFound();
    }
    List<Roommate> roommatesToMove = roommates.Where(rm => rm.RoomId == id).ToList();
    foreach (Roommate roomie in roommatesToMove)
    {
        roomie.RoomId = null;
    }

    rooms.Remove(roomToDelete);

    return Results.NoContent();
});

// get roommates

app.MapGet("/roommates", () =>
{
    return Results.Ok(roommates);
});

// get roommate with chores

app.MapGet("/roommates/{id}", (int id) =>
{
    Roommate foundRoommate = roommates.SingleOrDefault(rm => rm.Id == id);

    if (foundRoommate == null)
    {
        return Results.NotFound();
    }

    List<Chore> foundChores = chores.Where(c => c.RoommateId == id).ToList();

    foundRoommate.Chores = foundChores;

    return Results.Ok(foundRoommate);

});

// add a roommate 
app.MapPost("/roommates", (Roommate roommate) =>
{
    roommate.Id = roommates.Count > 0 ? roommates.Max(r => r.Id) + 1 : 1;
    roommate.MovedInDate = DateTime.Today;
    roommates.Add(roommate);
    return Results.Created($"/roommates/{roommate.Id}", roommate);
});
// assign a roommate to a chore
app.MapPost("/chores/{choreId}/assign/{roommateId}", (int roommateId, int choreId) =>
{
    Chore foundChore = chores.FirstOrDefault(c => c.Id == choreId);

    bool roommateExists = roommates.Any(r => r.Id == roommateId);

    if (roommateExists && foundChore != null)
    {
        foundChore.RoommateId = roommateId;

        return Results.NoContent();
    }

    return Results.NotFound();

});
// calculate rent for each roommate and return a report
app.MapGet("/totalrent", () =>
{
    // create place to store solution
    var totalRent = new Dictionary<string, decimal>();
    // loop through roommates
    foreach (var roommate in roommates)
    {
        // add to solution
        totalRent.Add(roommate.FullName, roommate.Rent);
    }
    totalRent.Add("total rent", TOTAL_RENT);
    // return solution
    return Results.Ok(totalRent);
});

app.Run();

// Array - fixed length, useful for storing data that you don't need to change
// List - like JS array, can use .Add()
// Dictionary - like JS Object, Key Value pairs
// Enum - can access data as properties 