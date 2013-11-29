using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Komin
{
    public class KominNetworkJobHolder
    {
        private List<KominNetworkJob> jobs;
        private uint job_id;

        public KominNetworkJobHolder()
        {
            jobs = new List<KominNetworkJob>();
            job_id = 0;
        }

        public KominNetworkJob AddJob()
        {
            KominNetworkJob job = new KominNetworkJob(++job_id);
            jobs.Add(job);
            return job;
        }

        public void FinishJob(KominNetworkJob job)
        {
            for(int i=0; i<jobs.Count; i++)
                if (jobs[i].JobID == job.JobID)
                {
                    jobs.RemoveAt(i);
                    break;
                }
        }

        public void Restart()
        {
            WaitForJobsFinished();
            job_id = 0;
        }

        public void MarkNewArrival(uint job_id, ref KominNetworkPacket packet)
        {
            foreach(KominNetworkJob job in jobs)
                if (job.JobID == job_id)
                {
                    job.Packet = packet;
                    break;
                }
        }

        public void WaitForJobsFinished()
        {
            while (jobs.Count > 0) ;
        }
    }

    public class KominNetworkJob
    {
        private uint job_id;
        public uint JobID
        {
            get
            {
                return job_id;
            }
        }
        private bool new_arrival;
        private KominNetworkPacket packet;
        public KominNetworkPacket Packet
        {
            set
            {
                packet = value;
                new_arrival = true;
            }
            get
            {
                new_arrival = false;
                return packet;
            }
        }

        public KominNetworkJob(uint id)
        {
            job_id = id;
            new_arrival = false;
            packet = null;
        }

        public void WaitForNewArrival()
        {
            while (new_arrival == false) ;
        }
    }
}
