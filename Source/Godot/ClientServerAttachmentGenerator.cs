using Godot;

namespace Savage.Logs;

public class ClientServerAttachmentGenerator : AttachmentGenerator<ClientServerDecoration> {

    public override ThreadRequirement ThreadRequirement => ThreadRequirement.Main;


    private ENetMultiplayerPeer multiplayerPeer;

    public ClientServerAttachmentGenerator(ENetMultiplayerPeer multiplayerPeer) {
        this.multiplayerPeer = multiplayerPeer;
    }

    public override MessageAttachment CreateAttachmentFor(ref LogEntry logEntry) {

        return new ClientServerDecoration(isServer: multiplayerPeer.GetUniqueId() == 1);
    }
}
