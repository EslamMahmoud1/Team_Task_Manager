using Team_Task_Manager.Shared;

namespace Team_Task_Manager.Services.Interfaces
{
    public interface IEmailService
    {
        Task Send(EmailMetadata emailMetadata);
    }
}
