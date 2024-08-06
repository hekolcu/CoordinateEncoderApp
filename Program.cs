using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public struct Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinate(int X, int Y) {
        this.X = X;
        this.Y = Y;
    }
}

public static class CoordinateEncoder
{
    private const int BitWidth = 16;
    private const int MaxValue = (1 << BitWidth) - 1; // 65535, which covers the range 0-999

    public static int Encode(Coordinate c)
    {
        return (c.X << BitWidth) | c.Y;
    }

    public static Coordinate Decode(int encoded)
    {
        int x = encoded >> BitWidth;
        int y = encoded & MaxValue;
        return new Coordinate { X = x, Y = y };
    }
}

public class Program
{
    public static void Main()
    {
        const int numCoordinates = 1000000;
        var random = new Random();
        var coordinates = new Coordinate[numCoordinates];
        var encodedCoordinates = new int[numCoordinates];

        // Generate random coordinates
        for (int i = 0; i < numCoordinates; i++)
        {
            coordinates[i] = new Coordinate(random.Next(1000), random.Next(1000));
        }
        Console.WriteLine(coordinates[numCoordinates - 1]);

        // Time the creation and saving of coordinates.json
        var stopwatch = Stopwatch.StartNew();
        string coordinatesJson = JsonSerializer.Serialize(coordinates);
        File.WriteAllText("coordinates.json", coordinatesJson);
        stopwatch.Stop();
        Console.WriteLine($"Creating and saving coordinates.json took: {stopwatch.ElapsedMilliseconds} ms");
        
        // Time the creation and saving of encodedCoordinates.json
        stopwatch.Restart();
        // Encode coordinates
        for (int i = 0; i < numCoordinates; i++)
        {
            encodedCoordinates[i] = CoordinateEncoder.Encode(coordinates[i]);
        }

        Console.WriteLine(encodedCoordinates[numCoordinates - 1]);

        string encodedCoordinatesJson = JsonSerializer.Serialize(encodedCoordinates);
        File.WriteAllText("encodedCoordinates.json", encodedCoordinatesJson);
        stopwatch.Stop();
        Console.WriteLine($"Creating and saving encodedCoordinates.json took: {stopwatch.ElapsedMilliseconds} ms");

        // Time the reading and decoding of coordinates.json
        stopwatch.Restart();
        string readCoordinatesJson = File.ReadAllText("coordinates.json");
        var readCoordinates = JsonSerializer.Deserialize<Coordinate[]>(readCoordinatesJson);
        Console.WriteLine(readCoordinates[numCoordinates - 1]);
        stopwatch.Stop();
        Console.WriteLine($"Reading and decoding coordinates.json took: {stopwatch.ElapsedMilliseconds} ms");

        // Time the reading and decoding of encodedCoordinates.json
        stopwatch.Restart();
        string readEncodedCoordinatesJson = File.ReadAllText("encodedCoordinates.json");
        var readEncodedCoordinates = JsonSerializer.Deserialize<int[]>(readEncodedCoordinatesJson);
        var decodedCoordinates = new Coordinate[numCoordinates];
        for (int i = 0; i < numCoordinates; i++)
        {
            decodedCoordinates[i] = CoordinateEncoder.Decode(readEncodedCoordinates[i]);
        }
        Console.WriteLine(decodedCoordinates[numCoordinates - 1]);
        stopwatch.Stop();
        Console.WriteLine($"Reading and decoding encodedCoordinates.json took: {stopwatch.ElapsedMilliseconds} ms");
    }
}
