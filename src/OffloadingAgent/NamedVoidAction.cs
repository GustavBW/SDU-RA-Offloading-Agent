using System;

namespace OffloadingAgent;

public class NamedVoidAction
{
    public string Name { get; }
    public Action Action { get; }

    public NamedVoidAction(string name, Action action)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Action = action ?? throw new ArgumentNullException(nameof(action));
    }
}
