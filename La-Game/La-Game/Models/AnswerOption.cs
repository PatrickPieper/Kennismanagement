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
    using System.ComponentModel.DataAnnotations;

    public partial class AnswerOption
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AnswerOption()
        {
            this.QuestionResults = new HashSet<QuestionResult>();
        }
    
        public int idAnswer { get; set; }
        public int Question_idQuestion { get; set; }
        public Nullable<short> correctAnswer { get; set; }
        [Required(ErrorMessage = "answer is required")]
        public string answerText { get; set; }
    
        public virtual Question Question { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuestionResult> QuestionResults { get; set; }
    }
}
