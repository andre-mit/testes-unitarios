using System;
using Flunt.Notifications;

namespace Store.Domain.Entities;
public abstract class Entity : Notifiable<Notification>
{
    public Guid Id { get; private set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }
}