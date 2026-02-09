using System.Diagnostics.CodeAnalysis;

namespace DragonFruit2.Defaults;

public static class DefaultOffsetNumeric
{
    public static DefaultOffsetInt32 Create(int start, int offset)
    {
        return new DefaultOffsetInt32(start, offset);
    }

    public static DefaultOffsetInt32 Create(string name, int offset)
    {
        return new DefaultOffsetInt32(name, offset);
    }

    // TODO: Add Create methods and stub classes for other numeric types, ensuring the approach works in .NET Standard and can be consumed by C# 7.3

    public class DefaultOffset<TValue> : DefaultDefinition<TValue>
            where TValue : struct
    {
        private readonly TValue? _start;
        private readonly string? _dependentStart;
        private readonly TValue _offset;
        private readonly Func<TValue, TValue, TValue> _valueGetter;

        protected DefaultOffset(TValue start, TValue offset, Func<TValue, TValue, TValue> valueGetter)
            : base($"offset from {start} by {offset}.")
        {
            _start = start;
            _offset = offset;
            _valueGetter = valueGetter;
        }
        protected DefaultOffset(string dependentValueName, TValue offset, Func<TValue, TValue, TValue> valueGetter)
            : base($"offset from {dependentValueName} by {offset}.")
        {
            _dependentStart = dependentValueName;
            _offset = offset;
            _valueGetter = valueGetter;
        }

        public override bool TryGetDefaultValue(DataValues dataValues, [NotNullWhen(true)] out TValue value)
        {
            var maybeStart = GetValue(dataValues, _start, _dependentStart);
            if (maybeStart.HasValue)
            {
                var start = maybeStart.Value;
                if (!typeof(int).IsAssignableFrom(start.GetType()))
                {
                    throw new InvalidOperationException($"The starting value is not an integer type, it is {start.GetType().Name}");
                }
                value = _valueGetter(start, _offset);
                return true;
            }
            value = default!;
            return false;
        }



        private TValue? GetValue(DataValues dataValues, TValue? value, string? dependentName)
        {
            if (value.HasValue)
            {
                return value.Value;
            }

            if (dependentName is not null)
            {
                // TODO: The following code, and all the dependent value code, may not handle nullable ValueTypes correctly
                if (dataValues.TryGetValue<TValue>(dependentName, out var current))
                {
                    return current.Value;
                }
            }
            return default;
        }
    }

    public class DefaultOffsetInt32 : DefaultOffset<int>
    {
        internal DefaultOffsetInt32(int start, int offset)
        : base(start, offset, (start, offset) => start + offset)
        { }
        internal DefaultOffsetInt32(string dependentValueName, int offset)
            : base(dependentValueName, offset, (start, offset) => start + offset)
        {  }
    }
}

