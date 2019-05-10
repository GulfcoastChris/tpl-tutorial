using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace tpl_tutorial.models
{
    public static class ActionBlockTutorial
    {
        public struct Settings 
        {
            public const int MaxParallelCount = 1;
        };

        public static async Task RunSampleOne<T>(Action<T> action, IEnumerable<T> items) 
        {
            var block = new ActionBlock<T>(action, new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Settings.MaxParallelCount
            });

            foreach (var item in items)
            {
                block.Post(item);
            }

            block.Complete();

            await block.Completion;
        }
    }
}
