using Microsoft.EntityFrameworkCore;
using StreetSweepingReminder.Api.Entities;

namespace StreetSweepingReminder.Api.DbContext;

public static class DataSeeder
{
    public static async Task SeedStreetsAsync(ApplicationDbContext context, string csvFilePath)
    {
        if (await context.Streets.AnyAsync())
        {
            Console.WriteLine("Streets table already seeded. Skipping.");
            return;
        }

        Console.WriteLine($"Seeding Streets from CSV: {csvFilePath}...");
        if (!File.Exists(csvFilePath))
        {
            await Console.Error.WriteLineAsync($"Seed file not found: {csvFilePath}");
            return;
        }

        var streetsToInsert = new List<Street>();
        var lines = await File.ReadAllLinesAsync(csvFilePath);

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split(',', StringSplitOptions.TrimEntries);

            if (parts.Length == 2)
            {
                var streetName = parts[0].Trim('"');
                var zipCodeString = parts[1].Trim('"');
                
                if (int.TryParse(zipCodeString, out int zipCodeInt))
                {
                    
                    if (!string.IsNullOrEmpty(streetName))
                    {
                        streetsToInsert.Add(new Street
                        {
                            StreetName = streetName,
                            ZipCode = zipCodeInt
                        });
                    }
                    else
                    {
                        Console.WriteLine($"Skipping line with empty street name: {line}");
                    }
                }
                else
                {
                    Console.WriteLine($"Skipping line due to invalid zip code format: {line}");
                }
            }
            else
            {
                Console.WriteLine($"Skipping line with incorrect format (expected 2 parts): {line}");
            }
        }

        if (streetsToInsert.Any())
        {
            const int batchSize = 500;
            Console.WriteLine($"Preparing to insert {streetsToInsert.Count} streets in batches of {batchSize}...");
            for (var i = 0; i < streetsToInsert.Count; i += batchSize)
            {
                var batch = streetsToInsert.Skip(i).Take(batchSize).ToList();
                await context.Streets.AddRangeAsync(batch);
                try
                {
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Inserted batch {i / batchSize + 1}. Total processed: {Math.Min(i + batchSize, streetsToInsert.Count)}");
                }
                catch (Exception ex)
                {
                     await Console.Error.WriteLineAsync($"Error saving batch starting at index {i}: {ex.Message}");
                     throw;
                }
            }
            Console.WriteLine($"Successfully seeded {streetsToInsert.Count} streets.");
        }
        else
        {
             Console.WriteLine("No valid street data found to seed in the CSV file.");
        }
    }
}