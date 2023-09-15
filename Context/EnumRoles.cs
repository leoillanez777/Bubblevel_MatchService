using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bubblevel_MatchService.Context
{
  public enum EnumRoles
  {
    [Display(Name = "Super Admin", Description = "All access")]
    SuperAdmin,
    [Display(Name = "Admin", Description = "Has access to everything except user management.")]
    Admin,
    [Display(Name = "Customer", Description = "With this role, you have all the permissions to edit, add, and delete customers.")]
    Customer,
    [Display(Name = "Add Customer", Description = "With this role, you only have permission to add new customers.")]
    CustomerAdd,
    [Display(Name = "Edit Customer", Description = "With this role, you only have permission to edit customers.")]
    CustomerEdit,
    [Display(Name = "Delete Customer", Description = "With this role, you only have permission to delete customers.")]
    CustomerDelete,
    [Display(Name = "Project", Description = "With this role, you have all the permissions to edit, add, and delete projects.")]
    Project,
    [Display(Name = "Add Project", Description = "With this role, you only have permission to add new projects.")]
    ProjectAdd,
    [Display(Name = "Edit Customer", Description = "With this role, you only have permission to edit projects.")]
    ProjectEdit,
    [Display(Name = "Delete Customer", Description = "With this role, you only have permission to delete projects.")]
    ProjectDelete,
    [Display(Name = "Comment", Description = "With this role, you have all the permissions to edit, add, and delete comments.")]
    Comment,
    [Display(Name = "Add Comment", Description = "With this role, you only have permission to add new comments.")]
    CommentAdd,
    [Display(Name = "Edit Comment", Description = "With this role, you only have permission to edit comments.")]
    CommentEdit,
    [Display(Name = "Delete Comment", Description = "With this role, you only have permission to delete comments.")]
    CommentDelete,
    [Display(Name = "Intervention", Description = "With this role, you have all the permissions to edit, add, and delete interventions.")]
    Intervention,
    [Display(Name = "Add Intervention", Description = "With this role, you only have permission to add new interventions.")]
    InterventionAdd,
    [Display(Name = "Edit Intervention", Description = "With this role, you only have permission to edit interventions.")]
    InterventionEdit,
    [Display(Name = "Delete Intervention", Description = "With this role, you only have permission to delete interventions.")]
    InterventionDelete,
    [Display(Name = "Support Incident", Description = "With this role, you can add, edit, and delete support incidents in all states.")]
    SupportIncident,
    [Display(Name = "Add Support Incident", Description = "With this role, you can only add in all support incident states.")]
    SupportIncidentAdd,
    [Display(Name = "Edit Support Incident", Description = "With this role, you can only edit in all support incident states.")]
    SupportIncidentEdit,
    [Display(Name = "Delete Support Incident", Description = "With this role, you can only delete in all support incident states.")]
    SupportIncidentDelete,
    [Display(Name = "Launch", Description = "With this role, you can add, edit, and delete support incidents only in the pending state.")]
    Launch,
    [Display(Name = "Add In Launch", Description = "With this role, you can add support incidents only in the pending state.")]
    LaunchAdd,
    [Display(Name = "Edit In Launch", Description = "With this role, you can edit support incidents only in the pending state.")]
    LaunchEdit,
    [Display(Name = "Delete In Launch", Description = "With this role, you can delete support incidents only in the pending state.")]
    LaunchDelete,
    [Display(Name = "Awaiting", Description = "With this role, you can add, edit, and delete support incidents only in the awaiting state.")]
    Awaiting,
    [Display(Name = "Add In Awaiting", Description = "With this role, you can add support incidents only in the awaiting state.")]
    AwaitingAdd,
    [Display(Name = "Edit In Awaiting", Description = "With this role, you can edit support incidents only in the awaiting state.")]
    AwaitingEdit,
    [Display(Name = "Delete In Awaiting", Description = "With this role, you can delete support incidents only in the awaiting state.")]
    AwaitingDelete,
    [Display(Name = "In Progress", Description = "With this role, you can add, edit, and delete support incidents only in the progress state.")]
    InProgress,
    [Display(Name = "Add In Progress", Description = "With this role, you can add support incidents only in the progress state.")]
    InProgressAdd,
    [Display(Name = "Edit In Progress", Description = "With this role, you can edit support incidents only in the progress state.")]
    InProgressEdit,
    [Display(Name = "Delete In Progress", Description = "With this role, you can delete support incidents only in the progress state.")]
    InProgressDelete,
    [Display(Name = "Solved", Description = "With this role, you can close, re-open, and delete support incidents only in the solved state.")]
    Solved,
    [Display(Name = "Close In Solved", Description = "With this role, you can close support incidents only in the solved state.")]
    SolvedClose,
    [Display(Name = "Re-Open In Solved", Description = "With this role, you re-open edit support incidents only in the solved state.")]
    SolvedReOpen,
    [Display(Name = "Delete In Solved", Description = "With this role, you can delete support incidents only in the solved state.")]
    SolvedDelete,
    [Display(Name = "Report", Description = "With this role, you can view report.")]
    Report,
  }
}

