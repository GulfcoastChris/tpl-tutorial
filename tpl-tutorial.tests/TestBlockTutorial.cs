using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using tpl_tutorial;
using tpl_tutorial.models;

namespace Tests
{
    public class TestBlockTutorial
    {
        static void Log(string msg) => Console.Error.WriteLine(msg);

        static Random rando = new Random();

        [Test]
        public async Task TestSampleOne()
        {
            var inputs = new[] { MockInputFactoryCreate(), MockInputFactoryCreate(), MockInputFactoryCreate() };

            await ActionBlockTutorial.RunSampleOne(
                action: async (input) => await input.Process(),
                items: inputs.Select(input => input.Object)
            );

            // Verify process was called
            foreach (var input in inputs)
            {
                input.Verify(obj => obj.Process(), $"Input {input.Object.Id} did not get processed");
            }
        }

        private static int _count = 1;

        private Mock<ISampleInput> MockInputFactoryCreate()
        {
            var localCount = _count++;

            var mockInput = new Mock<ISampleInput>();

            // Set ID
            mockInput
                .SetupGet(x => x.Id)
                .Returns(localCount);

            // When Process called sleep for a random amount so jobs will complete in a different order
            mockInput
                .Setup(input => input.Process())
                .Returns(() =>
                {
                    Thread.Sleep(rando.Next(0, 500));
                    Log($"Processing item {mockInput.Object.Id}");
                    return Task.CompletedTask;
                });

            return mockInput;
        }
    }
}