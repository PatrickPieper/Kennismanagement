
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
    
public partial class Lesson_Participant
{

    public int Participant_idParticipant { get; set; }

    public int Lesson_idLesson { get; set; }

    public int idLesson_Participant { get; set; }



    public virtual Lesson Lesson { get; set; }

    public virtual Participant Participant { get; set; }

}

}
