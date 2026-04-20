namespace DragonFruit2;

public interface IOperationOnMemberDefinition<TReturn>
{
    TReturn Operate<TValue>(MemberDataDefinition<TValue> memberDefinition);
}

public interface IOperateOnDataValue<TRootArgs, TReturn>
    where TRootArgs : CommandRootBase<TRootArgs>
{
    bool TryOperate<TValue>(DataValue<TValue> dataValue,IOperateOnDataValue<TRootArgs, TReturn> operation, out TReturn returnValue);
    Result<TRootArgs> Result { get; }
    string OperationName { get; }
}



