namespace HostPilot.Core.Services.Process;

public enum ServerStopMode
{
    /// <summary>Sends RCON "quit" and waits for the process to exit gracefully.</summary>
    RconQuit,
    /// <summary>Sends a close-window signal (WM_CLOSE / CloseMainWindow) and waits.</summary>
    CtrlC,
    /// <summary>Hard-kills the process tree immediately without a graceful period.</summary>
    KillAfterTimeout
}
