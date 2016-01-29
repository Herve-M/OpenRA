using System;
using System.Threading;
using OpenRA.Updater;

namespace OpenRA
{
    public class GameUpdater
    {
        private readonly IUpdater _updater;
        private readonly GameLaunchCallback _gameLaunch;
        private readonly string[] _args;
        private readonly AutoResetEvent _updatEvent = new AutoResetEvent(false);
       
        public delegate void GameLaunchCallback(string[] args);
        public GameUpdater(GameLaunchCallback callback, string[] args)
        {
            _gameLaunch = callback;
            _args = args;
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;

            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    _updater = new WinUpdater(_updatEvent);
                    break;
                case PlatformID.Unix:
                    //TODO
                    break;
#if __MonoCS__
                case PlatformID.MacOSX:
                    _updater = new OSXUpdater();
                    break;
#endif
                default:
                {
                    //Unix old Mono ID
                    if ((int) pid == 128)
                    {
                        //TODO
                    }
                }
                break;
            }
        }

        public void CheckForUpdate()
        {
            _updater.CheckForUpdate();
            // Wait for updater to check for an update
            _updatEvent.WaitOne(-1);
        }

        public void Cleanup()
        {
            if (_updater.HaveUpdate())
            {
                _updatEvent.Reset();
                //Wait user to install or not update
                _updatEvent.WaitOne(-1);
            }

            _updater.Cleanup();

            if (!_updater.HaveUpdate() || _updater.HaveCancelledUpdate())
            {
                _gameLaunch(_args);
            }
        }
    }
}
