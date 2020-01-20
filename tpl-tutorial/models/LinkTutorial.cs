using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace tpl_tutorial.models
{
    public static class LinkTutorial
    {
        public static async Task RunSampleOne(Func<int, Task<User[]>> getUsers, Func<User, Task> writeUser, int pageCount)
        {
            var getDataBlock = new TransformManyBlock<int, User>(
                transform: async (pageNumber) => await getUsers(pageNumber), 
                dataflowBlockOptions: new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

            var writeDataBlock = new ActionBlock<User>(
                action: async (user) => await writeUser(user),
                dataflowBlockOptions: new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1 });

            getDataBlock.LinkTo(writeDataBlock, new DataflowLinkOptions { PropagateCompletion = true });

            for (var i = 1; i <= pageCount; i++)
                getDataBlock.Post(i);

            getDataBlock.Complete();
            await writeDataBlock.Completion;
        }
    }

    public interface IUserService
    {
        
    }

    public class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
