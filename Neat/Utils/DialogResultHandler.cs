using Neat.Services;
using Neat.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Neat
{
    public class DialogResultHandler
    {
        public Type DialogResultType { get; }

        private readonly Action<DialogInputResult> context;

        public DialogResultHandler(Type resultType, Action<DialogInputResult> context)
        {
            this.context = context;
            DialogResultType = resultType;
        }

        public void HandleResult(DialogInputResult result)
        {
            if (result.GetType() != DialogResultType) return;
            context.Invoke(result);
        }
    }

    public class DefaultDialogResultProcessor : IEnumerable<DialogResultHandler>, IDialogResultProcessor
    {
        private readonly HashSet<DialogResultHandler> handlers;

        public DefaultDialogResultProcessor()
        {
            handlers = new HashSet<DialogResultHandler>();
        }

        public void ProcessResult(DialogInputResult result)
        {
            var filtered = handlers.Where(h => h.DialogResultType == result.GetType()).ToList();

            for (var i = filtered.Count - 1; i >= 0; i--)
            {
                filtered[i].HandleResult(result);
            }
        }

        public IEnumerator<DialogResultHandler> GetEnumerator()
        {
            return handlers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddHandler(DialogResultHandler handler)
        {
            handlers.Add(handler);
        }
    }
}
