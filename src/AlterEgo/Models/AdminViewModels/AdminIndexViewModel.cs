using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterEgo.Models.AdminViewModels
{
    public class AdminIndexViewModel
    {
        #region Categories

        public List<Category> Categories { get; set; }

        // Create
        public string CategoryName { get; set; }
        public int CategoryReadableBy { get; set; }

        // Update
        public int CategorySortOrder { get; set; }

        // Delete
        public bool CategoryIsDeleted { get; set; }

        #endregion

        #region Forums

        public List<Forum> Forums { get; set; }

        // Create
        public string ForumName { get; set; }
        public string ForumDescription { get; set; }
        public int CategoryId { get; set; }
        public int ForumReadableBy { get; set; } = (int)GuildRank.Everyone;
        public int ForumCanStartThreads { get; set; } = (int)GuildRank.Triallist;
        public int ForumCanReplyToThreads { get; set; } = (int)GuildRank.Everyone;
        public int ForumCanLockThreads { get; set; } = (int)GuildRank.ForumAdmin;
        public int ForumCanStickyThreads { get; set; } = (int)GuildRank.ForumAdmin;
        public int ForumCanEditThreads { get; set; } = (int)GuildRank.ForumAdmin;
        public int ForumCanDeleteThreads { get; set; } = (int)GuildRank.ForumAdmin;

        // Update
        public int ForumSortOrder { get; set; }

        // Delete
        public bool ForumIsDeleted { get; set; }

        #endregion
    }
}
