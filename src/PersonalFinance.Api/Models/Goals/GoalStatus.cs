namespace PersonalFinance.Api.Models.Goals;

public enum GoalStatus 
{
    Pending = 0,     // Criada, mas sem movimentação inicial
    InProgress = 1,  // Ativa e dentro do prazo
    Paused = 2,      // O usuário parou de poupar temporariamente
    Finished = 3,    // Objetivo atingido com sucesso!
    Canceled = 4,    // O usuário desistiu da meta
    Overdue = 5      // O prazo venceu e o objetivo não foi alcançado
}