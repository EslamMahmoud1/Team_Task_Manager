namespace Team_Task_Manager.Shared
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }
        public List<string> Errors { get; private set; } = new List<string>();

        private Result(bool isSuccess , T value , List<string> erros)
        {
            IsSuccess  = isSuccess;
            Value = value;
            Errors = erros;
        }

        public static Result<T> Success (T value)
        {
            return new Result<T>(true, value, new List<string>());
        }
        public static Result<T> Failure (List<string> errors)
        {
            return new Result<T>(false, default, errors);
        }
    }
}
