namespace Assignment3.Core;

public interface IWorkItemRepository
{
    (Response Response, int WorkItemId) Create(WorkItemCreateDTO workItem);
    IReadOnlyCollection<WorkItemDTO> ReadAll();
    //IReadOnlyCollection<WorkItemDTO> ReadAllRemoved();
    IReadOnlyCollection<WorkItemDTO> ReadAllByTag(string tag);
    IReadOnlyCollection<WorkItemDTO> ReadAllByUser(int userId);
    IReadOnlyCollection<WorkItemDTO> ReadAllByState(State state);
    WorkItemDetailsDTO Read(int workItemId);
    Response Update(WorkItemUpdateDTO workItem);
    Response Delete(int workItemId);
}
