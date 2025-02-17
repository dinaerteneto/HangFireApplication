using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HangFireApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {

        [HttpGet]
        public void ListaInteiros()
        {
            for (int i = 0; i < 10000; i++)
            {
                Console.WriteLine(i);
            }
        }


        [HttpPost]
        [Route("CriarBackgroundJob")]
        public ActionResult CriarBackgroundJob()
        {
            BackgroundJob.Enqueue(() => ListaInteiros());

            return Ok();
        }

        [HttpPost]
        [Route("CriarScheduledJob")]
        public ActionResult CriarScheduledJob()
        {
            var scheduleDateTime = DateTime.Now.AddSeconds(5);
            var dateTimeOffSet = new DateTimeOffset(scheduleDateTime);

            BackgroundJob.Schedule(() => Console.WriteLine("Tarefa agendada!"), dateTimeOffSet);

            return Ok();
        }

        [HttpPost]
        [Route("CriarContinuationJob")]
        public ActionResult CriarContinuationJob()
        {
            var scheduleDateTime = DateTime.Now.AddSeconds(5);
            var dateTimeOffSet = new DateTimeOffset(scheduleDateTime);

            var job1 = BackgroundJob.Schedule(() => Console.WriteLine("Tarefa agendada!"), dateTimeOffSet);
            var job2 = BackgroundJob.ContinueJobWith(job1, () => Console.WriteLine("Job 2!"));
            var job3 = BackgroundJob.ContinueJobWith(job2, () => Console.WriteLine("Job 3!"));
            var job4 = BackgroundJob.ContinueJobWith(job3, () => Console.WriteLine("Job 4!"));

            return Ok();
        }

        [HttpPost]
        [Route("CriarRecurringJob")]
        public ActionResult CriarRecurringJob()
        {
            
            RecurringJob.AddOrUpdate(() => Console.WriteLine("Recurring job"), "* * * * *");

            return Ok();
        }


    }
}
