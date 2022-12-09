using System.Diagnostics;

namespace Asynkron.Akka.OpenTelemetry;

public delegate void ActivitySetup(Activity activity, object message);