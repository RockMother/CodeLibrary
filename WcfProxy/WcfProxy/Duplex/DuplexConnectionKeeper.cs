using System;
using System.Threading;
using TreeFrogs.WcfProxy.Interfaces;

namespace TreeFrogs.WcfProxy.Duplex
{
    /// <summary>
    /// Class allows to track duplex connection with server and restore it when it is failed. Also
    /// allows to invoke custom actions on connection initialize and fail.
    /// </summary>
    public sealed class DuplexConnectionKeeper : IDisposable
    {
        private readonly Timer restoreConnTimer;
        private static readonly TimeSpan RestoreConnTimerInterval = TimeSpan.FromSeconds(10);

        private readonly IBaseDuplexClientContract duplexService;
        private readonly Action initializeConnectionAction;
        private readonly Action connectionFailedAction;
        private readonly Action<string, Exception> writeErrorAction;
        private bool restoring;

        /// <summary> 
        /// Ctor
        /// </summary>
        /// <param name="duplexService">service to track connection</param>
        /// <param name="writeErrorAction">Write error</param>
        /// <param name="initializeConnectionAction">action used when connection is initialized</param>
        /// <param name="connectionFailedAction">action used when connection is faulted</param>
        public DuplexConnectionKeeper(IBaseDuplexClientContract duplexService, Action<string, Exception> writeErrorAction = null,
            Action initializeConnectionAction = null, Action connectionFailedAction = null)
        {
            this.duplexService = duplexService;
            this.initializeConnectionAction = initializeConnectionAction ?? (() => { });
            this.connectionFailedAction = connectionFailedAction ?? (() => { });
            this.writeErrorAction = writeErrorAction ?? ((m, e) => { });

            duplexService.ConnectionLost += DuplexServiceConnectionLost;

            restoreConnTimer = new Timer(s => RestoreConnection(), null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Initializes the connection first time and starts tracking
        /// </summary>
        public void InitializeConnection()
        {
            try
            {
                InitializeConnectionInternal();
            }
            catch (Exception ex)
            {
                writeErrorAction("Error in InitializeConnection:", ex);

                ConnectionFailedActionInternal();
            }
        }

        private void InitializeConnectionInternal()
        {
            duplexService.Connect();
            initializeConnectionAction();
        }

        private void ConnectionFailedActionInternal()
        {
            try
            {
                connectionFailedAction();
            }
            catch (Exception ex)
            {
                writeErrorAction("Error in connectionFailedAction:", ex);
            }

            EnableRestoreConnectionTimer();
        }

        private void EnableRestoreConnectionTimer(TimeSpan dueTime)
        {
            restoreConnTimer.Change(dueTime, RestoreConnTimerInterval);
        }

        private void EnableRestoreConnectionTimer()
        {
            EnableRestoreConnectionTimer(TimeSpan.Zero);
        }

        private void DisableRestoreConnectionTimer()
        {
            restoreConnTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void RestoreConnection()
        {
            restoring = true;
            DisableRestoreConnectionTimer();
            try
            {
                InitializeConnectionInternal();
            }
            catch (Exception ex)
            {
                writeErrorAction("Error in RestoreConnection:", ex);

                EnableRestoreConnectionTimer(RestoreConnTimerInterval);
            }
            finally
            {
                restoring = false;
            }
        }

        private void DuplexServiceConnectionLost(object sender, EventArgs e)
        {
            if (!restoring)
            {
                ConnectionFailedActionInternal();
            }
        }

        #region Dispose

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    KillTimer();
                    duplexService.ConnectionLost -= DuplexServiceConnectionLost;
                }

                disposed = true;
            }
        }

        private void KillTimer()
        {
            if (restoreConnTimer != null)
            {
                restoreConnTimer.Dispose();
            }
        }

        ~DuplexConnectionKeeper()
        {
            Dispose(false);
        }

        #endregion
    }
}
