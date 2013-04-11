namespace Urdms.NotificationService.UserSearch
{
    public class JsonResponse<T>
    {
        public string [] Errors { get; set; }
        public string Success { get; set; }
        public T Data { get; set; }
    }
}
