namespace Team_Task_Manager.Extesions
{
    public static class FluentEmail
    {
        public static IServiceCollection AddFluentEmail (this IServiceCollection services , ConfigurationManager configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            var defaultFromEmail = emailSettings["DefaultFromEmail"];
            var host = emailSettings["SMTPSetting:Host"];
            var port = emailSettings.GetValue<int>("Port");

            services.AddFluentEmail("kokoeslam62@gmail.com")
                    .AddSmtpSender("smtp.gmail.com", 587, "kokoeslam62@gmail.com", "joixzivkiijestzg");

            return services;
        }
    }
}
