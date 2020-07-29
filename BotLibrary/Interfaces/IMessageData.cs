using System;
using System.Collections.Generic;
using System.Text;
using BotLibrary.Enums;

namespace BotLibrary.Interfaces
{
    public interface IMessageData
    {
        OutboxMessageType Type { get; }
        object Data { get; }
        List<IMessageData> NestedElements { get; }
    }
}
