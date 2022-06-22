//Inspired by: https://github.com/vkhorikov/CSharpFunctionalExtensions

namespace MarketGossip.Shared.Dtos.Result;

public partial struct Result : IUnitResult
{
    public bool IsFailure { get; }
    public bool IsSuccess => !IsFailure;
    public string Error { get; }

    private Result(bool isFailure, string error)
    {
        IsFailure = isFailure;
        Error = error;
    }
}

public partial struct Result
{
    public static Result Success()
    {
        return new Result(false, default);
    }
    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(false, default, value);
    }
    public static Result Failure(string error)
    {
        return new Result(true, error);
    }
    public static Result<T> Failure<T>(string error)
    {
        return new Result<T>(true, error, default);
    }
}