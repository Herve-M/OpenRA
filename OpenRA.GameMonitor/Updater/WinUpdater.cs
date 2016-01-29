using System;
using System.Threading;
using System.Windows.Forms;
using WinSparkleDotNet;

namespace OpenRA.Updater
{
    class WinUpdater : IUpdater
    {
        private readonly WinSparkleNet _sparkleNet;
        private readonly AutoResetEvent _updatEvent;
        private bool _haveUpdate;
        private bool _haveCancelledUpdate;

        public WinUpdater(AutoResetEvent updateEvent)
        {
            try
            {
                _sparkleNet = new WinSparkleNet();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Error was : " + ex, @"Error during SparkleNet instantiation",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _sparkleNet.SetDidFindUpdateCallback(DidFindUpdate);
            _sparkleNet.SetDidNotFindUpdateCallback(DidNotFindUpdate);
            _sparkleNet.SetUpdateCancelledCallback(CancelledUpdate);
            _updatEvent = updateEvent;
        }

        public void Initialize()
        {
            _sparkleNet.Initialize();
        }

        public void CheckForUpdate()
        {
            _sparkleNet.CheckForUpdate();
        }

        public void Cleanup()
        {
            _sparkleNet.Cleanup();
        }

        public bool HaveUpdate()
        {
            return _haveUpdate;
        }

        public bool HaveCancelledUpdate()
        {
            return _haveCancelledUpdate;
        }

        private void DidNotFindUpdate()
        {
            _haveUpdate = false;
            _updatEvent.Set();
        }

        private void DidFindUpdate()
        {
            _haveUpdate = true;
            _updatEvent.Set();
        }

        private void CancelledUpdate()
        {
            _haveCancelledUpdate = true;
            _updatEvent.Set();
        }
    }
}
