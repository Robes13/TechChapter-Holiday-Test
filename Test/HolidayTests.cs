using NUnit.Framework;
using System;

namespace HolidayCalendar.Tests;

public class HolidayTests
{
    private readonly IHolidayCalendar fixture = new HolidayCalendar();

    [Test]
    public void GIVEN_XmasDay_WHEN_IsHoliday_THEN_return_true()
    {
        // Arrange
        var date = new DateTime(2023, 12, 25);

        // Act
        var result = fixture.IsHoliday(date);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void GIVEN_regular_weekday_WHEN_IsHoliday_THEN_return_false()
    {
        // Arrange
        var date = new DateTime(2023, 4, 21);

        // Act
        var result = fixture.IsHoliday(date);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void GIVEN_April2023_WHEN_GetHolidays_THEN_return_EasterDays()
    {
        // Arrange
        var startDate = new DateTime(2023, 4, 1);
        var endDate = new DateTime(2023, 4, 30);

        // Act
        var result = fixture.GetHolidays(startDate, endDate);

        // Assert
        Assert.IsTrue(result.Contains(new DateTime(2023, 4, 6))); // Maundy Thursday
        Assert.IsTrue(result.Contains(new DateTime(2023, 4, 7))); // Good Friday
        Assert.IsTrue(result.Contains(new DateTime(2023, 4, 9))); // Easter Sunday
        Assert.IsTrue(result.Contains(new DateTime(2023, 4, 10))); // Easter Monday
        Assert.AreEqual(4, result.Count);
    }
}
