namespace DragonFruit2.Defaults;

#if NET60_OR_GREATER
public class DefaultDateOnlyOffset : DefaultDefinition<DateOnl.y>
{
    private readonly DateOnly _start;
    private readonly int _years;
    private readonly int _months;
    private readonly int _days;
    private readonly int _minutes;
    private readonly int _seconds;

    public DefaultDateOnlyOffset( int years = 0, int months = 0, int days = 0)
         : base("offset from now. Negative values are in the past.")
    {
        _start = DateOnly.Now;
        _years = years;
        _months = months;
        _days = days;
    }

    public DefaultDateOnlyOffset(DateOnly start, int years = 0, int months = 0, int days = 0)
         : base ("offset from a specified date. Negative values are in the past.")
   {
        _start = start;
        _years = years;
        _months = months;
        _days = days;
    }

    public override DateOnly GetDefault(DataValues dataValues)
    {
        var returnDate = _start;
        if (_years != 0) returnDate = returnDate.AddYears(_years);
        if (_months != 0) returnDate = returnDate.AddMonths(_months);
        if (_days != 0) returnDate = returnDate.AddDays(_days);
        return returnDate;
    }
}
#endif