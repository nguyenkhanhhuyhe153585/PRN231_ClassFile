namespace ClassFileBackEnd.Mapper
{
    public class TokenResponseDTO<T>
    {
        private string token;
        private T? data;

        public TokenResponseDTO(string token)
        {
            this.token = token;
        }

        public string Token { get { return token; } set { token = value; } }
        public T? Data { get { return data; } set { data = value; } }

    }
}
