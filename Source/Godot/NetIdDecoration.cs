using System;
using System.Globalization;

namespace Savage.Logs;

public class NetIdDecoration : LogDecoration {
    public override string Tag => "NetId";
    public override bool ShowTag => true;

    public override string Value => value;
    private readonly string value;

    public override Type Type => typeof(int);

    public override DisplayLocation Location => DisplayLocation.InlinePreceding;
    public override int DisplayPriority => -3_000;

    public NetIdDecoration(int id) {
        value = id.ToString(CultureInfo.InvariantCulture);
    }
}
