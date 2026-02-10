namespace DragonFruit2;

public interface IOperationOnDataValue
{
    TReturn Operate<TReturn, TValue>(DataValue<TValue> dataValue);
}

public interface IOperationOnMemberDefinition<TReturn>
{
    TReturn Operate<TValue>(MemberDataDefinition<TValue> memberDefinition);
}


