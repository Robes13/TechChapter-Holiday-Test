using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace HolidayCalendar;
/// <summary>
/// This class is responsible for handling the logic and getting information about specific dates.
/// </summary>
public class HolidayCalendar : IHolidayCalendar
{
    /// <summary>
    /// This class is responsible for saving the event info received from the API.
    /// </summary>
    private class DayInfo
    {
        public required List<EventInfo> events { get; set; }
    }

    /// <summary>
    /// This class represents whether a day is a holiday or not. True / False.
    /// </summary>
    private class EventInfo
    {
        public bool holliday { get; set; }
    }

    /// <summary>
    /// This method fetches holiday data from the Kalendarium API for a given date. 
    /// It is used internally to determine whether a given date is a holiday or not.
    /// </summary>
    /// <param name="date"> The date to be retrieved from the API.</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<bool> FetchHolidayStatusAsync(DateTime date)
    {
        using var client = new HttpClient();
        // Creating our APIUrl with the our date variable set as the correct form for kalendarium API.
        string apiUrl = $"https://api.kalendarium.dk/Dayinfo/{date:yyyy-MM-dd}";
        var response = await client.GetAsync(apiUrl);

        // Throwing error if the responses was unsuccessful.
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to fetch data from API for date {date:yyyy-MM-dd}");
        }

        // JSON response from kalendarium API.
        var jsonResponse = await response.Content.ReadAsStringAsync();

        // Giving the response to our DayInfo class, which contains a list of events, since that's the only information we care about from the response.
        var dayInfo = JsonSerializer.Deserialize<DayInfo>(jsonResponse);

        // If the events are not null and there is at least 1 event in the response, we check if the date is a holiday, and if it is, we return true to represent that it is a holiday.
        if (dayInfo?.events != null && dayInfo.events.Count > 0)
        {
            foreach (var eventInfo in dayInfo.events)
            {
                if (eventInfo.holliday)
                {
                    return true;
                }
            }
        }
        // Return false if not a holiday.
        return false;
    }

    /// <summary>
    /// Checks whether a specific date is a holiday.
    /// </summary>
    /// <param name="date">The date to be tested</param>
    /// <returns></returns>
    public bool IsHoliday(DateTime date)
    {
        try
        {
            return FetchHolidayStatusAsync(date).Result;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Method for getting holidays between two specific dates.
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns></returns>
    public ICollection<DateTime> GetHolidays(DateTime startDate, DateTime endDate)
    {
        var holidays = new List<DateTime>();

        try
        {
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (FetchHolidayStatusAsync(date).Result)
                {
                    holidays.Add(date);
                }
            }
        }
        catch (Exception)
        {
            return new List<DateTime>();
        }

        return holidays;
    }
}
