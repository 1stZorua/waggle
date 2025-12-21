namespace Waggle.NotificationWorker.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly string _templatePath;

        public EmailTemplateService()
        {
            _templatePath = Path.Combine(AppContext.BaseDirectory, "Templates");
        }

        public async Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> values)
        {
            var path = Path.Combine(_templatePath, templateName);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Email template not found: {path}");

            var content = await File.ReadAllTextAsync(path);
            foreach (var pair in values)
            {
                content = content.Replace($"{{{{{pair.Key}}}}}", pair.Value);
            }
            return content;
        }
    }
}
