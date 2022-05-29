using System;

namespace MurderMystery.API.Internal
{
    public abstract class MMEventHandler
    {
        internal MMEventHandler() => Name = GetType().Name;

        public abstract MurderMystery Plugin { get; }

        public string Name { get; }
        public bool Enabled { get; private set; }

        /// <summary>
        /// Toggles the handler.
        /// </summary>
        /// <param name="enable">Whether the handler should be enabled.</param>
        public void ToggleHandlers(bool enable)
        {
            if (Enabled ^ enable)
            {
                if (enable)
                    InternalEnable();
                else
                    InternalDisable();

                Enabled = enable;
            }
        }

        /// <summary>
        /// Calls <see cref="Enable"/> safely.
        /// </summary>
        private void InternalEnable()
        {
            MMLog.Debug("Enabling '" + Name + "'");

            try
            {
                Enable();
            }
            catch (Exception e)
            {
                MMLog.Error(string.Concat(
                    "Failed to enable events for event handler ",
                    Name,
                    "\n",
                    e.ToString()
                    ));
            }
        }

        /// <summary>
        /// Calls <see cref="Disable"/> safely.
        /// </summary>
        private void InternalDisable()
        {
            MMLog.Debug("Disabling '" + Name + "'");

            try
            {
                Disable();
            }
            catch (Exception e)
            {
                MMLog.Error(string.Concat(
                    "Failed to disable events for event handler ",
                    Name,
                    "\n",
                    e.ToString()
                    ));
            }
        }

        /// <summary>
        /// Called when the handler is enabled.
        /// </summary>
        protected abstract void Enable();

        /// <summary>
        /// Called when the handler is disabled.
        /// </summary>
        protected abstract void Disable();
    }
}
