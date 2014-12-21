using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using TreeFrogs.WcfProxy.Interfaces;

namespace TreeFrogs.WcfProxy.Services
{
    /// <summary>
    /// Base class for all service with duplex channels
    /// </summary>
    public abstract class BaseServiceDuplex<TCallBack> : IBaseDuplexContract
    {
        private readonly Dictionary<string, CallBackContainer> clientsCallbacks = new Dictionary<string, CallBackContainer>();
        private readonly object callBackCollectionLocker = new object();

        /// <summary>
        /// Connect to host
        /// </summary>
        public virtual void Connect()
        {
            var sessionId = OperationContext.Current.SessionId;
            if (sessionId != null)
            {
                lock (callBackCollectionLocker)
                {
                    if (!clientsCallbacks.ContainsKey(sessionId))
                    {
                        var callBack = OperationContext.Current.GetCallbackChannel<TCallBack>();
                        var channel = OperationContext.Current.Channel;
                        channel.Faulted += ChannelOnLost;
                        channel.Closed += ChannelOnLost;
                        clientsCallbacks.Add(sessionId, new CallBackContainer {CallBack = callBack, ContextChannel = channel});
                    }
                }
            }
        }

        private void ChannelOnLost(object sender, EventArgs eventArgs)
        {
            var contextChannel = sender as IContextChannel;
            if (contextChannel != null)
            {
                RemoveCallback(contextChannel.InputSession.Id);
            }
        }

        private void RemoveCallback(string sessionId) 
        {
            if (sessionId != null)
            {
                lock (callBackCollectionLocker)
                {
                    if (clientsCallbacks.ContainsKey(sessionId))
                    {
                        var channel = clientsCallbacks[sessionId].ContextChannel;
                        channel.Faulted -= ChannelOnLost;
                        channel.Closed -= ChannelOnLost;
                        try
                        {
                            channel.Close();
                        }
                        catch (CommunicationObjectFaultedException)
                        {
                            channel.Abort();
                        }
                        catch (TimeoutException)
                        {
                            channel.Abort();
                        }

                        clientsCallbacks.Remove(sessionId);
                    }
                }
            }
        }

        /// <summary>
        /// Dissconnect
        /// </summary>
        public virtual void Disconnect()
        {
            RemoveCallback(OperationContext.Current.SessionId);
        }

        /// <summary>
        /// Execute action for all client callbacks
        /// </summary>
        /// <param name="executor">Action for execute</param>
        protected virtual void ExecuteCallbacks(Action<TCallBack> executor)
        {
            var sessionsToDelete = new List<string>();
            IEnumerable<KeyValuePair<string, CallBackContainer>> callBacksSnapshot;

            lock (callBackCollectionLocker)
            {
                callBacksSnapshot = clientsCallbacks.ToArray();
            }

            foreach (var keyValue in callBacksSnapshot)
            {
                var channelState = keyValue.Value.ContextChannel.State;
                if (channelState != CommunicationState.Opened && channelState != CommunicationState.Opening)
                {
                    sessionsToDelete.Add(keyValue.Key);
                }
                else
                {
                    try
                    {
                        executor(keyValue.Value.CallBack);
                    }
                    catch (TimeoutException ex)
                    {
                        LogSystemException(ex);
                        sessionsToDelete.Add(keyValue.Key);
                    }
                    catch (CommunicationException ex)
                    {
                        LogSystemException(ex);
                        sessionsToDelete.Add(keyValue.Key);
                    }
                }
            }

            if (sessionsToDelete.Count > 0)
            {
                sessionsToDelete.ForEach(RemoveCallback);
            }
        }

        protected virtual void LogSystemException(Exception exception)
        {
        }

        #region Nested class
        private class CallBackContainer
        {
            public TCallBack CallBack { get; set; }
            public IContextChannel ContextChannel { get; set; }
        }
        #endregion

    }
}
