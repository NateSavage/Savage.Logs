using Godot;
using Savage.Logs.LogDecorations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Savage.Logs;

public class NetIdAttachmentAttachmentGenerator : AttachmentGenerator<ClientServerDecoration> {

    public override ThreadRequirement ThreadRequirement => ThreadRequirement.Main;


    private ENetMultiplayerPeer multiplayerPeer;

    public NetIdAttachmentAttachmentGenerator(ENetMultiplayerPeer multiplayerPeer) {
        this.multiplayerPeer = multiplayerPeer;
    }

    public override MessageAttachment CreateAttachmentFor(ref LogEntry logEntry) {
        return new NetIdDecoration(multiplayerPeer.GetUniqueId());
    }

}
