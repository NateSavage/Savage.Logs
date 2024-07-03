using Godot;

namespace Savage.Logs;

public class ClientServerDecorationGenerator : DecorationGenerator<ClientServerDecoration> {

    public override ThreadRequirement ThreadRequirement => ThreadRequirement.Main;


    private ENetMultiplayerPeer multiplayerPeer;

    public ClientServerDecorationGenerator(ENetMultiplayerPeer multiplayerPeer) {
        this.multiplayerPeer = multiplayerPeer;
    }

    public override LogDecoration Emit(ref LogEntry logEntry) {

        return new ClientServerDecoration(isServer: multiplayerPeer.GetUniqueId() == 1);
    }
}
