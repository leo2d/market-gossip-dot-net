namespace MarketGossip.Shared.Dtos.Result;

public partial struct Result<T> : IResult<T>
{
    public T Value { get; }

    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;
    public string Error { get; }

    internal Result(bool isFailure, string error, T value)
    {
        IsFailure = isFailure;
        Error = error;
        Value = value;
    }
}