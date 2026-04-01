using Mapster;
using Team_Task_Manager.Models.Entities.Task;
using Team_Task_Manager.ViewModels.Task;

namespace Team_Task_Manager.Extesions
{
    public class MappingConfiguration
    {
        public void ConfigureMappings()
        {
            TypeAdapterConfig<TaskItem, ShowTaskViewModel>.NewConfig()
                .Map(dest => dest.AssignedToName, src => src.AssignedTo.UserName)
                .Map(dest => dest.CreatedByName, src => src.CreatedBy.UserName);
                

        }
    }
}
