using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public class DefaultDateTimeOffset : DefaultDefinition<DateTime>
{
    private readonly DateTime? _start;
    private readonly string? _dependentStart;
    private readonly int _years;
    private readonly int _months;
    private readonly int _days;
    private readonly int _hours;
    private readonly int _minutes;
    private readonly int _seconds;

    private DefaultDateTimeOffset(bool _, string description, int years = 0, int months = 0, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
    : base(description)
    {
        _years = years;
        _months = months;
        _days = days;
        _hours = hours;
        _minutes = minutes;
        _seconds = seconds;
    }
    public DefaultDateTimeOffset(int years = 0, int months = 0, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        : this(true, "offset from now. Negative values are in the past.", years, months, days, hours, minutes, seconds)
    {
        _start = DateTime.Now;
    }
    public DefaultDateTimeOffset(DateTime start, int years = 0, int months = 0, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        : this(true, "offset from a specified date. Negative values are in the past.", years, months, days, hours, minutes, seconds)
    {
        _start = start;
    }
    public DefaultDateTimeOffset(string dependentValue, int years = 0, int months = 0, int days = 0, int hours = 0, int minutes = 0, int seconds = 0)
        : this(true, "offset from a specified date. Negative values are in the past.", years, months, days, hours, minutes, seconds)
    {
        _dependentStart = dependentValue;
    }



    public override bool TryGetDefaultValue(DataValues dataValues, [NotNullWhen(true)] out DateTime value)
    {
        var maybeStartDate = GetValue(dataValues, _start, _dependentStart);
        if (maybeStartDate.HasValue)
        {
            var returnDate = maybeStartDate.Value;
            if (_years != 0) returnDate = returnDate.AddYears(_years);
            if (_months != 0) returnDate = returnDate.AddMonths(_months);
            if (_days != 0) returnDate = returnDate.AddDays(_days);
            if (_hours != 0) returnDate = returnDate.AddHours(_days);
            if (_minutes != 0) returnDate = returnDate.AddMinutes(_minutes);
            if (_seconds != 0) returnDate = returnDate.AddSeconds(_seconds);
            value = returnDate;
            return true;
        }
        value = default!;
        return false;
    }

    private T? GetValue<T>(DataValues dataValues, T? value, string? dependentName)
        where T : struct
    {
        if (value.HasValue)
        {
            return value.Value;
        }

        if (dependentName is not null)
        {
            // TODO: The following code, and all the dependent value code, probably does not handle nullable ValueTypes correctly
            if (dataValues.TryGetValue<T>(dependentName, out var current))
            {
                return current.Value;
            }
        }
        return default;
    }
}
