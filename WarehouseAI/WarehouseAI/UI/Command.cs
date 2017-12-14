using System;

namespace WarehouseAI.UI {
    /// <summary>
    /// A command that the user enters in the server console UI along with a description
    /// </summary>
    public struct Command {
        /// <summary>
        /// The actual command to be executed
        /// </summary>
        public Action<string[]> Action { get; }
        /// <summary>
        /// The description of the command (from help [command])
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Creates a new command for a user to enter
        /// </summary>
        /// <param name="action">The method to be executed</param>
        /// <param name="description">Description of the command for the user to see</param>
        public Command(Action<string[]> action, string description) {
            Action = action;
            Description = description;
        }
    }
}