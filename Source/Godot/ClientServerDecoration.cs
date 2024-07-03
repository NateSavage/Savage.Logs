using System;

namespace Savage.Logs;

/// <summary> Simplified decoration that simply displays "Server" if the network peer is the host or "Client" if they aren't. </summary>
public class ClientServerDecoration : LogDecoration {
    public override string Tag => tag;
    public string tag;
    public override bool ShowTag => true;

    public override string Value => "";

    public override Type Type => typeof(int);

    public override DisplayLocation Location => DisplayLocation.InlinePreceding;
    public override int DisplayPriority => -3_001;

    public ClientServerDecoration(bool isServer) {
        tag = isServer ? String.Intern("Server") : String.Intern("Client");
    }
}
