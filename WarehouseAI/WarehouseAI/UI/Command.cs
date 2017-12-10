using System;

namespace WarehouseAI.UI {
    public struct Command {
        public Action<string[]> Action { get; }
        public string Description { get; }

        public Command(Action<string[]> action, string description) {
            Action = action;
            Description = description;
        }
    }
}