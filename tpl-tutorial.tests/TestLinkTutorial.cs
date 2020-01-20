using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tpl_tutorial.models;

namespace Tests
{
    public class TestLinkTutorial
    {
        static void Log(string msg) => Console.WriteLine(msg);
        static Random rando = new Random();
        private const int _usersPerPage = 5;
        private const int _pageCount = 3;

        [Test]
        public async Task TestLinkTutorial_AllUsersAreProcessed()
        {
            // Setup
            var getUsersMock = new Mock<Func<int, Task<User[]>>>();
            getUsersMock.Setup(x => x(It.IsAny<int>())).Returns<int>((x) => GetUsersTask(x));
            var writeUsersMock = new Mock<Func<User, Task>>();
            writeUsersMock.Setup(x => x(It.IsAny<User>())).Returns<User>((x) => WriteUser(x));

            // Call
            await LinkTutorial.RunSampleOne(getUsersMock.Object, writeUsersMock.Object, _pageCount);

            // Validate "get data" called per page
            getUsersMock.Verify(x => x(It.IsAny<int>()), Times.Exactly(_pageCount));

            // Validate "write data" called per user
            writeUsersMock.Verify(x => x(It.IsAny<User>()), Times.Exactly(_usersPerPage * _pageCount));
        }

        [Test]
        public async Task TestLinkTutorial_WriteOnlyCalledOneAtATime()
        {
            // Setup
            var getUsersMock = new Mock<Func<int, Task<User[]>>>();
            getUsersMock.Setup(x => x(It.IsAny<int>())).Returns<int>((x) => GetUsersTask(x));

            var currentWriteCount = 0;
            var writeUsersMock = new Mock<Func<User, Task>>();
            writeUsersMock
                .Setup(x => x(It.IsAny<User>()))
                .Returns<User>(async (x) =>
                {
                    Assert.AreEqual(0, currentWriteCount);
                    Interlocked.Increment(ref currentWriteCount);
                    await WriteUser(x);
                    Interlocked.Decrement(ref currentWriteCount);
                });

            // Call
            await LinkTutorial.RunSampleOne(getUsersMock.Object, writeUsersMock.Object, _pageCount);
        }

        private async Task<User[]> GetUsersTask(int pageNumber)
        {
            await Task.Delay(rando.Next(250, 500));
            Log($"Getting {_usersPerPage} users for page {pageNumber}");
            var users = Enumerable.Range(0, _usersPerPage)
                .Select(_ => UserFactoryCreate())
                .ToArray();
            return users;
        }

        private async Task WriteUser(User user)
        {
            await Task.Delay(rando.Next(250, 500));
            Log($"Writing {user.Name}");
        }

        private static volatile int _userCount = 1;
        private User UserFactoryCreate()
        {
            var user = new User { Name = $"User #{_userCount}", Age = rando.Next(10, 50) };
            Interlocked.Increment(ref _userCount);
            return user;
        }
    }
}
