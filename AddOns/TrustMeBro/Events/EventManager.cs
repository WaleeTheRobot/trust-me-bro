using System;

namespace NinjaTrader.Custom.AddOns.TrustMeBro.Events
{
    public static class EventManager
    {
        public static event Action<string, bool> OnPrintMessage;

        public static void InvokeEvent(Action eventHandler)
        {
            try
            {
                eventHandler?.Invoke();
            }
            catch (Exception ex)
            {
                PrintMessage($"Error invoking event: {ex.Message}");
            }
        }

        public static void InvokeEvent<T>(Action<T> eventHandler, T arg)
        {
            try
            {
                eventHandler?.Invoke(arg);
            }
            catch (Exception ex)
            {
                PrintMessage($"Error invoking event: {ex.Message}");
            }
        }

        public static void InvokeEvent<T1, T2>(Action<T1, T2> eventHandler, T1 arg1, T2 arg2)
        {
            try
            {
                eventHandler?.Invoke(arg1, arg2);
            }
            catch (Exception ex)
            {
                PrintMessage($"Error invoking event: {ex.Message}");
            }
        }

        public static T InvokeEvent<T>(Func<T> eventHandler)
        {
            try
            {
                if (eventHandler != null)
                {
                    return eventHandler();
                }

                PrintMessage("Event handler is null");
                return default;
            }
            catch (Exception ex)
            {
                PrintMessage($"Error invoking event: {ex.Message}");
                return default;
            }
        }

        public static void PrintMessage(string eventMessage, bool addNewLine = false)
        {
            InvokeEvent(OnPrintMessage, eventMessage, addNewLine);
        }
    }
}
