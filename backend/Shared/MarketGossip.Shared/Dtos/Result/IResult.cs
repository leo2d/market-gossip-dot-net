namespace MarketGossip.Shared.Dtos.Result;

public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
}

public interface IValue<out T>
{
    T Value { get; }
}

public interface IError<out E>
{
    E Error { get; }
}

public interface IResult<out T, out E> : IResult, IValue<T>, IError<E>
{
}

public interface IResult<out T> : IResult<T, string>
{
}

public interface IUnitResult<out E> : IResult, IError<E>
{
}
public interface IUnitResult : IUnitResult<string>
{
}