using System;
using System.Collections.Generic;
using System.Linq;

public class HotelBookingSystem
{
    private List<User> users = new List<User>();
    private List<Room> rooms = new List<Room>();
    private List<Reservation> reservations = new List<Reservation>();
    private User currentUser = null;

    public HotelBookingSystem()
    {
        InitializeSampleData();
    }

    // Initialize sample data for testing
    private void InitializeSampleData()
    {
        // Add sample users
        users.Add(new User { UserId = 1, Username = "john_doe", Password = "password123", Name = "John Doe", Email = "john@example.com" });
        users.Add(new User { UserId = 2, Username = "jane_smith", Password = "password456", Name = "Jane Smith", Email = "jane@example.com" });

        // Add sample rooms
        rooms.Add(new Room { RoomId = 101, RoomType = "Single", Capacity = 1, PricePerNight = 50.00m });
        rooms.Add(new Room { RoomId = 102, RoomType = "Single", Capacity = 1, PricePerNight = 50.00m });
        rooms.Add(new Room { RoomId = 201, RoomType = "Double", Capacity = 2, PricePerNight = 80.00m });
        rooms.Add(new Room { RoomId = 202, RoomType = "Double", Capacity = 2, PricePerNight = 80.00m });
        rooms.Add(new Room { RoomId = 301, RoomType = "Suite", Capacity = 4, PricePerNight = 150.00m });
        rooms.Add(new Room { RoomId = 302, RoomType = "Suite", Capacity = 4, PricePerNight = 150.00m });
    }

    // User Registration
    public bool Register(string username, string password, string name, string email)
    {
        if (users.Any(u => u.Username == username))
        {
            Console.WriteLine("Username already exists!");
            return false;
        }

        int newUserId = users.Count > 0 ? users.Max(u => u.UserId) + 1 : 1;
        users.Add(new User { UserId = newUserId, Username = username, Password = password, Name = name, Email = email });
        Console.WriteLine($"Registration successful! Welcome {name}");
        return true;
    }

    // User Login
    public bool Login(string username, string password)
    {
        User user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
        if (user != null)
        {
            currentUser = user;
            Console.WriteLine($"Login successful! Welcome {user.Name}");
            return true;
        }
        Console.WriteLine("Invalid username or password!");
        return false;
    }

    // User Logout
    public void Logout()
    {
        if (currentUser != null)
        {
            Console.WriteLine($"Goodbye {currentUser.Name}!");
            currentUser = null;
        }
    }

    // View Available Rooms
    public void ViewAvailableRooms(DateTime checkInDate, DateTime checkOutDate, int numPersons)
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please login first!");
            return;
        }

        Console.WriteLine($"\n--- Available Rooms ({numPersons} person(s), {(checkOutDate - checkInDate).Days} night(s)) ---");
        List<Room> availableRooms = GetAvailableRooms(checkInDate, checkOutDate, numPersons);

        if (availableRooms.Count == 0)
        {
            Console.WriteLine("No available rooms matching your criteria.");
            return;
        }

        foreach (var room in availableRooms)
        {
            decimal totalPrice = room.PricePerNight * (checkOutDate - checkInDate).Days;
            Console.WriteLine($"Room {room.RoomId} - Type: {room.RoomType}, Capacity: {room.Capacity}, Price: ${room.PricePerNight}/night (Total: ${totalPrice})");
        }
    }

    // Get Available Rooms
    private List<Room> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate, int numPersons)
    {
        List<Room> available = new List<Room>();

        foreach (var room in rooms)
        {
            // Check if room capacity meets requirement
            if (room.Capacity < numPersons)
                continue;

            // Check if room is available for the entire stay
            bool isBooked = reservations.Any(r =>
                r.RoomId == room.RoomId &&
                r.Status == "Confirmed" &&
                !(checkOutDate <= r.CheckInDate || checkInDate >= r.CheckOutDate));

            if (!isBooked)
                available.Add(room);
        }

        return available;
    }

    // Make a Reservation
    public bool MakeReservation(int roomId, DateTime checkInDate, DateTime checkOutDate, int numPersons)
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please login first!");
            return false;
        }

        if (checkInDate >= checkOutDate)
        {
            Console.WriteLine("Invalid dates! Check-out date must be after check-in date.");
            return false;
        }

        Room room = rooms.FirstOrDefault(r => r.RoomId == roomId);
        if (room == null)
        {
            Console.WriteLine($"Room {roomId} not found!");
            return false;
        }

        if (room.Capacity < numPersons)
        {
            Console.WriteLine($"Room {roomId} cannot accommodate {numPersons} persons. Max capacity: {room.Capacity}");
            return false;
        }

        // Check if room is available
        if (!GetAvailableRooms(checkInDate, checkOutDate, numPersons).Any(r => r.RoomId == roomId))
        {
            Console.WriteLine($"Room {roomId} is not available for the selected dates.");
            return false;
        }

        int reservationId = reservations.Count > 0 ? reservations.Max(r => r.ReservationId) + 1 : 1;
        decimal totalPrice = room.PricePerNight * (checkOutDate - checkInDate).Days;

        Reservation reservation = new Reservation
        {
            ReservationId = reservationId,
            UserId = currentUser.UserId,
            RoomId = roomId,
            CheckInDate = checkInDate,
            CheckOutDate = checkOutDate,
            NumPersons = numPersons,
            TotalPrice = totalPrice,
            Status = "Confirmed",
            BookingDate = DateTime.Now
        };

        reservations.Add(reservation);
        Console.WriteLine($"\n✓ Reservation successful!");
        Console.WriteLine($"Reservation ID: {reservationId}");
        Console.WriteLine($"Room {roomId} ({room.RoomType}) - {numPersons} person(s)");
        Console.WriteLine($"Check-in: {checkInDate:yyyy-MM-dd}");
        Console.WriteLine($"Check-out: {checkOutDate:yyyy-MM-dd}");
        Console.WriteLine($"Total Price: ${totalPrice}");
        return true;
    }

    // View My Reservations
    public void ViewMyReservations()
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please login first!");
            return;
        }

        var myReservations = reservations.Where(r => r.UserId == currentUser.UserId).ToList();

        if (myReservations.Count == 0)
        {
            Console.WriteLine("You have no reservations.");
            return;
        }

        Console.WriteLine($"\n--- Your Reservations ---");
        foreach (var reservation in myReservations)
        {
            Room room = rooms.FirstOrDefault(r => r.RoomId == reservation.RoomId);
            Console.WriteLine($"Reservation ID: {reservation.ReservationId}");
            Console.WriteLine($"Room {reservation.RoomId} ({room.RoomType}) - {reservation.NumPersons} person(s)");
            Console.WriteLine($"Check-in: {reservation.CheckInDate:yyyy-MM-dd} | Check-out: {reservation.CheckOutDate:yyyy-MM-dd}");
            Console.WriteLine($"Total Price: ${reservation.TotalPrice} | Status: {reservation.Status}");
            Console.WriteLine();
        }
    }

    // Cancel Reservation
    public bool CancelReservation(int reservationId)
    {
        if (currentUser == null)
        {
            Console.WriteLine("Please login first!");
            return false;
        }

        Reservation reservation = reservations.FirstOrDefault(r => r.ReservationId == reservationId && r.UserId == currentUser.UserId);
        if (reservation == null)
        {
            Console.WriteLine("Reservation not found!");
            return false;
        }

        if (reservation.CheckInDate <= DateTime.Now)
        {
            Console.WriteLine("Cannot cancel a reservation that has already started!");
            return false;
        }

        reservation.Status = "Cancelled";
        Console.WriteLine($"Reservation {reservationId} has been cancelled.");
        return true;
    }
}

// User Class
public class User
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

// Room Class
public class Room
{
    public int RoomId { get; set; }
    public string RoomType { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
}

// Reservation Class
public class Reservation
{
    public int ReservationId { get; set; }
    public int UserId { get; set; }
    public int RoomId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumPersons { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }
    public DateTime BookingDate { get; set; }
}

// Demo Program
public class Program
{
    static void Main()
    {
        HotelBookingSystem system = new HotelBookingSystem();

        // Demo: Register and Login
        Console.WriteLine("=== Hotel Booking System Demo ===\n");
        system.Register("john_doe", "password123", "John Doe", "john@example.com");
        system.Login("john_doe", "password123");

        // Demo: View Available Rooms
        DateTime checkIn = DateTime.Now.AddDays(5);
        DateTime checkOut = checkIn.AddDays(3);
        system.ViewAvailableRooms(checkIn, checkOut, 2);

        // Demo: Make Reservation
        Console.WriteLine("\n--- Making Reservation ---");
        system.MakeReservation(201, checkIn, checkOut, 2);

        // Demo: View My Reservations
        system.ViewMyReservations();

        // Demo: Logout
        Console.WriteLine("--- Logging Out ---");
        system.Logout();
    }
}
