using System;
using Sitecore.Commerce.Core;

namespace Plugin.Sample.Settings.Entities
{
    public class Setting : CommerceEntity
    {
        public bool IsActive { get; set; }
        public Setting()
        {
            this.DateCreated = DateTime.UtcNow;
            this.DateUpdated = this.DateCreated;
        }

        public Setting(string id) : this()
        {
            this.Id = id;
        }
    }
}
