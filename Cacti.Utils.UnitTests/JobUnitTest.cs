using Cacti.Utils.JobUtil;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cacti.Utils.UnitTests
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public async Task DoJob()
        {
            bool done = false;
            IJob job = new Job(() => { done = true; });

            await job.Execute(CancellationToken.None);

            Assert.AreEqual(done, true);
        }

        [TestMethod]
        public async Task ThenJob()
        {
            bool done = false;
            IJob job = new Job(() => { })
                .Then(new Job(() => { done = true; }));

            await job.Execute(CancellationToken.None);

            Assert.AreEqual(done, true);
        }

        [TestMethod]
        public async Task CancelJob()
        {
            bool done = false;
            IJob job = new Job(() => { })
                .Delay(TimeSpan.FromMilliseconds(100))
                .Then(new Job(() => { done = true; }));

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(TimeSpan.FromMilliseconds(50));

            try
            {
                await job.Execute(tokenSource.Token);
            }
            catch(TaskCanceledException)
            { }
            
            Assert.AreNotEqual(done, true);
        }

        [TestMethod]
        public async Task RepeatJob()
        {
            int repeatTimes = 0;
            IJob job = new Job(() => repeatTimes++)
                .Repeat(TimeSpan.FromMilliseconds(10));

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            tokenSource.CancelAfter(TimeSpan.FromMilliseconds(50));

            try
            {
                await job.Execute(tokenSource.Token);
            }
            catch(TaskCanceledException)
            { }
            
            Assert.AreEqual(repeatTimes, 5);
        }

        [TestMethod]
        public async Task TimesJob()
        {
            int repeatTimes = 0;
            IJob job = new Job(() => repeatTimes++)
                .Times(5);

            await job.Execute(CancellationToken.None);

            Assert.AreEqual(repeatTimes, 5);
        }

        [TestMethod]
        public async Task AggregateJob()
        {
            bool job1Done = false;
            bool job2Done = false;

            await new[]
            {
                new Job(() => job1Done = true),
                new Job(() => job2Done = true)
            }
            .Aggregate()
            .Execute(CancellationToken.None);

            Assert.AreEqual(job1Done, true);
            Assert.AreEqual(job2Done, true);
        }

        [TestMethod]
        public async Task ProcessJob()
        {
            LoginModel model = new LoginModel();
            await GetUserCrendentials(model)
                .Then(AuthenticateUser(model)
                    //handle authentication exception.
                    .Handle<ArgumentOutOfRangeException>())
                //Repeat get crendential and authentication 3 times if not authenticated
                .Times(3, () => model.User == null)
                .Execute(CancellationToken.None);

            Assert.IsNotNull(model.User);
        }

        private IJob GetUserCrendentials(LoginModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            //For test purposes.
            int retry = 2;

            return new Job(() =>
            {
                retry--;
                model.Login = retry == 0 ? nameof(model.Login) : string.Empty;
                model.Password = nameof(model.Password);
            });
        }

        private IJob AuthenticateUser(LoginModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));

            return new Job(() =>
            {
                if (model.Login != nameof(model.Login)
                || model.Password != nameof(model.Password))
                    throw new ArgumentOutOfRangeException();
                
                model.User = new object();
            });
        }

        private class LoginModel
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public object User { get; set; }
        }
    }
}

