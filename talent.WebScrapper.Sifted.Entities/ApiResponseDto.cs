namespace talentX.WebScrapper.Sifted.Entities
{
    public class ApiResponseDto<T>
    {
        public T Data { get; set; }
        public bool isSuccess { get; set; }
    }
}
