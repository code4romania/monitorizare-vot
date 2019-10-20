using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities
{
    public partial class NotificationRegData
    {
        public int ObserverId { get; set; }
        public string ChannelName { get; set; }
        public string Token { get; set; }

        public virtual Observer Observer { get; set; }
    }
}
