

    public class ResponseModel<T>
    {
        public T? Result { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ResponseModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

