using talentX.WebScrapper.Sifted.Entities;

namespace talentX.WebScrapper.Sifted.Utils
{
    public static class ResponseUtils
    {

        public static ApiResponseDto<T> GetSuccesfulResponse<T>(T data)
        {
            return new()
            {
                Data = data,
                isSuccess = true,
            };
        }

        public static ApiResponseDto<T> GetBadRequestResponse<T>(T data)
        {
            return new()
            {
                Data = data,
                isSuccess = false,
            };
        }

    }
}
