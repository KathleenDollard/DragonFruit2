//using System.Diagnostics.CodeAnalysis;

//namespace DragonFruit2.Defaults;

//public struct DependentValue<T>
//    where T : struct
//{
//    public T? Value { get; }
//    public string? DependentName { get; }

//    public DependentValue(T value)
//    {
//        Value = value;
//    }

//    public DependentValue(string dependentName)
//    {
//        DependentName = dependentName;
//    }

//    public T? GetValue(DataValues dataValues)
//    {
//        T workingValue;
//        if (Value.HasValue)
//        {
//            workingValue = Value.Value;
//        }
//        else
//        {
//            if (TryGetValueFromDependentData(dataValues, out workingValue))
//                { }
//        }


//        if (DependentName is not null)
//        {
//            // TODO: The following code, and all the dependent value code, probably does not handle nullable ValueTypes correctly
//            if (dataValues.TryGetValue<T>(DependentName, out var current))
//            {
//                return current.Value;
//            }
//        }
//        return default;
//    }

//    private bool TryGetValueFromDependentData(DataValues dataValues, [NotNullWhen(true)] out T workingValue)
//    {
//        var dataValue = dataValues.TryGetValue<T>(DependentName, out var interimValue);

//    }
//}
