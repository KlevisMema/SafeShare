﻿using System.ComponentModel.DataAnnotations;

namespace SafeShare.ClientDTO.Expense;

public class ClientDto_ExpenseDelete
{
    [Required]
    public Guid ExpenseId { get; set; }
    public Guid UserId { get; set; }
    [Required]
    public decimal ExpenseAmount { get; set; }
    [Required]
    public Guid GroupId { get; set; }
}