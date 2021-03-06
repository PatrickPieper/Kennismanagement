
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace La_Game.Models
{

using System;
    using System.Collections.Generic;

    public partial class Participant
    {

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Participant()
        {

            this.Lesson_Participant = new HashSet<Lesson_Participant>();

            this.QuestionResults = new HashSet<QuestionResult>();

        }


        public int idParticipant { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public System.DateTime birthDate { get; set; }

        public Nullable<int> studentCode { get; set; }



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<Lesson_Participant> Lesson_Participant { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<QuestionResult> QuestionResults { get; set; }
        public string fullName {
            get { return firstName +" " + lastName; }
        }
        
    }

}
