namespace OpenRA.Updater
{
    interface IUpdater
    {
        void Initialize();
        void CheckForUpdate();
        void Cleanup();
        bool HaveUpdate();
        bool HaveCancelledUpdate();
    }
}
