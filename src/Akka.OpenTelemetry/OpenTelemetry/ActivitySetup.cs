using System.Diagnostics;

namespace Akka.OpenTelemetry;

public delegate void ActivitySetup(Activity activity, object message);