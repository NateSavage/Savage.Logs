using System;
using Microsoft.Extensions.Logging;

namespace Savage.Logs.LogDecorations {

public class MicrosoftEventIdAttachment : MessageAttachment {
    public override string Tag { get; }
    public override string Value { get; }
    public override Type Type => typeof(EventId);
    public override DisplayLocation Location { get; }

    public MicrosoftEventIdAttachment(EventId eventId) {
        
    }
}

}