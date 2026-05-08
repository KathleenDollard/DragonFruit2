namespace DragonFruit2;

public interface IOperationOnMemberDefinition<TReturn>
{
    TReturn Operate<TValue>(MemberDataDefinition<TValue> memberDefinition);
}

public interface IOperateOnDataValue<TRootCommand, TReturn>
{
    bool TryOperate<TValue>(DataValue<TValue> dataValue,IOperateOnDataValue<TRootCommand, TReturn> operation, out TReturn returnValue);
    Result<TRootCommand> Result { get; }
    string OperationName { get; }
}



