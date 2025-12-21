namespace Waggle.NotificationWorker.Services
{
    public interface IEmailTemplateService
    {
        Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> values);
    }
}
