using Godot;
using Savage.Logs.LogDecorations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Savage.Logs;

public class NetIdDecorationDecorationGenerator : DecorationGenerator<ClientServerDecoration> {

    public override ThreadRequirement ThreadRequirement => ThreadRequirement.Main;


    private ENetMultiplayerPeer multiplayerPeer;

    public NetIdDecorationDecorationGenerator(ENetMultiplayerPeer multiplayerPeer) {
        this.multiplayerPeer = multiplayerPeer;
    }

    public override LogDecoration Emit(ref LogEntry logEntry) {
        return new NetIdDecoration(multiplayerPeer.GetUniqueId());
    }

}
