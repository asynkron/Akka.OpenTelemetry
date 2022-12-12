using System.Diagnostics;

namespace Akka.OpenTelemetry.Telemetry;

public delegate void ActivitySetup(Activity activity, object message);