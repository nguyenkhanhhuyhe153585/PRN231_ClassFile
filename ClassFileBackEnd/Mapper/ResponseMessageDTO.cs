namespace ClassFileBackEnd.Mapper
{
    public class ResponseMessageDTO<T>
    {
        private string message;
        private T? data;

        public ResponseMessageDTO(string message)
        {
            this.message = message;
        }

        public string Message { get { return message; } set { message = value; } }
        public T? Data { get { return data; } set { data = value; } }
    }
}
