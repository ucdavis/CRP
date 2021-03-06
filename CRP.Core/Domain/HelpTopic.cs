﻿using System.ComponentModel.DataAnnotations;
using CRP.Core.Validation.Extensions;
using UCDArch.Core.DomainModel;

namespace CRP.Core.Domain
{
    public class HelpTopic : DomainObject
    {
        public HelpTopic()
        {
            SetDefaults();
        }

        public virtual void SetDefaults()
        {
            IsActive = true;
            NumberOfReads = 0;
        }

        [Required]
        public virtual string Question { get; set; }

        public virtual string Answer { get; set; }

        public virtual bool AvailableToPublic { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int NumberOfReads { get; set; }

        public virtual bool IsVideo { get; set; }

        [StringLength(50)]
        public virtual string VideoName { get; set; }


        #region Complex Validation. Fields not in database

        [AssertFalse(ErrorMessage = "VideoName required when IsVideo selected")]
        public virtual bool IsVideoNeedsVideoName
        {
            get
            {
                if (IsVideo)
                {
                    if(string.IsNullOrEmpty(VideoName) || VideoName.Trim() == string.Empty)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        [AssertFalse(ErrorMessage = "Answer required when IsVideo not selected")]
        public virtual bool IsNotVideoNeedsAnswer
        {
            get
            {
                if (!IsVideo)
                {
                    if (string.IsNullOrWhiteSpace(Answer))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        #endregion Complex Validation. Fields not in database
    }
}
