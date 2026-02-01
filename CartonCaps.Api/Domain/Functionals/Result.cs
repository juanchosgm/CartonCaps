namespace CartonCaps.Api.Domain.Functionals;

public class ResultError
{
    private ResultError(string message)
    {
        Key = string.Empty;
        Message = message ?? throw new ArgumentNullException();
    }

    public ResultError(string key, string message)
    {
        Key = key ?? throw new ArgumentNullException();
        Message = message ?? throw new ArgumentNullException();
    }

    public string Key { get; }
    public string Message { get; }

    public static ResultError Create(string key, string message) => new ResultError(key, message);
    public static implicit operator ResultError(string error) => new ResultError(error);
}

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IEnumerable<ResultError> Errors { get; }
    public string StringErrors => Errors is null ? string.Empty : string.Join(" ", Errors.Select(e => e.Message));
    private Result()
    {
        if (IsSuccess && Errors != null && Errors.Any())
        {
            throw new InvalidOperationException();
        }
    }
    protected Result(bool isSuccess, params ResultError[] errors)
        : this()
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }
    protected Result(bool isSuccess, IEnumerable<ResultError> errors) : this()
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public override string ToString()
    {
        return IsSuccess.ToString();
    }

    public static Result Fail(params ResultError[] errors) => new Result(false, errors);
    public static Result Fail(IEnumerable<ResultError> errors) => new Result(false, errors);
    public static Result Ok() => new Result(true);
    public static Result<T> Fail<T>(params ResultError[] errors) => new Result<T>(default, false, errors);
    public static Result<T> Fail<T>(IEnumerable<ResultError> errors) => new Result<T>(default, false, errors);
    public static Result<T> Ok<T>(T value) => new Result<T>(value, true);

    public static implicit operator bool(Result result) => result?.IsSuccess ?? false;
}

public class Result<T> : Result
{
    private readonly T _value;
    public T Value => !IsSuccess ? throw new InvalidOperationException() : _value;
    protected internal Result(T value, bool isSuccess, params ResultError[] errors) : base(isSuccess, errors) => _value = value;
    protected internal Result(T value, bool isSuccess, IEnumerable<ResultError> errors) : base(isSuccess, errors) => _value = value;
}
