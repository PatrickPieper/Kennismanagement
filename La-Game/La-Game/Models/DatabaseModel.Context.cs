﻿

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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;


public partial class LaGameDBContext : DbContext
{
    public LaGameDBContext()
        : base("name=LaGameDBContext")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<AnswerOption> AnswerOptions { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<Language_Member> Language_Member { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Lesson_Participant> Lesson_Participant { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Participant> Participants { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<QuestionList> QuestionLists { get; set; }

    public virtual DbSet<QuestionResult> QuestionResults { get; set; }

    public virtual DbSet<QuestionList_Question> QuestionList_Question { get; set; }

    public virtual DbSet<Lesson_QuestionList> Lesson_QuestionList { get; set; }

    public virtual DbSet<QuestionOrder> QuestionOrders { get; set; }

}

}

