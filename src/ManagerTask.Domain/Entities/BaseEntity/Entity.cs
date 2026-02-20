using ManagerTask.Domain.Entities.Events;

namespace ManagerTask.Domain.Entities.BaseEntity;

public class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new(); 
    
    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}