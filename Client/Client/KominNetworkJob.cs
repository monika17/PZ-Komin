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

        public KominNetworkJob AddJob(uint opposite_id = uint.MaxValue, bool opposite_is_group = false)
        {
            KominNetworkJob job = new KominNetworkJob(++job_id, opposite_id, opposite_is_group);
            jobs.Add(job);
            return job;
        }

        public void FinishJob(KominNetworkJob job)
        {
            for (int i = 0; i < jobs.Count; i++)
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

        public void MarkNewArrival(uint job_id, KominNetworkPacket packet)
        {
            foreach (KominNetworkJob job in jobs)
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

        public void CancelJobs(uint opposite_id, bool opposite_is_group)
        {
            foreach (KominNetworkJob job in jobs)
                job.CancelJob(opposite_id, opposite_is_group);
        }

        public void CancelAllJobs()
        {
            foreach (KominNetworkJob job in jobs)
                job.CancelJob(job.opposite_id, job.opposite_is_group);
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
        public uint opposite_id { get; private set; }
        public bool opposite_is_group { get; private set; }
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

        public KominNetworkJob(uint id, uint opposite_id, bool opposite_is_group)
        {
            job_id = id;
            this.opposite_id = opposite_id;
            this.opposite_is_group = opposite_is_group;
            new_arrival = false;
            packet = null;
        }

        public void WaitForNewArrival()
        {
            while (new_arrival == false) ;
        }

        public void CancelJob(uint opposite_id, bool opposite_is_group)
        {
            if (this.opposite_id == uint.MaxValue)
                return;
            if ((this.opposite_id == opposite_id) && (this.opposite_is_group == opposite_is_group))
            {
                packet = null;
                new_arrival = true;
            }
        }
    }
}
