using System;

namespace Urdms.Dmp.Database.Entities
{
    public class FormTimer
    {
        public virtual int Id { get; set; }
        public virtual int Step { get; set; }
        public virtual string UserId { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime EndTime { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var t = obj as FormTimer;
            if (t == null)
                return false;
            if (Id == t.Id && Step == t.Step)
                return true;
            return false;
        }
        public override int GetHashCode()
        {
            return (Id + "|" + Step).GetHashCode();
        }  
    }
}